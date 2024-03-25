using System.Data;
using System.Dynamic;
using System.Text;
using KSK_LIB.DataStructure.MQRequest;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class ReportManager
{
    public dynamic DailyReport(DateTime stamp){
        using (BaseContext bc = new BaseContext())
        {
            dynamic result = new ExpandoObject();
            List<string> operators = bc.Roles.AsNoTracking().Where(x => x.Type == RoleType.Operator).Select(x => x.UserAccName).Distinct().ToList();
            List<string> masters = bc.Roles.AsNoTracking().Where(x => x.Type == RoleType.PostMaster).ToList().Where(x=>x.MasterPosts.Count>0)
                .Select(x => x.UserAccName).Distinct().ToList();
            var records = bc.WorkStatusLogs.AsNoTracking().Where(x => x.Stamp.Date == stamp.Date).ToList();
            var issues = bc.WorkIssueLogs.AsNoTracking().Where(x =>
                x.Start.Date == stamp.Date || (x.End != null && x.End.Value.Date == stamp.Date)).ToList();
            
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
            
            result.Posts = new List<object>();
            foreach (var bcPost in bc.Posts.AsNoTracking().ToList())
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

    public string ReportToHtml(dynamic DailyReport)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<span>Отчет от <strong>{DailyReport.Date:dd.MM.yyyy}</strong></span><br>");
        sb.AppendLine(
            $"<span>Запущено оператором <strong>{DailyReport.StartedWorksCount}</strong> заданий для <strong>{DailyReport.StartedOrderCount}</strong> заказов с суммарным нормативом <strong>{DailyReport.StartedCost}</strong></span><br>");
        sb.AppendLine($"<span>Общее количество завершенных заданий <strong>{DailyReport.EndedWorksCount}</strong>, суммарный норматив <strong>{DailyReport.EndedCost}</strong></span><br>");
        sb.AppendLine($"<span>Количество созданных событий <strong>{DailyReport.StartedIssuesCount}</strong>, количество разрешенных событий <strong>{DailyReport.EndedIssuesCount}</strong></span><br>");
        sb.AppendLine($"<span>Количество сданных изделий: <strong>{DailyReport.ArticleEndGoodsCount}</strong>, в <strong>{DailyReport.ArticleEndCount}</strong> заданиях</span><br");
        sb.AppendLine($"<br><table><tr>" +
                      $"<th>Участок</th>" +
                      $"<th>Количество завершенных заданий</th>" +
                      $"<th>Сумма норматива завершенных заданий</th>" +
                      $"<th>Количество выполненных операций</th>" +
                      $"</tr>");
        foreach (dynamic p in DailyReport.Posts)
        {
            sb.AppendLine($"<tr>" +
                          $"<td><strong>{p.PostName}</strong></td>" +
                          $"<td style='text-align: center;'>{p.EndedWorkCount}</td>" +
                          $"<td style='text-align: center;'>{p.EndedCost}</td>" +
                          $"<td style='text-align: center;'>{p.EndedCount}</td>" +
                          $"</tr>");
        }

        sb.AppendLine("</table>");
        sb.AppendLine("<br><span></span>");
        return sb.ToString();

    }

    public MailRequest DailyReportMail(DateTime stamp)
    {
        using (BaseContext bc = new BaseContext())
        {
            var report = DailyReport(stamp);
            MailRequest mr = new MailRequest();
            mr.Body = ReportToHtml(report);
            mr.From = "product-report@ksk.ru";
            mr.Subject = $"Отчет производства от {stamp:dd.MM.yyyy}";
            mr.IsBodyHtml = true;
            mr.To = bc.Users.Select(x => x.Mail).ToList();//new List<string>() {"po@ksk.ru"};// 
            mr.Bcc = new List<string>() {"artur.vagapov@ksk.ru"};
            #if DEBUG
                mr.Bcc = new List<string>();
            #endif
            return mr;

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
                r["Производство"] = w.ProductLine;
                r["Дата сдачи"] = w.DeadLine;
                r["Операции"] = w.CommentMap.Replace("\t","\r\n");
                r["Статус"] = w.StatusString;
                dt.Rows.Add(r);
            }

            
            
            return dt;

        }
    }
}