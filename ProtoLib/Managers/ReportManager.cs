using System.Data;
using System.Dynamic;
using System.Text;
using KSK_LIB.DataStructure.MQRequest;
using KSK_LIB.Excel;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class ReportManager
{
    public List<dynamic> PeriodReport(DateTime from, DateTime? to)
    {
        List<dynamic> result = new List<object>();

    
        DateTime date = from;
        DateTime end = to.HasValue ? to.Value : DateTime.Today;

        List<string> lines = new List<string>();
        using (BaseContext c = new BaseContext())
        {
            lines = c.ProductionLines.Select(x => x.Id).Distinct().ToList();
        }
        while (date.Date!=end.Date)
        {
            foreach (var productionLine in lines)
            {
                dynamic dateRep = DailyReportLineWithWork(date, productionLine);
                result.Add(dateRep);
                
            }
            date=date.AddDays(1);
        }
        
        return result;
    }
    public dynamic DailyReportLine(DateTime stamp, string productionLine){
        using (BaseContext bc = new BaseContext())
        {
            dynamic result = new ExpandoObject();
            result.Date = stamp;
            result.ProductionLine = productionLine;
            List<string> operators = bc.Roles.AsNoTracking().Where(x => x.Type == RoleType.Operator).Select(x => x.UserAccName).Distinct().ToList();
            List<string> masters = bc.Roles.AsNoTracking().Where(x => x.Type == RoleType.PostMaster).ToList().Where(x=>x.MasterPosts.Count>0)
                .Select(x => x.UserAccName).Distinct().ToList();
            var records = bc.WorkStatusLogs.AsNoTracking().Where(x => x.Stamp.Date == stamp.Date).ToList();
            
            //отфильтровать рекорды по работе - оставить только нужную произв. линию
            var workIds = records.Select(x => x.WorkId).Distinct().ToList();
            var lineWorksIds = bc.Works.AsNoTracking().Where(x => workIds.Contains(x.Id) && x.ProductLineId == productionLine).Select(x=>x.Id).Distinct().ToList();
            records = records.Where(x => lineWorksIds.Contains(x.WorkId)).ToList();
            
            var issues = bc.WorkIssueLogs.AsNoTracking().Where(x =>
                x.Start.Date == stamp.Date || (x.End != null && x.End.Value.Date == stamp.Date)).ToList();
            //убрать логи событий по фильтру произв. линии
            var sourceIssuesIds = issues.Select(x => x.SourceIssueId).Distinct().ToList();
            var filteredIssuesIds  = bc.Issues.AsNoTracking().Where(x => x.Work.ProductLineId == productionLine && sourceIssuesIds.Contains(x.Id)).Select(x=>x.Id).Distinct().ToList();

            issues = issues.Where(x => filteredIssuesIds.Contains(x.Id)).ToList();
            
            
            
            
            
            
            
            //запущенные работы оператором
            var startedWork = records.Where(x => x.Stamp.Date == stamp.Date &&
                x.NewStatus == WorkStatus.income && x.PrevStatus == WorkStatus.hidden &&
                operators.Contains(x.EditedBy));
            var startedOrders = startedWork.Select(x => x.OrderNumber).Distinct().ToList();
            var worksStartedIds = startedWork.Select(x => x.WorkId).ToList();
            var worksIds = records.Where(x => x.WorkId > 0).Select(x => x.WorkId).Distinct().ToList();
            var works = bc.Works.AsNoTracking().Where(x=>worksIds.Contains(x.Id)).ToList();
           // var endedWorks = works.Where(x => x.Status == WorkStatus.ended).ToList();
           // var EndedArticles = records.Where(x=>x.)
            int articleEndedWorkCount = 0;
            decimal articleEndedGoodsCount = 0;
        
           
          
            //сданные работы
            var endedWorks = records.Where(x => x.NewStatus == WorkStatus.ended && masters.Contains(x.EditedBy));
            
            //события-созданы 
            var issueStarted = issues.Where(x => x.Start.Date == stamp);
            //события-завершены
            var issuesEnded = issues.Where(x => x.End != null && x.End.Value.Date == stamp);
            decimal totalEndCost = 0;
            decimal totalEndCount = 0;
            decimal productionEnd = 0;
            
            result.Posts = new List<object>();
            foreach (var bcPost in bc.Posts.OrderBy(x=>x.ProductOrder).AsNoTracking().ToList())
            {
                dynamic postStat = new ExpandoObject();
                postStat.PostName = bcPost.Name;
                var postEndedWorks = endedWorks.Where(x => x.PostId == bcPost.Name);
               // var worksIds = postEndedWorks.Select(x => x.WorkId).ToList();
                var worksPost = works.Where(x => x.PostId == bcPost.Name && x.Status==WorkStatus.ended).ToList();
                postStat.EndedWorkCount = worksPost.Count;
                postStat.EndedCost = worksPost.Sum(x => x.SingleCost * x.Count);
                postStat.EndedCount = worksPost.Sum(x => x.Count);

                var articleEnded = worksPost.Where(x => x.MovedTo != null && x.MovedTo == Constants.Work.EndPosts.TotalEnd);
                articleEndedWorkCount += articleEnded.Count();
                articleEndedGoodsCount += articleEnded.Sum(x => x.Count);
                productionEnd += articleEnded.Sum(x => x.TotalCost);
               
                
                totalEndCost += postStat.EndedCost;
                totalEndCount += worksPost.Count();
                
                result.Posts.Add(postStat);
            }

            result.Date = stamp.Date;
            
            result.StartedWorksCount = startedWork.Count();
            result.StartedOrderCount = startedOrders.Count();
            result.StartedIssuesCount = issueStarted.Count();
            result.EndedIssuesCount = issuesEnded.Count();
            result.EndedWorksCount = totalEndCount;
            result.EndedCost = totalEndCost;
            result.StartedCost = bc.Works.Where(x => worksStartedIds.Contains(x.Id)).Sum(x=>x.SingleCost * x.Count);
            result.ArticleEndCount = articleEndedWorkCount;
            result.ArticleEndGoodsCount = articleEndedGoodsCount;
            return result;
        }
    }
     public dynamic DailyReportLineWithWork(DateTime stamp, string productionLine){
        using (BaseContext bc = new BaseContext())
        {
            DailySourceManager dsm = new DailySourceManager();
            
            dynamic result = new ExpandoObject();
            result.Date = stamp;
            result.ProductionLine = productionLine;
            List<string> operators = bc.Roles.AsNoTracking().Where(x => x.Type == RoleType.Operator).Select(x => x.UserAccName).Distinct().ToList();
            List<string> masters = bc.Roles.AsNoTracking().Where(x => x.Type == RoleType.PostMaster).ToList().Where(x=>x.MasterPosts.Count>0)
                .Select(x => x.UserAccName).Distinct().ToList();
            var records = bc.WorkStatusLogs.AsNoTracking().Where(x => x.Stamp.Date == stamp.Date).ToList();
            
            //отфильтровать рекорды по работе - оставить только нужную произв. линию
            var workIds = records.Select(x => x.WorkId).Distinct().ToList();
            var lineWorksIds = bc.Works.AsNoTracking().Where(x => workIds.Contains(x.Id) && x.ProductLineId == productionLine).Select(x=>x.Id).Distinct().ToList();
            records = records.Where(x => lineWorksIds.Contains(x.WorkId)).ToList();
            
            var issues = bc.WorkIssueLogs.AsNoTracking().Where(x =>
                x.Start.Date == stamp.Date || (x.End != null && x.End.Value.Date == stamp.Date)).ToList();
            //убрать логи событий по фильтру произв. линии
            var sourceIssuesIds = issues.Select(x => x.SourceIssueId).Distinct().ToList();
            var filteredIssuesIds  = bc.Issues.AsNoTracking().Where(x => x.Work.ProductLineId == productionLine && sourceIssuesIds.Contains(x.Id)).Select(x=>x.Id).Distinct().ToList();

            issues = issues.Where(x => filteredIssuesIds.Contains(x.Id)).ToList();
            
            
            
            
            
            
            
            //запущенные работы оператором
            var startedWork = records.Where(x => x.Stamp.Date == stamp.Date &&
                x.NewStatus == WorkStatus.income && x.PrevStatus == WorkStatus.hidden &&
                operators.Contains(x.EditedBy));
            var startedOrders = startedWork.Select(x => x.OrderNumber).Distinct().ToList();
            var worksStartedIds = startedWork.Select(x => x.WorkId).ToList();
            var worksIds = records.Where(x => x.WorkId > 0).Select(x => x.WorkId).Distinct().ToList();
            var works = bc.Works.AsNoTracking().Where(x=>worksIds.Contains(x.Id)).ToList();
           // var endedWorks = works.Where(x => x.Status == WorkStatus.ended).ToList();
           // var EndedArticles = records.Where(x=>x.)
            int articleEndedWorkCount = 0;
            decimal articleEndedGoodsCount = 0;
        
           
          
            //сданные работы
            var endedWorks = records.Where(x => x.NewStatus == WorkStatus.ended && masters.Contains(x.EditedBy));
            
            //события-созданы 
            var issueStarted = issues.Where(x => x.Start.Date == stamp);
            //события-завершены
            var issuesEnded = issues.Where(x => x.End != null && x.End.Value.Date == stamp);
            decimal totalEndCost = 0;
            decimal totalEndCount = 0;
            decimal productionEnd = 0;
            
            result.Posts = new List<object>();
            foreach (var bcPost in bc.Posts.OrderBy(x=>x.ProductOrder).AsNoTracking().ToList())
            {
                var dses = dsm.DateValue(bcPost.Name, stamp);
                dynamic postStat = new ExpandoObject();
                postStat.DailySource = dses.FirstOrDefault(x => x.ProductLineId == productionLine).Value;
                postStat.PostName = bcPost.Name;
                var postEndedWorks = endedWorks.Where(x => x.PostId == bcPost.Name);
               // var worksIds = postEndedWorks.Select(x => x.WorkId).ToList();
                var worksPost = works.Where(x => x.PostId == bcPost.Name && x.Status==WorkStatus.ended).ToList();
                postStat.EndedWorkCount = worksPost.Count;
                postStat.EndedCost = worksPost.Sum(x => x.SingleCost * x.Count);
                postStat.EndedCount = worksPost.Sum(x => x.Count);

                postStat.Works = new List<object>();
                
                foreach (var wp in worksPost)
                {
                    dynamic w = new ExpandoObject();
                    w.OrderNumber = wp.OrderNumber;
                    w.OrderLineNumber = wp.OrderLineNumber;
                    w.Article = wp.Article;
                    w.Count = wp.Count;
                    w.Cost = wp.TotalCost;
                    postStat.Works.Add(w);

                }
                
                var articleEnded = worksPost.Where(x => x.MovedTo != null && x.MovedTo == Constants.Work.EndPosts.TotalEnd);
                articleEndedWorkCount += articleEnded.Count();
                articleEndedGoodsCount += articleEnded.Sum(x => x.Count);
                productionEnd += articleEnded.Sum(x => x.TotalCost);
               
                
                totalEndCost += postStat.EndedCost;
                totalEndCount += worksPost.Count();
                
                result.Posts.Add(postStat);
            }

            result.Date = stamp.Date;
            
            result.StartedWorksCount = startedWork.Count();
            result.StartedOrderCount = startedOrders.Count();
            result.StartedIssuesCount = issueStarted.Count();
            result.EndedIssuesCount = issuesEnded.Count();
            result.EndedWorksCount = totalEndCount;
            result.EndedCost = totalEndCost;
            result.StartedCost = bc.Works.Where(x => worksStartedIds.Contains(x.Id)).Sum(x=>x.SingleCost * x.Count);
            result.ArticleEndCount = articleEndedWorkCount;
            result.ArticleEndGoodsCount = articleEndedGoodsCount;
            return result;
        }
    }
   

    public MailRequest PeriodReportToMail(List<dynamic> reports, string accName)
    {
        DataTable t = new DataTable();
        t.Columns.Add("Дата",typeof(DateTime));
        t.Columns.Add("Линия");
        t.Columns.Add("Изделий завершено (шт.)", typeof(decimal));
        t.Columns.Add("Выполненный норматив (мин.)",typeof(decimal));
        t.Columns.Add("Дневной ресурс (мин.)", typeof(decimal));

        t.TableName = "Общий";
        DataTable t2 = new DataTable();
        t2.Columns.Add("Дата",typeof(DateTime));
        t2.Columns.Add("Линия");
        t2.Columns.Add("Участок");
        t2.Columns.Add("Изделий передано (шт.)", typeof(decimal));
        t2.Columns.Add("Выполненный норматив (мин.)", typeof(decimal));
        t2.Columns.Add("Дневной ресурс (мин.)", typeof(decimal));
        t2.TableName = "По участкам";
        foreach (var report in reports)
        {
            DataRow total = t.NewRow();

            total["Дата"] = report.Date;
            total["Линия"] = report.ProductionLine;
            total["Изделий завершено (шт.)"] = report.ArticleEndGoodsCount;
            total["Выполненный норматив (мин.)"] = report.EndedCost;


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
                dailySource += postReport.DailySource;
                
                t2.Rows.Add(postRow);
            }

            total["Дневной ресурс (мин.)"] = dailySource;
            t.Rows.Add(total);
            
        }

        DataSet ds = new DataSet();
        ds.Tables.Add(t);
        ds.Tables.Add(t2);
        ExcelExporter ee = new ExcelExporter("report.xlsx");
        ee.ExportSet(ds);

        using (BaseContext c = new BaseContext())
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
        t.Columns.Add("Дата");
        t.Columns.Add("Пост");
        t.Columns.Add("Заказ",typeof(long));
        t.Columns.Add("Номер строки заказа",typeof(int));
        t.Columns.Add("Артикул");
        t.Columns.Add("Количество",typeof(int));
        t.Columns.Add("Норматив общий",typeof(decimal));
        foreach (dynamic p in DailyReport.Posts)
        {
            foreach (var w in p.Works)
            {
                DataRow r = t.NewRow();
                r[0] = DailyReport.Date;
                r[1] = p.PostName;
                r[2] = w.OrderNumber;
                r[3] = w.OrderLineNumber;
                r[4] = w.Article;
                r[5] = w.Count;
                r[6] = w.Cost;
                
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
        sb.AppendLine($"<span>Cумма выполненного норматива <strong>{DailyReport.EndedCost}</strong> (мин.)</span><br>");
        sb.AppendLine($"<span>Количество созданных событий <strong>{DailyReport.StartedIssuesCount}</strong>, количество разрешенных событий <strong>{DailyReport.EndedIssuesCount}</strong></span><br>");
        sb.AppendLine($"<span>Количество завершенных изделий: <strong>{DailyReport.ArticleEndGoodsCount}</strong> (шт.)</span>");
        sb.AppendLine($"<br><table><tr>" +
                      $"<th>Участок</th>" +
                      $"<th>Количество сданных изделий (шт.)</th>" +
                      $"<th>Выполненный норматив (мин.)</th>" +
                      $"<th>Дневной ресурс (мин.)</th>" +
                      $"</tr>");
       // decimal totalWorkEnd = 0;
        decimal totalCostEnd = 0;
        decimal totalOperation = 0;
        
        foreach (dynamic p in DailyReport.Posts)
        {
            totalOperation += p.EndedCount;
            totalCostEnd += p.EndedCost;
           // totalWorkEnd += p.EndedWorkCount;
            sb.AppendLine($"<tr>" +
                          $"<td><strong>{p.PostName}</strong></td>" +
                         // $"<td style='text-align: center;'>{p.EndedWorkCount}</td>" +
                          $"<td style='text-align: center;'>{p.EndedCount}</td>" +
                          $"<td style='text-align: center;'>{p.EndedCost}</td>" +
                          $"<td style='text-align: center;'>{p.DailySource}</td>" +
                          $"</tr>");
        }

        sb.AppendLine($"<tr><td><strong>Сумма</strong></td><td style='text-align: center;'><strong>{totalOperation}</strong></td><td style='text-align: center;'><strong>{totalCostEnd}</strong></td>");
        sb.AppendLine("</table>");
        sb.AppendLine("<br><span></span>");
        return sb.ToString();

    }

    

    public List<MailRequest> DailyReportMail(DateTime stamp)
    {
        using (BaseContext bc = new BaseContext())
        {
            List<MailRequest> result = new List<MailRequest>();
            var prodLine = bc.ProductionLines.Select(x => x.Id).Distinct().ToList();
            foreach (var line in prodLine)
            {
                var report = DailyReportLineWithWork(stamp, line);
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
        using (BaseContext c = new BaseContext())
        {
            var works = c.Works.Where(x => ids.Contains(x.Id)).ToList();
            DataTable dt = new DataTable();
            dt.Columns.Add("Заказ");
            dt.Columns.Add("Артикул");
            dt.Columns.Add("Количество");
            dt.Columns.Add("Комментарий");
            dt.Columns.Add("Производство");

            dt.Columns.Add("Норматив");
            
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