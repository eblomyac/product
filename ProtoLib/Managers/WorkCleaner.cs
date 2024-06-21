using System.Data;
using System.Diagnostics;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using KSK_LIB.Maconomy;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class WorkCleaner
{
    public void RemoveError()
    {
        long startId = 93381; // >
        long endId = 99766; // <=

        startId = 99766;
        endId = 102708;


        using (BaseContext c = new BaseContext(""))
        {
            using (var transaction = c.Database.BeginTransaction())
            {
                var wss = c.WorkStatusLogs.Where(x => x.Id > 93381 && x.Id <= 99766).ToList();
                var byWork = wss.GroupBy(x => x.WorkId);
                int loop = 0;

                foreach (var workLogs in byWork)
                {
                    long workId = workLogs.Key;
                    int prevStatus = workLogs.Min(x => (int)x.PrevStatus);


                    var work = c.Works.FirstOrDefault(x => x.Id == workId);
                    work.Status = (WorkStatus)prevStatus;
                    if (work.Status != WorkStatus.sended && !string.IsNullOrWhiteSpace(work.MovedTo))
                    {
                        work.MovedTo = "";
                    }

                    if (work.Status == WorkStatus.hidden)
                    {
                        work.MovedFrom = "";
                    }

                    loop++;
                    Debug.WriteLine($"{loop}\\{byWork.Count()}");
                }

                Debug.WriteLine("Saving");

                c.SaveChanges();
                transaction.Commit();
            }
        }
    }

    public string Clean()
    {
        using (BaseContext bc = new BaseContext(""))
        {
            var actualWorks = bc.Works.AsNoTracking().Include(x => x.Issues).ThenInclude(x => x.Template).AsNoTracking()
                .Include(x => x.Post).AsNoTracking().Where(x => x.Status != WorkStatus.ended).ToList();
            var actualOrders = actualWorks.Select(x => x.OrderNumber).Distinct().ToList();
            string s = string.Join(',', actualOrders.Select(x => $"'{x}'"));

            StringBuilder sb = new StringBuilder();
            using (MaconomyBase mb = new MaconomyBase())
            {
                sb.AppendLine($"Количество не закрытых работ {actualWorks.Count}, в {actualOrders.Count} заказах");
                var t = mb.getTableFromDB(
                    $"SELECT ProductionLine.LineNumber, ProductionLine.TransactionNumber,ITEMNUMBER, ENTRYTEXT, PRODUCTIONDATE, FINISHEDITEMLOCATION,  " +
                    $"ProductionLine.numberproduction as numberof  from" +
                    $" ProductionLine left join ProductionVoucher on ProductionLine.TransactionNumber = ProductionVoucher.TransactionNumber " +
                    $"where ProductionLine.TransactionNumber in ({s})");
                WorkStatusChanger wss = new WorkStatusChanger();
                IssueManager im = new IssueManager();
                foreach (DataRow r in t.Rows)
                {
                    string article = r["ITEMNUMBER"].ToString();
                    long order = long.Parse(r["TransactionNumber"].ToString());
                    int lineNumber = int.Parse(r["LineNumber"].ToString());


                    int numberof = int.Parse(r["numberof"].ToString());

                    bool isTotalEnded = numberof == 0;


                    var rowWorks = actualWorks.Where(x => x.OrderNumber == order && x.Article == article)
                        .OrderBy(x => x.Post.ProductOrder).ToList();
                    if (rowWorks.Count > 1)
                    {
                        rowWorks = actualWorks.Where(x => x.OrderNumber == order && x.OrderLineNumber == lineNumber)
                            .OrderBy(x => x.Post.ProductOrder).ToList();
                    }


                    if (isTotalEnded)
                    {
                        for (int loop = 0; loop < rowWorks.Count; loop++)
                        {
                            sb.AppendLine(
                                $"Обнаружено расхождение в артикуле {article}, заказ: {order}, произв. задание в макономи выполнено, а в ДП нет");
                            var toEndWork = rowWorks[loop];
                            string sendTo = "";

                            if (loop != rowWorks.Count - 1)
                            {
                                sendTo = rowWorks[loop + 1].PostId;
                            }
                            else
                            {
                                sendTo = Constants.Work.EndPosts.TotalEnd;
                            }

                            sb.AppendLine(
                                $"\t{toEndWork.PostId} не выполнено, утерянный норматив: {toEndWork.TotalCost}");
                            if (toEndWork.Issues.Where(x => x.Resolved.HasValue == false).Count() > 0)
                            {
                                foreach (var issue in toEndWork.Issues.Where(x => x.Resolved.HasValue == false))
                                {
                                    sb.AppendLine(
                                        $"\t\tСобытие {issue.Template.Name}: {issue.Description} было принудительно разрешено");
                                    im.ResolveIssue(issue.Id, "system");
                                }
                            }

                            for (int loopStatus = (int)toEndWork.Status + 10;
                                 loopStatus <= (int)WorkStatus.ended;
                                 loopStatus += 10)
                            {
                                wss.ChangeStatus(toEndWork.Id, (WorkStatus)loopStatus, "system", sendTo);
                            }
                        }
                    }
                }
            }

            return sb.ToString();
        }
    }

    private string CloseWork(BaseContext c,Work toEndWork, string sendTo)
    {
        StringBuilder sb = new StringBuilder();

        if (toEndWork.Issues != null && toEndWork.Issues.Count > 0)
        {
            IssueManager ism = new IssueManager(c);
            foreach (var issue in toEndWork.Issues.Where(x => x.Resolved == null))
            {
                ism.ResolveIssue(issue.Id, "system");
                sb.AppendLine($"Событие {issue.Description} завершено");
            }
        }

        WorkStatusChanger wss = new WorkStatusChanger(c);
        for (int loopStatus = (int)toEndWork.Status + 10; loopStatus <= (int)WorkStatus.ended; loopStatus += 10)
        {
            sb.AppendLine(
                $"Смена статуса работы ({toEndWork.Id}) {toEndWork.OrderNumber}-{toEndWork.OrderLineNumber} {toEndWork.Article}x{toEndWork.Count} с {toEndWork.Status} на {loopStatus}");
            wss.ChangeStatus(toEndWork.Id, (WorkStatus)loopStatus, "system", sendTo);
        }

        return sb.ToString();
    }
  public string CleanByMovement(DateTime stamp)
    {
        using (BaseContext bc = new BaseContext(""))
        {
            var actualWorks = bc.Works.AsNoTracking().Include(x => x.Issues).ThenInclude(x => x.Template).AsNoTracking()
                .Include(x => x.Post).AsNoTracking().Where(x => x.Status != WorkStatus.ended && x.Post.CanEnd).ToList();
            var actualOrders = actualWorks.Select(x => x.OrderNumber).Distinct().ToList();
            string s = string.Join(',', actualOrders.Select(x => $"'{x}'"));

           
            StringBuilder sb = new StringBuilder();
            using (BaseContext c = new BaseContext(""))
            {
                
                using (MaconomyBase mb = new MaconomyBase())
                {
                    sb.AppendLine($"Количество не закрытых работ {actualWorks.Count}, в {actualOrders.Count} заказах");

                    var productionLines = mb.getTableFromDB(
                        $"SELECT ProductionLine.LineNumber, ProductionLine.TransactionNumber,ITEMNUMBER, ENTRYTEXT, PRODUCTIONDATE, FINISHEDITEMLOCATION,  " +
                        $"ProductionLine.numberproduction as numberof  from" +
                        $" ProductionLine left join ProductionVoucher on ProductionLine.TransactionNumber = ProductionVoucher.TransactionNumber " +
                        $"where ProductionLine.TransactionNumber in ({s})");

                    var closedArticles = mb.getTableFromDB(
                        $"SELECT * FROM KSK.A___ITEMMOVEMENTSVIE WHERE PRODUCTIONTRANSACTIONNUMBER in ({s}) and APPROVALDATE='{stamp:yyyy.MM.dd}' and MOVEMENTVOUCHERTYPE=0");

                    var transactions =
                        mb.getTableFromDB(
                            $"SELECT DISTINCT TransactionNumber FROM KSK.A___ITEMMOVEMENTSVIE WHERE PRODUCTIONTRANSACTIONNUMBER in ({s}) and APPROVALDATE='{stamp:yyyy.MM.dd}' and MOVEMENTVOUCHERTYPE=0");
                    foreach (DataRow transaction in transactions.Rows)
                    {
                        StringBuilder myLog = new StringBuilder();
                        string trId = transaction[0].ToString();
                        long parsedTrId = long.Parse(trId);
                        myLog.AppendLine($"Транзакция {trId}");
                        DataRow[] transactonContent = closedArticles.Select($"TransactionNumber='{trId}'");

                        var existMovement = c.MaconomyMovementTransactions.FirstOrDefault(x =>
                            x.TransactionId == parsedTrId && x.IsSuccess == true);
                        if (existMovement != null)
                        {
                            myLog.AppendLine($"Уже проведена {existMovement.Stamp}, лог: {existMovement.LogFile}");
                            continue;
                        }
                        
                        MaconomyMovementTransaction mmt = new();
                        using (var baseTransaction = c.Database.BeginTransaction())
                        {
                            myLog.AppendLine($"Создана внутренняя транзакция: {baseTransaction.TransactionId}");
                            bool isOk = true;
                            try
                            {
                                WorkManagerFacade wmf = new WorkManagerFacade(c);

                                foreach (DataRow closedArticle in transactonContent)
                                {
                                    long orderNumber =
                                        long.Parse(closedArticle["ProductionTransactionNumber"].ToString());
                                    string article = closedArticle["ItemNumber"].ToString();
                                    int count = int.Parse(closedArticle["NumberOf"].ToString());

                                    var postsWorks = actualWorks
                                        .Where(x => x.OrderNumber == orderNumber && x.Article == article)
                                        .ToList()
                                        .OrderBy(x => x.Post.ProductOrder).GroupBy(x => x.PostId);
                                    if (postsWorks.Count() > 0)
                                    {
                                        myLog.AppendLine(
                                            $"Транзакция № {closedArticle["TransactionNumber"]} для заказа {closedArticle["ProductionTransactionNumber"]}");
                                        myLog.AppendLine(
                                            $"Артикул {closedArticle["ItemNumber"]} количество {closedArticle["NumberOf"]}");
                                    }

                                    var postQueue = postsWorks.Select(x => x.Key).ToList();

                                    foreach (var postWorks in postsWorks)
                                    {
                                        string nextPost = "";
                                        int current = postQueue.IndexOf(postWorks.Key);
                                        if (current == postQueue.Count - 1)
                                        {
                                            nextPost = Constants.Work.EndPosts.TotalEnd;
                                        }
                                        else
                                        {
                                            nextPost = postQueue[current + 1];
                                        }

                                        myLog.AppendLine($"Участок: {postWorks.Key} (следующий участок: {nextPost})");


                                        if (postWorks.Count() == 1)
                                        {
                                            //значит работа одна

                                            var singleWork = postWorks.First();
                                            if (singleWork.Count == count)
                                            {
                                                myLog.AppendLine(
                                                    $"Идеальный случай, работа одна и количество совпадает. Закрываем");
                                                //идеальный случай
                                                //close work
                                                myLog.AppendLine(CloseWork(c, singleWork, nextPost));
                                            }
                                            else
                                            {
                                                if (singleWork.Count > count)
                                                {
                                                    //спишем часть
                                                    myLog.AppendLine(
                                                        $"Сдана меньшая часть, делим работы и закрываем сданную часть. Было {singleWork.Count}, отделяем {count}");

                                                    //split && close
                                                    var splitResult = wmf.SplitWork(singleWork, count);
                                                    if (splitResult.Count > 1)
                                                    {
                                                        for (int loop = 1; loop <= splitResult.Count; loop += 2)
                                                        {
                                                            myLog.AppendLine(CloseWork(c, splitResult[loop], nextPost));
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    myLog.AppendLine(
                                                        $"Сдается большая часть. Косяк! закроем все что есть");
                                                    //по идее быть такого не должно

                                                    //close
                                                    myLog.AppendLine(CloseWork(c, singleWork, nextPost));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //понять это разделенная работа или есть несколько заказов на произв.
                                            var productionArticleRows =
                                                productionLines.Select(
                                                    $"ItemNumber='{article}' and TransactionNumber='{orderNumber}'"); // количество заказов на произв.
                                            var groupedByLines = postWorks.GroupBy(x => x.OrderLineNumber);
                                            bool isSplitted = groupedByLines.Count() > 1;
                                            bool isMultiOrder = productionArticleRows.Length > 1;
                                            myLog.AppendLine(
                                                $"Несколько работ {postWorks.Count()}. Разделена {isSplitted}, несколько строк {isMultiOrder}");

                                            //поищем работу с точным совпадением по количеству, если такая есть закроем ее.
                                            //сли нет, поочередно будем закрывать работы с наименьшим количеством, пока не доберем нужно количество

                                            var exactWork = postWorks.FirstOrDefault(x => x.Count == count);
                                            if (exactWork != null)
                                            {
                                                myLog.AppendLine(
                                                    $"Надена работа с точным совпадением по количеству: {exactWork.Id}");
                                                //close 
                                                myLog.AppendLine(CloseWork(c, exactWork, nextPost));
                                            }
                                            else
                                            {
                                                int closedCount = 0;
                                                int totalWorkCount = postWorks.Sum(x => x.Count);
                                                if (count > totalWorkCount)
                                                {
                                                    myLog.AppendLine(
                                                        $"Количество из транзакции {count} превышает количество в работах {totalWorkCount}, косяк. Уменьшено ");
                                                    count = totalWorkCount;
                                                }

                                                int loop = 0;
                                                while (closedCount != count)
                                                {
                                                    myLog.AppendLine(
                                                        $"Начинаем списывать. Текущая итерация {loop}, сейчас списано {closedCount}");
                                                    var currentWork = postWorks.Skip(loop).FirstOrDefault();
                                                    if (currentWork == null)
                                                    {
                                                        myLog.AppendLine($"Не найдена работа чтобы списать, косяк");
                                                        break;
                                                    }

                                                    int takedCount = 0;
                                                    if (currentWork.Count > count - closedCount)
                                                    {
                                                        //частично списать
                                                        //split && close
                                                        takedCount = count - closedCount;

                                                        myLog.AppendLine(
                                                            $"Работа ({currentWork.Id}) {currentWork.OrderNumber}-{currentWork.OrderLineNumber} {currentWork.Article}x{currentWork.Count}. Будет разделена, отделим {takedCount}");
                                                        var splitResult = wmf.SplitWork(currentWork, takedCount);
                                                        if (splitResult.Count > 1)
                                                        {
                                                            for (int loopClose = 1;
                                                                 loopClose <= splitResult.Count;
                                                                 loopClose += 2)
                                                            {
                                                                myLog.AppendLine(CloseWork(c, splitResult[loopClose],
                                                                    nextPost));
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //списать все в работе
                                                        //close
                                                        takedCount = currentWork.Count;
                                                        myLog.AppendLine(
                                                            $"Работа ({currentWork.Id}) {currentWork.OrderNumber}-{currentWork.OrderLineNumber} {currentWork.Article}x{currentWork.Count}. Закроем {takedCount}");
                                                        myLog.AppendLine(CloseWork(c, currentWork, nextPost));
                                                    }

                                                    loop++;
                                                    closedCount += takedCount;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            catch (Exception exc)
                            {
                                myLog.AppendLine($"Отказ: {exc.Message}");
                                baseTransaction.Rollback();
                                isOk = false;
                            }
                            
                            sb.AppendLine(myLog.ToString());
                            if (isOk)
                            {
                                baseTransaction.Commit();
                            }
                           
                            mmt.TransactionId = parsedTrId;
                            mmt.IsSuccess = isOk;
                            mmt.Stamp = DateTime.Now;
                            string directory = Path.Combine(Environment.CurrentDirectory,"clean-log");
                            Directory.CreateDirectory(directory);
                            string fileName = trId+"-"+baseTransaction.TransactionId+".log";
                            using (StreamWriter sw = new StreamWriter(Path.Combine(directory, fileName)))
                            {
                                sw.WriteLine(myLog);
                            }

                            mmt.LogFile = Path.Combine(directory, fileName);
                            c.MaconomyMovementTransactions.Add(mmt);
                            


                        }

                        c.MaconomyMovementTransactions.Add(mmt);
                        c.SaveChanges();
                    }
                }
            }

            return sb.ToString();
        }
    }
    public string CleanByMovement2(DateTime stamp)
    {
        using (BaseContext bc = new BaseContext(""))
        {
            var actualWorks = bc.Works.AsNoTracking().Include(x => x.Issues).ThenInclude(x => x.Template).AsNoTracking()
                .Include(x => x.Post).AsNoTracking().Where(x => x.Status != WorkStatus.ended).ToList();
            var actualOrders = actualWorks.Select(x => x.OrderNumber).Distinct().ToList();
            string s = string.Join(',', actualOrders.Select(x => $"'{x}'"));

           
            StringBuilder sb = new StringBuilder();
            using (BaseContext c = new BaseContext(""))
            {
                
                using (MaconomyBase mb = new MaconomyBase())
                {
                    sb.AppendLine($"Количество не закрытых работ {actualWorks.Count}, в {actualOrders.Count} заказах");

                    var productionLines = mb.getTableFromDB(
                        $"SELECT ProductionLine.LineNumber, ProductionLine.TransactionNumber,ITEMNUMBER, ENTRYTEXT, PRODUCTIONDATE, FINISHEDITEMLOCATION,  " +
                        $"ProductionLine.numberproduction as numberof  from" +
                        $" ProductionLine left join ProductionVoucher on ProductionLine.TransactionNumber = ProductionVoucher.TransactionNumber " +
                        $"where ProductionLine.TransactionNumber in ({s})");

                    var closedArticles = mb.getTableFromDB(
                        $"SELECT * FROM KSK.A___ITEMMOVEMENTSVIE WHERE PRODUCTIONTRANSACTIONNUMBER in ({s}) and APPROVALDATE='{stamp:yyyy.MM.dd}' and MOVEMENTVOUCHERTYPE=0");

                    var transactions =
                        mb.getTableFromDB(
                            $"SELECT DISTINCT TransactionNumber FROM KSK.A___ITEMMOVEMENTSVIE WHERE PRODUCTIONTRANSACTIONNUMBER in ({s}) and APPROVALDATE='{stamp:yyyy.MM.dd}' and MOVEMENTVOUCHERTYPE=0");
                    foreach (DataRow transaction in transactions.Rows)
                    {
                        StringBuilder myLog = new StringBuilder();
                        string trId = transaction[0].ToString();
                        long parsedTrId = long.Parse(trId);
                        myLog.AppendLine($"Транзакция {trId}");
                        DataRow[] transactonContent = closedArticles.Select($"TransactionNumber='{trId}'");

                        var existMovement = c.MaconomyMovementTransactions.FirstOrDefault(x =>
                            x.TransactionId == parsedTrId && x.IsSuccess == true);
                        if (existMovement != null)
                        {
                            myLog.AppendLine($"Уже проведена {existMovement.Stamp}, лог: {existMovement.LogFile}");
                            continue;
                        }
                        
                        MaconomyMovementTransaction mmt = new();
                        using (var baseTransaction = c.Database.BeginTransaction())
                        {
                            myLog.AppendLine($"Создана внутренняя транзакция: {baseTransaction.TransactionId}");
                            bool isOk = true;
                            try
                            {
                                WorkManagerFacade wmf = new WorkManagerFacade(c);

                                foreach (DataRow closedArticle in transactonContent)
                                {
                                    long orderNumber =
                                        long.Parse(closedArticle["ProductionTransactionNumber"].ToString());
                                    string article = closedArticle["ItemNumber"].ToString();
                                    int count = int.Parse(closedArticle["NumberOf"].ToString());

                                    var postsWorks = actualWorks
                                        .Where(x => x.OrderNumber == orderNumber && x.Article == article)
                                        .ToList()
                                        .OrderBy(x => x.Post.ProductOrder).GroupBy(x => x.PostId);
                                    if (postsWorks.Count() > 0)
                                    {
                                        myLog.AppendLine(
                                            $"Транзакция № {closedArticle["TransactionNumber"]} для заказа {closedArticle["ProductionTransactionNumber"]}");
                                        myLog.AppendLine(
                                            $"Артикул {closedArticle["ItemNumber"]} количество {closedArticle["NumberOf"]}");
                                    }

                                    var postQueue = postsWorks.Select(x => x.Key).ToList();

                                    foreach (var postWorks in postsWorks)
                                    {
                                        string nextPost = "";
                                        int current = postQueue.IndexOf(postWorks.Key);
                                        if (current == postQueue.Count - 1)
                                        {
                                            nextPost = Constants.Work.EndPosts.TotalEnd;
                                        }
                                        else
                                        {
                                            nextPost = postQueue[current + 1];
                                        }

                                        myLog.AppendLine($"Участок: {postWorks.Key} (следующий участок: {nextPost})");


                                        if (postWorks.Count() == 1)
                                        {
                                            //значит работа одна

                                            var singleWork = postWorks.First();
                                            if (singleWork.Count == count)
                                            {
                                                myLog.AppendLine(
                                                    $"Идеальный случай, работа одна и количество совпадает. Закрываем");
                                                //идеальный случай
                                                //close work
                                                myLog.AppendLine(CloseWork(c, singleWork, nextPost));
                                            }
                                            else
                                            {
                                                if (singleWork.Count > count)
                                                {
                                                    //спишем часть
                                                    myLog.AppendLine(
                                                        $"Сдана меньшая часть, делим работы и закрываем сданную часть. Было {singleWork.Count}, отделяем {count}");

                                                    //split && close
                                                    var splitResult = wmf.SplitWork(singleWork, count);
                                                    if (splitResult.Count > 1)
                                                    {
                                                        for (int loop = 1; loop <= splitResult.Count; loop += 2)
                                                        {
                                                            myLog.AppendLine(CloseWork(c, splitResult[loop], nextPost));
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    myLog.AppendLine(
                                                        $"Сдается большая часть. Косяк! закроем все что есть");
                                                    //по идее быть такого не должно

                                                    //close
                                                    myLog.AppendLine(CloseWork(c, singleWork, nextPost));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //понять это разделенная работа или есть несколько заказов на произв.
                                            var productionArticleRows =
                                                productionLines.Select(
                                                    $"ItemNumber='{article}' and TransactionNumber='{orderNumber}'"); // количество заказов на произв.
                                            var groupedByLines = postWorks.GroupBy(x => x.OrderLineNumber);
                                            bool isSplitted = groupedByLines.Count() > 1;
                                            bool isMultiOrder = productionArticleRows.Length > 1;
                                            myLog.AppendLine(
                                                $"Несколько работ {postWorks.Count()}. Разделена {isSplitted}, несколько строк {isMultiOrder}");

                                            //поищем работу с точным совпадением по количеству, если такая есть закроем ее.
                                            //сли нет, поочередно будем закрывать работы с наименьшим количеством, пока не доберем нужно количество

                                            var exactWork = postWorks.FirstOrDefault(x => x.Count == count);
                                            if (exactWork != null)
                                            {
                                                myLog.AppendLine(
                                                    $"Надена работа с точным совпадением по количеству: {exactWork.Id}");
                                                //close 
                                                myLog.AppendLine(CloseWork(c, exactWork, nextPost));
                                            }
                                            else
                                            {
                                                int closedCount = 0;
                                                int totalWorkCount = postWorks.Sum(x => x.Count);
                                                if (count > totalWorkCount)
                                                {
                                                    myLog.AppendLine(
                                                        $"Количество из транзакции {count} превышает количество в работах {totalWorkCount}, косяк. Уменьшено ");
                                                    count = totalWorkCount;
                                                }

                                                int loop = 0;
                                                while (closedCount != count)
                                                {
                                                    myLog.AppendLine(
                                                        $"Начинаем списывать. Текущая итерация {loop}, сейчас списано {closedCount}");
                                                    var currentWork = postWorks.Skip(loop).FirstOrDefault();
                                                    if (currentWork == null)
                                                    {
                                                        myLog.AppendLine($"Не найдена работа чтобы списать, косяк");
                                                        break;
                                                    }

                                                    int takedCount = 0;
                                                    if (currentWork.Count > count - closedCount)
                                                    {
                                                        //частично списать
                                                        //split && close
                                                        takedCount = count - closedCount;

                                                        myLog.AppendLine(
                                                            $"Работа ({currentWork.Id}) {currentWork.OrderNumber}-{currentWork.OrderLineNumber} {currentWork.Article}x{currentWork.Count}. Будет разделена, отделим {takedCount}");
                                                        var splitResult = wmf.SplitWork(currentWork, takedCount);
                                                        if (splitResult.Count > 1)
                                                        {
                                                            for (int loopClose = 1;
                                                                 loopClose <= splitResult.Count;
                                                                 loopClose += 2)
                                                            {
                                                                myLog.AppendLine(CloseWork(c, splitResult[loopClose],
                                                                    nextPost));
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //списать все в работе
                                                        //close
                                                        takedCount = currentWork.Count;
                                                        myLog.AppendLine(
                                                            $"Работа ({currentWork.Id}) {currentWork.OrderNumber}-{currentWork.OrderLineNumber} {currentWork.Article}x{currentWork.Count}. Закроем {takedCount}");
                                                        myLog.AppendLine(CloseWork(c, currentWork, nextPost));
                                                    }

                                                    loop++;
                                                    closedCount += takedCount;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            catch (Exception exc)
                            {
                                myLog.AppendLine($"Отказ: {exc.Message}");
                                baseTransaction.Rollback();
                                isOk = false;
                            }
                            
                            sb.AppendLine(myLog.ToString());
                            if (isOk)
                            {
                                baseTransaction.Commit();
                            }
                           
                            mmt.TransactionId = parsedTrId;
                            mmt.IsSuccess = isOk;
                            mmt.Stamp = DateTime.Now;
                            string directory = Path.Combine(Environment.CurrentDirectory,"clean-log");
                            Directory.CreateDirectory(directory);
                            string fileName = trId+"-"+baseTransaction.TransactionId+".log";
                            using (StreamWriter sw = new StreamWriter(Path.Combine(directory, fileName)))
                            {
                                sw.WriteLine(myLog);
                            }

                            mmt.LogFile = Path.Combine(directory, fileName);
                            c.MaconomyMovementTransactions.Add(mmt);
                            


                        }

                        c.MaconomyMovementTransactions.Add(mmt);
                        c.SaveChanges();
                    }
                }
            }

            return sb.ToString();
        }
    }
}