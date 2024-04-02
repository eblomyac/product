using System.Data;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using KSK_LIB.Maconomy;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class WorkCleaner
{

    public string Clean()
    {
        using (BaseContext bc = new BaseContext())
        {
            var actualWorks = bc.Works.AsNoTracking().Include(x=>x.Issues).ThenInclude(x=>x.Template).AsNoTracking()
                .Include(x=>x.Post).AsNoTracking().Where(x => x.Status != WorkStatus.ended).ToList();
            var actualOrders = actualWorks.Select(x=>x.OrderNumber).Distinct().ToList();
            string s = string.Join(',',actualOrders.Select(x => $"'{x}'"));
            StringBuilder sb = new StringBuilder();
            using (MaconomyBase mb = new MaconomyBase())
            {
                sb.AppendLine($"Количество не закрытых работ {actualWorks.Count}, в {actualOrders.Count} заказах");
               var t =  mb.getTableFromDB(
                   $"SELECT ProductionLine.TransactionNumber,ITEMNUMBER, ENTRYTEXT, PRODUCTIONDATE, FINISHEDITEMLOCATION, SUM(NUMBEROF) as numberof, SUM(ProductionLine.NUMBERENDREPORTED) as numberended  from" +
                   $" ProductionLine left join ProductionVoucher on ProductionLine.TransactionNumber = ProductionVoucher.TransactionNumber " +
                   $"where ProductionLine.TransactionNumber in ({s}) group By ProductionLine.TransactionNUmber, ItemNumber, ENTRYTEXT, FINISHEDITEMLOCATION, PRODUCTIONDATE");

               foreach (DataRow r in t.Rows)
               {
                   string article = r["ITEMNUMBER"].ToString();
                   long order = long.Parse(r["TransactionNumber"].ToString());


                   int numberEnded =int.Parse(r["numberended"].ToString());
                   int numberof = int.Parse(r["numberof"].ToString());

                   bool isTotalEnded = numberof == numberEnded;
                   bool isPartEnded = numberEnded != 0;
                   
                   
                   var rowWorks = actualWorks.Where(x => x.OrderNumber == order && x.Article == article).OrderBy(x=>x.Post.ProductOrder).ToList();
                   if (isPartEnded)
                   {
                       
                   }
                   if (isTotalEnded)
                   {
                       sb.AppendLine($"Обнаружено расхождение в артикуле {article}, заказ: {order}, произв. задание в макономи выполнено, а в ДП нет");
                       WorkStatusChanger wss = new WorkStatusChanger();
                       IssueManager im = new IssueManager();

                       for (int loop = 0; loop < rowWorks.Count; loop++)
                       {
                           var toEndWork = rowWorks[loop];
                           string sendTo = "";
                           
                           if (loop != rowWorks.Count-1)
                           {
                               sendTo = rowWorks[loop + 1].PostId;
                           }
                           else
                           {
                               sendTo = Constants.Work.EndPosts.TotalEnd;
                           }
                           
                           sb.AppendLine($"\t{toEndWork.PostId} не выполнено, утерянный норматив: {toEndWork.TotalCost}");
                           if (toEndWork.Issues.Where(x => x.Resolved.HasValue == false).Count()>0)
                           {
                               foreach (var issue in toEndWork.Issues.Where(x=>x.Resolved.HasValue==false))
                               {
                                   sb.AppendLine(
                                       $"\t\tСобытие {issue.Template.Name}: {issue.Description} было принудительно разрешено");
                                   im.ResolveIssue(issue.Id, "system");
                               }
                           }

                           for (int loopStatus = (int) toEndWork.Status+10; loopStatus <= (int) WorkStatus.ended; loopStatus += 10)
                           {
                               
                               wss.ChangeStatus(toEndWork.Id, (WorkStatus)loopStatus, "system",sendTo);
                               
                           }
                       }
                     
                   }
                   
               }
            }

            return sb.ToString();
        }
         
    }
}