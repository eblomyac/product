using System.Data;
using System.Dynamic;
using System.Text;
using ClosedXML.Excel;
using KSK_LIB.DataStructure.MQRequest;
using KSK_LIB.Excel;
using KSK_LIB.Maconomy;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class ReportManager
{
    public async Task AdditioncalCostReportPeriodMail(DateTime from, DateTime to, string accName)
    {
        DateTime d = from;
        DataTable fullTable = null;

        while (d.Date != to.Date)
        {
            var report = AdditionalCostReport(d);
            if (fullTable == null)
            {
                fullTable = report;
            }
            else
            {
                fullTable.Merge(report);
            }
            d = d.AddDays(1);
        }

        fullTable.TableName = "additional-cost-report";
        ExcelExporter ee = new ExcelExporter("report-additional-cost.xlsx");
        ee.ExportTable(fullTable);
        
        using (BaseContext c = new BaseContext(""))
        {
            var user = c.Users.First(x => x.AccName == accName);

            MailRequest mr = new MailRequest();
            mr.IsBodyHtml = true;
            mr.Bcc = new List<string>(){"po@ksk.ru"};
            mr.To = new List<string>() { user.Mail };
            mr.CopyTo = new List<string>();
            mr.From = "product-report@ksk.ru";
            mr.Subject = $"Отчет о доп. работах";
            mr.Body = "<body>Отчет во вложении</body>";
            mr.MailAttachments = new List<MailAttachment>()
            {
                new MailAttachment("report-additional-cost.xlsx")
            };
            await EmailNotificatorSingleton.Instance.Send(mr);  
        }
    } 
    public DataTable AdditionalCostReport( DateTime stamp)
    {
        using (BaseContext c = new BaseContext())
        {
            DateTime minVal = new DateTime(stamp.Year, stamp.Month, stamp.Day, 10,30,0);
            DateTime maxVal = minVal.AddDays(1).AddSeconds(-1);
            var records = c.WorkStatusLogs.AsNoTracking()
                .Where(x => x.Stamp>=minVal && x.Stamp<=maxVal && x.NewStatus== WorkStatus.ended).ToList();
            
            var worksIds = records.Where(x => x.WorkId > 0).Select(x => x.WorkId).Distinct().ToList();
            var works = c.Works.AsNoTracking().Include(x => x.AdditionalCosts).ThenInclude(x=>x.AdditionalCostTemplate).Where(x => worksIds.Contains(x.Id))
                .ToList();

            DataTable t = new DataTable();
            t.Columns.Add("Дата (отчет)", typeof(DateTime));
            t.Columns.Add("Дата (создания)", typeof(DateTime));
            t.Columns.Add("Дата (завершения)", typeof(DateTime));

            t.Columns.Add("Тип доп. работы");
            t.Columns.Add("Артикул");
            t.Columns.Add("Заказ", typeof(long));
            t.Columns.Add("Номер строки", typeof(int));
            t.Columns.Add("Участок");
            t.Columns.Add("Линия пр-ва");
            t.Columns.Add("Общее назанчение работы");
            t.Columns.Add("Описание");
            t.Columns.Add("Норматив", typeof(decimal));
            
            foreach (var w in works)
            {
                string type = w.OrderNumber == 100 ? "Общая" : "Для артикула";
                string article = w.Article;
                long orderNumber = w.OrderNumber;
                int orderLineNumber = w.OrderLineNumber;
                string post = w.PostId;
                string prodLine = w.ProductLineId;
                
                foreach (var ac in w.AdditionalCosts)
                {
                    string acType = ac.AdditionalCostTemplate.Name;
                    string description = ac.Description;
                    decimal cost = ac.Cost;

                    DataRow r = t.NewRow();
                    r[0] = stamp;
                    r[2] = records.LastOrDefault(x=>x.WorkId==w.Id)!=null?records.Last(x=>x.WorkId==w.Id).Stamp:stamp;
                    r[1] = w.CreatedStamp;
                    r[3] = type;
                    r[4] = w.OrderNumber!=100?article:"Нет";
                    r[5] = w.OrderNumber!=100? orderNumber:0;
                    r[6] = w.OrderNumber!=100?orderLineNumber:0;
                    r[7] = post;
                    r[8] = prodLine;
                    r[9] = acType;
                    r[10] = description;
                    r[11] = cost;

                    t.Rows.Add(r);
                }
                
                    
                
            }

            return t;

        }
    }
    public async Task<List<dynamic>> PeriodReport(DateTime from, DateTime? to, bool moveDay)
    {
        List<dynamic> result = new List<object>();

    
        DateTime date = from;
        DateTime end = to.HasValue ? to.Value : DateTime.Today;

        List<string> lines = new List<string>();
        using (BaseContext c = new BaseContext("system"))
        {
            lines = c.ProductionLines.Select(x => x.Id).Distinct().ToList();
        }

        while (date.Date <= end.Date)

        {
            foreach (var productionLine in lines)
            {
                bool isOk = true;
                int tryCount = 0;
                do
                {
                    tryCount++;
                    try
                    {
                        dynamic dateRep = DailyReport(date, productionLine, moveDay);
                        result.Add(dateRep);
                        isOk = true;
                    }
                    catch (Exception exc)
                    {
                        if (tryCount > 1)
                        {
                            await EmailNotificatorSingleton.Instance.Send(new MailRequest()
                            {
                                Bcc = new List<string>(),
                                Body = $"Period report error: {exc.Message} @ try :{tryCount} , day{date:dd.MM.yyyy}",
                                From = "product@ksk.ru",
                                CopyTo = new List<string>(),
                                IsBodyHtml = false,
                                MailAttachments = new List<MailAttachment>(),
                                Subject = "PERIOD REPORT ERROR",
                                To = new List<string>() { "po@ksk.ru" }
                            });
                        }
                        isOk = false;
                    }

                    if (tryCount > 5)
                    {
                        throw new Exception("too many tries");
                    }
                } while (!isOk);



            }
            date=date.AddDays(1);
        }
        
        return result;
    }

    public dynamic DailyReport(DateTime stamp, string productionLine, bool moveDay)
    {
        using (BaseContext bc = new BaseContext(""))
        {
            DailySourceManager dsm = new DailySourceManager();

            dynamic result = new ExpandoObject();
            result.Date = stamp;
            result.ProductionLine = productionLine;
            List<string> operators = bc.Roles.AsNoTracking().Where(x => x.Type == RoleType.Operator)
                .Select(x => x.UserAccName).Distinct().ToList();
            List<string> masters = bc.Roles.AsNoTracking().Where(x => x.Type == RoleType.PostMaster).ToList()
                .Where(x => x.MasterPosts.Count > 0)
                .Select(x => x.UserAccName).Distinct().ToList();
            if (!masters.Contains("system"))
            {
                masters.Add("system");
            }
            
             DateTime minVal = new DateTime(stamp.Year, stamp.Month, stamp.Day, 00, 00, 00);
             if (moveDay)
             {
                 minVal = new DateTime(stamp.Year, stamp.Month, stamp.Day, 10, 30, 00);
                 
             }
             DateTime maxVal = minVal.AddDays(1).AddSeconds(-1);
           // DateTime minVal = new DateTime(stamp.Year, stamp.Month, stamp.Day, 0, 0, 00);
           // DateTime maxVal = new DateTime(stamp.Year, stamp.Month, stamp.Day, 0, 0, 00).AddDays(1);
             
            
            var records = bc.WorkStatusLogs.AsNoTracking()
                .Where(x => x.Stamp>=minVal && x.Stamp<=maxVal && x.ProductionLineId == productionLine).ToList();

            var issues = bc.WorkIssueLogs.AsNoTracking().Where(x =>
                x.Start.Date == stamp.Date || (x.End != null && x.End.Value.Date == stamp.Date)).ToList();
            //убрать логи событий по фильтру произв. линии
            var sourceIssuesIds = issues.Select(x => x.SourceIssueId).Distinct().ToList();
            var filteredIssuesIds = bc.Issues.AsNoTracking()
                .Where(x => x.Work.ProductLineId == productionLine && sourceIssuesIds.Contains(x.Id)).Select(x => x.Id)
                .Distinct().ToList();

            issues = issues.Where(x => filteredIssuesIds.Contains(x.Id)).ToList();

            //запущенные работы оператором
            var startedWork = records.Where(x => x.Stamp >= minVal && x.Stamp<=maxVal &&
                                                 x.NewStatus == WorkStatus.hidden &&
                                                 x.PrevStatus == WorkStatus.unkown &&
                                                 operators.Contains(x.EditedBy));
            var startedOrders = startedWork.Select(x => x.OrderNumber).Distinct().ToList();
            
            var worksIds = records.Where(x => x.WorkId > 0).Select(x => x.WorkId).Distinct().ToList();
            var works = bc.Works.AsNoTracking().Include(x => x.AdditionalCosts).Where(x => worksIds.Contains(x.Id))
                .ToList();
            var addCost = bc.AdditionalCosts.Where(x => worksIds.Contains(x.WorkId)).ToList();

            WorkTemplateLoader wtl = new WorkTemplateLoader();
            List<WorkCreateTemplate> templates = wtl.LoadOnlyCrp(works.Select(x => x.Article).Distinct().ToList());
            
            DataTable macInfo = null;
            DataTable macResult = null;
            using (MaconomyBase mb = new MaconomyBase())
            {
                if (works.Count() > 0)
                {
                    string orders = string.Join(',', works.Select(x => x.OrderNumber).Distinct().Select(x => $"'{x}'"));
                    macInfo =
                        mb.getTableFromDB(
                            $"SELECT ProductionLine.TransactionNumber, LINENUMBER, ITEMNUMBER, ENTRYTEXT, PRODUCTIONDATE, FINISHEDITEMLOCATION, NUMBEROF as numberof from ProductionLine left join ProductionVoucher on ProductionLine.TransactionNumber = ProductionVoucher.TransactionNumber where ProductionLine.TransactionNumber in ({orders}) ");



                }

                macResult = mb.getTableFromDB(
                    $"SELECT SUM(NUMBEROF) FROM KSK.A___ITEMMOVEMENTSVIE WHERE APPROVALDATE='{stamp:yyyy.MM.dd}' AND ITEMNUMBER='{productionLine}' AND MOVEMENTVOUCHERTYPE='1'");
            }

            // var endedWorks = works.Where(x => x.Status == WorkStatus.ended).ToList();
            // var EndedArticles = records.Where(x=>x.)
            int articleEndedWorkCount = 0;
            decimal articleEndedGoodsCount = 0;

            

            //сданные работы
            var endedWorks = records.Where(x => x.NewStatus == WorkStatus.ended && masters.Contains(x.EditedBy) && x.WorkId!=0);

            //события-созданы 
            var issueStarted = issues.Where(x => x.Start.Date == stamp);
            //события-завершены
            var issuesEnded = issues.Where(x => x.End != null && x.End.Value.Date == stamp);
            decimal totalEndCost = 0;
            decimal totalEndCount = 0;
            decimal productionEnd = 0;
            decimal totalAdditionalCosts = 0;

            result.MaconomyClosed = 0;
            if (macResult != null)
            {
                result.MaconomyClosed = macResult.Rows[0][0];
            }

            result.Posts = new List<object>();

            
            
            foreach (var bcPost in bc.Posts.OrderBy(x => x.ProductOrder).AsNoTracking().ToList())
            {
                var dses = dsm.DateValue(bcPost.Name, stamp);
                dynamic postStat = new ExpandoObject();
                postStat.DailySource = dses.FirstOrDefault(x => x.ProductLineId == productionLine).Value;
                postStat.PostName = bcPost.Name;
                var postEndedWorks = endedWorks.Where(x => x.PostId == bcPost.Name).ToList();
                // var worksIds = postEndedWorks.Select(x => x.WorkId).ToList();
             //   var worksPost = works.Where(x => x.PostId == bcPost.Name && x.Status == WorkStatus.ended).ToList();
                postStat.EndedWorkCount = postEndedWorks.Count;
                postStat.EndedCost = postEndedWorks.Sum(x => x.TotalCost);
                postStat.EndedCount = postEndedWorks.Sum(x => x.Count);
                postStat.AdditionalCost = addCost.Where(x=>postEndedWorks.Select(z=>z.WorkId).Contains(x.WorkId))
                    .Sum(z=>z.Cost);
                postStat.Works = new List<object>();
                totalAdditionalCosts += postStat.AdditionalCost;
                foreach (var wp in postEndedWorks)
                {
                    dynamic w = new ExpandoObject();
                    w.OrderNumber = wp.OrderNumber;
                    w.OrderLineNumber = wp.OrderLineNumber;
                    w.Article = wp.Article;
                    w.Count = wp.Count;
                    w.Cost = wp.TotalCost;
                    w.AdditionalCost = addCost.Where(x=>x.WorkId==wp.WorkId).Sum(z=>z.Cost);
                  
                    if (macInfo != null && wp.OrderNumber!=100)
                    {
                        
                        DataRow[] thisOrderRows = macInfo.Select($"TransactionNumber='{wp.OrderNumber}'");
                        int maxLineNumber = thisOrderRows.Length;

                        DataRow[] thisOrderAndArticleRows =
                            macInfo.Select(
                                $"TransactionNumber='{wp.OrderNumber}' and LineNumber='{wp.OrderLineNumber}'");
                        w.MaxLineNumber = maxLineNumber;
                        if (thisOrderAndArticleRows.Length > 0)
                        {
                            w.TotalCount = thisOrderAndArticleRows[0]["numberof"];    
                        }
                        else
                        {
                            thisOrderAndArticleRows =
                                macInfo.Select(
                                    $"TransactionNumber='{wp.OrderNumber}' and  ITEMNUMBER='{wp.Article}'");
                            if (thisOrderAndArticleRows.Length == 0)
                            {
                                w.TotalCount = 0;    
                            }
                            else
                            {
                                w.TotalCount = thisOrderAndArticleRows[0]["numberof"];
                            }
                            
                            
                                //  w.MaxLineNumber = 0;
                        }
                        
                    }
                    else
                    {
                        w.MaxLineNumber = 0;
                        w.TotalCount = 0;

                    }

                    w.TotalCost = w.TotalCount * templates.ArticleSingleCost(wp.Article);
                    w.TotalCostCompleted = wp.Count * templates.ArticleSingleCost(wp.Article);
                    


                    postStat.Works.Add(w);

                }

                var articleEnded =
                    postEndedWorks.Where(x => x.MovedTo != null && x.MovedTo == Constants.Work.EndPosts.TotalEnd);
                articleEndedWorkCount += articleEnded.Count();
                articleEndedGoodsCount += articleEnded.Sum(x => x.Count.Value);
                productionEnd += articleEnded.Sum(x => x.TotalCost);


                totalEndCost += postStat.EndedCost;
                totalEndCount += postEndedWorks.Count();

                result.Posts.Add(postStat);
            }

            result.AdditionalCost = totalAdditionalCosts;
            result.Date = stamp.Date;
            result.ProductionEnd = productionEnd;
            result.StartedWorksCount = startedWork.Count();
            result.StartedOrderCount = startedOrders.Count();
            result.StartedIssuesCount = issueStarted.Count();
            result.EndedIssuesCount = issuesEnded.Count();
            result.EndedWorksCount = totalEndCount;
            result.EndedCost = totalEndCost;
            result.StartedCost = startedWork.Select(x => templates.ArticleSingleCost(x.Article)*x.Count);  
            result.ArticleEndCount = articleEndedWorkCount;
            result.ArticleEndGoodsCount = articleEndedGoodsCount;
            return result;
        }
    }

    public dynamic ArticleReportSingelDay(DateTime d, bool moveDay)
    {
        DateTime minVal = new DateTime(d.Year, d.Month, d.Day, 0, 0, 00);
        DateTime maxVal = minVal.AddDays(1);
        if (moveDay)
        {
            minVal = new DateTime(d.Year, d.Month, d.Day, 10, 30, 0);
            maxVal = minVal.AddDays(1).AddSeconds(-1);
        }

        dynamic result = new ExpandoObject();
        result.DateStamp = d;
        
        using (BaseContext c = new BaseContext())
        {
            var dayLog = c.WorkStatusLogs.AsNoTracking().Where(x => x.Stamp >= minVal && x.Stamp <= maxVal && x.OrderNumber!=100).ToList();
            var endedDayLog = dayLog.Where(x => x.MovedTo == Constants.Work.EndPosts.TotalEnd).OrderBy(x=>x.OrderNumber).ThenBy(x=>x.OrderLineNumber);

            var endedArticles =  endedDayLog.Select(x => x.Article).Distinct().ToList();
            WorkTemplateLoader wtl = new WorkTemplateLoader();
            
            var templates =  wtl.LoadOnlyCrp(endedArticles);

            var posts = c.Posts.AsNoTracking().Include(x => x.PostCreationKeys).OrderBy(x=>x.ProductOrder);

            DataTable t = new DataTable();
            t.Columns.Add("Дата",typeof(DateTime));
            t.Columns.Add("Артикул");
            t.Columns.Add("Произв. линия");
            t.Columns.Add("Завершенное количество", typeof(decimal));
            foreach (var p in posts)
            {
                t.Columns.Add(p.Name +"_CRP",typeof(decimal));
                t.Columns.Add(p.Name +"_DP",typeof(decimal));
            }

            t.Columns.Add("Сумма CRP",typeof(decimal));
            t.Columns.Add("Сумма DP",typeof(decimal));

            foreach (var log in endedDayLog)
            {
                var allArtWorks = c.Works.AsNoTracking().Where(x =>
                    x.OrderNumber == log.OrderNumber && x.OrderLineNumber == x.OrderLineNumber).ToList();

                DataRow r = t.NewRow();
                r[0] = d;
                r[1] = log.Article;
                r[2] = log.ProductionLineId;
                r[3] = log.Count;


                decimal sCrp = 0;
                decimal sDp = 0;
                foreach (var p in posts)
                {
                    
                    decimal crpCost =templates.ArticleSingleCost(log.Article, p.PostCreationKeys.Select(x => x.Key).ToList()) * log.Count.Value;
                    sCrp += crpCost;
                    r[p.Name + "_CRP"] = crpCost;
                        
                    
                    var w = allArtWorks.Where(x => x.PostId == p.Name && x.Status == WorkStatus.ended).ToList().Count>0;
                    if (w)
                    {
                        r[p.Name + "_DP"] = crpCost;
                        sDp += crpCost;
                    }
                    else
                    {
                        r[p.Name + "_DP"] = 0;
                    }
                    
                }

                r["Сумма CRP"] = sCrp;
                r["Сумма DP"] = sDp;
                
                t.Rows.Add(r);
            }

            result.Table = t;

        }

        return result;
    }

    public async Task ArticleReportMail(DateTime start, DateTime end, bool moveDay, string accName)
    {
        DateTime d = start;
        DataTable fullTable = null;

        while (d.Date != end.Date)
        {
            var report = ArticleReportSingelDay(d, moveDay);
            if (fullTable == null)
            {
                fullTable = report.Table;
            }
            else
            {
                fullTable.Merge(report.Table);
            }
            d = d.AddDays(1);
        }

        fullTable.TableName = "article-report";
        ExcelExporter ee = new ExcelExporter("report-article.xlsx");
        ee.ExportTable(fullTable);
        
        using (BaseContext c = new BaseContext(""))
        {
            var user = c.Users.First(x => x.AccName == accName);

            MailRequest mr = new MailRequest();
            mr.IsBodyHtml = true;
            mr.Bcc = new List<string>(){"po@ksk.ru"};
            mr.To = new List<string>() { user.Mail };
            mr.CopyTo = new List<string>();
            mr.From = "product-report@ksk.ru";
            mr.Subject = $"Поартикульный отчет производства за период";
            mr.Body = "<body>Отчет во вложении</body>";
            mr.MailAttachments = new List<MailAttachment>()
            {
                new MailAttachment("report-article.xlsx")
            };
            await EmailNotificatorSingleton.Instance.Send(mr);  
        }
    }
     
   

    public MailRequest PeriodReportToMail(List<dynamic> reports, string accName)
    {
        DataTable t = new DataTable();
        t.Columns.Add("Дата",typeof(DateTime));
        t.Columns.Add("Линия");
        t.Columns.Add("Изделий завершено (шт.)", typeof(decimal));
        t.Columns.Add("Сумма выполненного норматива по участкам (мин)",typeof(decimal));
        t.Columns.Add("Сумма норматива доп. работ по участкам (мин)",typeof(decimal));
        t.Columns.Add("Сумма норматива сданных на склад изделий (ДП) (мин)",typeof(decimal));
        t.Columns.Add("Сумма норматива принятых на склад изделий (Maconomy) (мин)",typeof(decimal));
        t.Columns.Add("Дневной ресурс (мин.)", typeof(decimal));
        

        t.TableName = "Общий";
        DataTable t2 = new DataTable();
        t2.Columns.Add("Дата",typeof(DateTime));
        t2.Columns.Add("Линия");
        t2.Columns.Add("Участок");
        t2.Columns.Add("Изделий передано (шт.)", typeof(decimal));
        t2.Columns.Add("Выполненный норматив (мин.)", typeof(decimal));
        t2.Columns.Add("Выполненный норматив доп. работ (мин.)", typeof(decimal));
        t2.Columns.Add("Дневной ресурс (мин.)", typeof(decimal));
        t2.Columns.Add("НОРМАТИВ(TEST)", typeof(decimal));
        t2.Columns.Add("НОРМАТИВ2(TEST)", typeof(decimal));
        t2.TableName = "По участкам";

        DataTable t3 = new DataTable();
        t3.TableName = "По артикулам и заказам";
        t3.Columns.Add("Дата",typeof(DateTime));
        t3.Columns.Add("Линия");
        t3.Columns.Add("Участок");
        t3.Columns.Add("Заказ",typeof(long));
        t3.Columns.Add("Номер строки заказа",typeof(int));
        t3.Columns.Add("Всего строк в заказе",typeof(int));
        t3.Columns.Add("Артикул");
        t3.Columns.Add("Количество выполнено",typeof(int));
        t3.Columns.Add("Количество в строке заказа",typeof(int));
        t3.Columns.Add("Норматив партии выполнено",typeof(decimal));
        t3.Columns.Add("Норматив доп. работ", typeof(decimal));
        t3.Columns.Add("НОРМАТИВ(TEST)", typeof(decimal));
        t3.Columns.Add("НОРМАТИВ2(TEST)", typeof(decimal));
        
        foreach (var report in reports)
        {
            DataRow total = t.NewRow();

            total["Дата"] = report.Date;
            total["Линия"] = report.ProductionLine;
            total["Изделий завершено (шт.)"] = report.ArticleEndGoodsCount;
            total["Сумма выполненного норматива по участкам (мин)"] = report.EndedCost;
            total["Сумма норматива сданных на склад изделий (ДП) (мин)"] = report.ProductionEnd;
            total["Сумма норматива принятых на склад изделий (Maconomy) (мин)"] = report.MaconomyClosed;
            total["Сумма норматива доп. работ по участкам (мин)"] = report.AdditionalCost;


            decimal dailySource = 0;

            foreach (var postReport in report.Posts)
            {
                DataRow postRow = t2.NewRow();

                postRow["Дата"] = report.Date;
                postRow["Линия"] = report.ProductionLine;
                postRow["Участок"] = postReport.PostName;
                postRow["Изделий передано (шт.)"] = postReport.EndedCount;
                postRow["Выполненный норматив (мин.)"] = postReport.EndedCost;
                postRow["Дневной ресурс (мин.)"] = postReport.DailySource;
                postRow["Выполненный норматив доп. работ (мин.)"] = postReport.AdditionalCost;
               
                dailySource += postReport.DailySource;
                decimal totalCost = 0;
                decimal totalCostCompleted = 0;

                foreach (var postWork in postReport.Works)
                {
                    DataRow r = t3.NewRow();
                    r[0] = report.Date;
                    r[1] = report.ProductionLine;
                    r[2] = postReport.PostName;
                    r[3] = postWork.OrderNumber;
                    r[4] = postWork.OrderLineNumber;
                    r[5] = postWork.MaxLineNumber;
                    r[6] = postWork.Article;
                    r[7] = postWork.Count;
                    r[8] = postWork.TotalCount;
                    r[9] = postWork.Cost;
                    r[10] = postWork.AdditionalCost;
                    r[11] = postWork.TotalCost;
                    r[12] = postWork.TotalCostCompleted;
                    totalCost += postWork.TotalCost;
                    totalCostCompleted += postWork.TotalCostCompleted;
                    t3.Rows.Add(r);
                }

                postRow["НОРМАТИВ(TEST)"] = totalCost;
                postRow["НОРМАТИВ2(TEST)"] = totalCostCompleted;
                
                t2.Rows.Add(postRow);
            }

            total["Дневной ресурс (мин.)"] = dailySource;
            t.Rows.Add(total);
            
        }

        DataSet ds = new DataSet();
        ds.Tables.Add(t);
        ds.Tables.Add(t2);
        ds.Tables.Add(t3);
        ExcelExporter ee = new ExcelExporter("report.xlsx");
        ee.ExportSet(ds);

        using (BaseContext c = new BaseContext(""))
        {
            var user = c.Users.First(x => x.AccName == accName);

            MailRequest mr = new MailRequest();
            mr.IsBodyHtml = true;
            mr.Bcc = new List<string>(){"po@ksk.ru"};
            mr.To = new List<string>() { user.Mail };
            mr.CopyTo = new List<string>();
            mr.From = "product-report@ksk.ru";
            mr.Subject = $"Отчет производства за период";
            mr.Body = "<body>Отчет во вложении</body>";
            mr.MailAttachments = new List<MailAttachment>()
            {
                new MailAttachment("report.xlsx")
            };
            return mr;
        }
    }

    public DataTable DailyWorksTable(dynamic DailyReport)
    {
        DataTable t = new DataTable();
        t.Columns.Add("Дата",typeof(DateTime));
        t.Columns.Add("Пост");
        t.Columns.Add("Заказ",typeof(long));
        t.Columns.Add("Номер строки заказа",typeof(int));
        t.Columns.Add("Всего строк в заказе",typeof(int));
        t.Columns.Add("Артикул");
        t.Columns.Add("Количество выполнено",typeof(int));
        t.Columns.Add("Количество в строке заказа",typeof(int));
        t.Columns.Add("Норматив партии выполнено",typeof(decimal));
        t.Columns.Add("Норматив доп. работ", typeof(decimal));
        foreach (dynamic p in DailyReport.Posts)
        {
            foreach (var w in p.Works)
            {
                DataRow r = t.NewRow();
                r[0] = DailyReport.Date;
                r[1] = p.PostName;
                r[2] = w.OrderNumber;
                r[3] = w.OrderLineNumber;
                r[4] = w.MaxLineNumber;
                r[5] = w.Article;
                r[6] = w.Count;
                r[7] = w.TotalCount;
                r[8] = w.Cost;
                r[9] = w.AdditionalCost;
                
                t.Rows.Add(r);    
            }
            
        }

        return t;
    }
    public string ReportToHtml(dynamic DailyReport)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<span><strong>Производственная линия: {DailyReport.ProductionLine}</strong></span><br>");
        sb.AppendLine($"<span>Отчет от <strong>{DailyReport.Date:dd.MM.yyyy}</strong></span><br>");
        sb.AppendLine(
            $"<span>Запущено оператором <strong>{DailyReport.StartedWorksCount}</strong> заданий для <strong>{DailyReport.StartedOrderCount}</strong> заказов с суммарным нормативом <strong>{DailyReport.StartedCost}</strong></span><br>");
        sb.AppendLine($"<span>Cумма выполненного норматива всех участков: <strong>{DailyReport.EndedCost}</strong> (мин.)</span><br>");
        sb.AppendLine($"<span>Cумма норматива доп. работ всех участков: <strong>{DailyReport.AdditionalCost}</strong> (мин.)</span><br>");
        sb.AppendLine($"<span>Cумма норматива изделий сданных на склад по ДП: <strong>{DailyReport.ProductionEnd}</strong> (мин.)</span><br>");
        sb.AppendLine($"<span>Cумма норматива изделий принятых на склад по Maconomy: <strong>{DailyReport.MaconomyClosed}</strong> (мин.)</span><br>");
        sb.AppendLine($"<span>Количество созданных событий <strong>{DailyReport.StartedIssuesCount}</strong>, количество разрешенных событий <strong>{DailyReport.EndedIssuesCount}</strong></span><br>");
        sb.AppendLine($"<span>Количество завершенных изделий: <strong>{DailyReport.ArticleEndGoodsCount}</strong> (шт.)</span>");
        sb.AppendLine($"<br><table><tr>" +
                      $"<th>Участок</th>" +
                      $"<th>Количество сданных изделий (шт.)</th>" +
                      $"<th>Выполненный норматив (мин.)</th>" +
                      $"<th>Доп. работы норматив (мин.)</th>" +
                      $"<th>Дневной ресурс (мин.)</th>" +
                      $"</tr>");
       // decimal totalWorkEnd = 0;
        decimal totalCostEnd = 0;
        decimal totalOperation = 0;
        decimal totalAdditional = 0;
        decimal totalDaily = 0;
        
        foreach (dynamic p in DailyReport.Posts)
        {
            totalOperation += p.EndedCount;
            totalCostEnd += p.EndedCost;
            totalAdditional += p.AdditionalCost;
            totalDaily += p.DailySource;
           // totalWorkEnd += p.EndedWorkCount;
            sb.AppendLine($"<tr>" +
                          $"<td><strong>{p.PostName}</strong></td>" +
                         // $"<td style='text-align: center;'>{p.EndedWorkCount}</td>" +
                          $"<td style='text-align: center;'>{p.EndedCount}</td>" +
                          $"<td style='text-align: center;'>{p.EndedCost}</td>" +
                          $"<td style='text-align: center;'>{p.AdditionalCost}</td>" +
                          $"<td style='text-align: center;'>{p.DailySource}</td>" +
                          $"</tr>");
        }

        sb.AppendLine($"<tr><td><strong>Сумма</strong></td>" +
                      $"<td style='text-align: center;'><strong>{totalOperation}</strong></td>" +
                      $"<td style='text-align: center;'><strong>{totalCostEnd}</strong></td>"+
                      $"<td style='text-align: center;'><strong>{totalAdditional}</strong></td>"+
                      $"<td style='text-align: center;'><strong>{totalDaily}</strong></td>"
                      );
        sb.AppendLine("</table>");
        sb.AppendLine("<br><span></span>");
        return sb.ToString();

    }

    

    public List<MailRequest> DailyReportMail(DateTime stamp)
    {
        using (BaseContext bc = new BaseContext(""))
        {
            List<MailRequest> result = new List<MailRequest>();
            var prodLine = bc.ProductionLines.Select(x => x.Id).Distinct().ToList();
            foreach (var line in prodLine)
            {
                var report = DailyReport(stamp, line,true);
                ExcelExporter ee = new ExcelExporter("workRep.xlsx");
                ee.ExportTable(DailyWorksTable(report));
                
                MailRequest mr = new MailRequest();
                mr.Body = ReportToHtml(report);
                mr.From = "product-report@ksk.ru";
                mr.Subject = $"Отчет производства от {stamp:dd.MM.yyyy}; произв. линия: {line}";
                mr.IsBodyHtml = true;
                mr.MailAttachments = new List<MailAttachment>() { new MailAttachment("workRep.xlsx") };
#if DEBUG
                mr.To = new List<string>() {"po@ksk.ru"};
                mr.Bcc = new List<string>();
#endif
#if RELEASE
            mr.To = bc.Users.Select(x => x.Mail).ToList();//new List<string>() {"po@ksk.ru"};// 
          //  mr.Bcc = new List<string>() {"artur.vagapov@ksk.ru"};
#endif
#if DEBUG
                mr.Bcc = new List<string>();
#endif
               
result.Add(mr);
            }
            return result;
        }
        
        
    }
    public List<MailRequest> DailyReportMail2(DateTime stamp)
    {
        using (BaseContext bc = new BaseContext(""))
        {
            List<MailRequest> result = new List<MailRequest>();
            var prodLine = bc.ProductionLines.Select(x => x.Id).Distinct().ToList();
            foreach (var line in prodLine)
            {
                var report = DailyReport(stamp, line, true);
                ExcelExporter ee = new ExcelExporter("workRep.xlsx");
                ee.ExportTable(DailyWorksTable(report));
                
                MailRequest mr = new MailRequest();
                mr.Body = ReportToHtml(report);
                mr.From = "product-report@ksk.ru";
                mr.Subject = $"Отчет производства от {stamp:dd.MM.yyyy}; произв. линия: {line}";
                mr.IsBodyHtml = true;
                mr.MailAttachments = new List<MailAttachment>() { new MailAttachment("workRep.xlsx") };
#if DEBUG
                mr.To = new List<string>() {"po@ksk.ru"};
                mr.Bcc = new List<string>();
#endif
#if RELEASE
                mr.To = bc.Users.Select(x => x.Mail).ToList();//new List<string>() {"po@ksk.ru"};// 
                //  mr.Bcc = new List<string>() {"artur.vagapov@ksk.ru"};
#endif
#if DEBUG
                mr.Bcc = new List<string>();
#endif
               
                result.Add(mr);
            }
            return result;
        }
        
        
    }

    public DataTable PrintWorkList(List<long> ids)
    {
        using (BaseContext c = new BaseContext(""))
        {
            var works = c.Works.AsNoTracking().Include(x=>x.AdditionalCosts).Where(x => ids.Contains(x.Id)).ToList();
            DataTable dt = new DataTable();
            dt.Columns.Add("Заказ");
            dt.Columns.Add("Артикул");
            dt.Columns.Add("Количество");
            dt.Columns.Add("Комментарий");
            dt.Columns.Add("Производство");

            dt.Columns.Add("Норматив");
            dt.Columns.Add("Доп. норматив");
            
            dt.Columns.Add("Дата сдачи");
            dt.Columns.Add("Операции");
            dt.Columns.Add("Статус");

            foreach (var w in works)
            {
                DataRow r = dt.NewRow();
                r["Заказ"] = w.OrderNumber;
                r["Артикул"] = w.Article;
                r["Количество"] = w.Count;
                r["Норматив"] = w.TotalCost;
                r["Доп. норматив"] = w.AdditionalCosts.Sum(x=>x.Cost);
                r["Комментарий"] = w.Description;
                r["Производство"] = w.ProductLineId;
                r["Дата сдачи"] = w.DeadLine;
                r["Операции"] = w.CommentMap.Replace("\t","\r\n");
                r["Статус"] = w.StatusString;
                dt.Rows.Add(r);
            }

            
            
            return dt;

        }
    }
}