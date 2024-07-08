using System.Data;
using System.Diagnostics;
using KSK_LIB.Maconomy;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class MaintenanceManager
{
    public void FillWorkCostAndComment()
    {
        using (BaseContext c = new BaseContext())
        {
            DateTime d = DateTime.Today.AddDays(-1); 
            var works  = c.Works.Include(x=>x.Post).
                ThenInclude(x=>x.PostCreationKeys).Where(x => (x.CommentMap.Length <2 || x.SingleCost==0)&&x.CreatedStamp.Date > d.Date).ToList();
            var artilces = works.Select(x => x.Article).ToList();
            CrpManager crp = new CrpManager();
            var data = crp.LoadWorkData(artilces);
            foreach (var work in works)
            {
                var keys = work.Post.PostCreationKeys.Select(x=>x.Key).ToList();
                var workCreateTemplates =data.Where(x => keys.Contains(x.PostKey) && x.Article==work.Article).ToList();
                work.Comments = workCreateTemplates.SelectMany(x => x.Comment.Split('\r', StringSplitOptions.RemoveEmptyEntries)).ToList();
                work.SingleCost = workCreateTemplates.Sum(x => x.SingleCost);
            }

            c.SaveChanges();
        }
    }
    public void FillWorkLog()
    {
        using (BaseContext c = new BaseContext())
        {
        
            DateTime d = DateTime.Today.AddDays(-1);
            while (true)
            {
                var toFix = c.WorkStatusLogs.Where(x =>
                        (x.ProductionLineId == null || x.Count == null || x.SingleCost == null || x.MovedFrom == null || x.MovedTo==null ||
                         x.OrderLineNumber == null || x.SingleCost==0) && x.WorkId > 0 && x.Stamp.Date>d.Date)
                    .Take(500).ToList();
                if (toFix.Count == 0)
                {
                    break;
                }
                var workIds = toFix.Select(x => x.WorkId).Distinct().ToList();
                var works = c.Works.Where(x => workIds.Contains(x.Id)).ToList();
                var toDelete = new List<WorkStatusLog>();
                foreach (var log in toFix)
                {
                    var work = works.FirstOrDefault(x => x.Id == log.WorkId);
                    if (work == null)
                    {
                        toDelete.Add(log);
                        continue;
                    }

                    log.SingleCost = work.SingleCost;
                    log.Count = work.Count;
                    log.ProductionLineId = work.ProductLineId;
                    log.OrderLineNumber = work.OrderLineNumber;
                    log.MovedFrom = work.MovedFrom;
                    log.MovedTo = work.MovedTo;
                }

                c.WorkStatusLogs.RemoveRange(toDelete);

                c.SaveChanges();
            }
        }
    }

    public void CleanClosedPrediction()
    {
        using (BaseContext c = new BaseContext("maintenance"))
        {
            var orders = c.Works.Where(x => x.Status != WorkStatus.ended).Select(x => x.OrderNumber).Distinct().ToList();
            List<long> closedOrders = new List<long>();
            using (MaconomyBase mb = new MaconomyBase())
            {
                string orderQuery = string.Join(',', orders.Select(x => $"'{x}'"));
                var closed = mb.getTableFromDB($"SELECT TRANSACTIONNUMBER from ProductionVoucher where CLOSED='1' and  TRANSACTIONNUMBER in ({orderQuery})");
                foreach (DataRow row in closed.Rows)
                {
                    closedOrders.Add((long)row.Field<decimal>("TRANSACTIONNUMBER"));
                }
            }

            var toCloseWorks = c.Works.AsNoTracking()
                .Include(x=>x.Issues)
                .Include(x=>x.Post)
                .Where(x => closedOrders.Contains(x.OrderNumber) && x.Status != WorkStatus.ended)
                .OrderBy(x=>x.Post.ProductOrder).ToList();

            WorkStatusChanger wsc = new WorkStatusChanger();
            foreach (var work in toCloseWorks)
            {
              
                var nextWorkPost =
                    toCloseWorks.FirstOrDefault(x => x.Id != work.Id && x.Post.ProductOrder >= work.Post.ProductOrder && x.OrderNumber==work.OrderNumber && x.OrderLineNumber==work.OrderLineNumber)?.PostId;
                for (int loopStatus = (int)work.Status + 10;
                     loopStatus <= (int)WorkStatus.ended;
                     loopStatus += 10)
                {
                    if (nextWorkPost != null)
                    {
                        wsc.ChangeStatus(work.Id, (WorkStatus)loopStatus, "maintenance", nextWorkPost);
                    }
                    else
                    {
                        wsc.ChangeStatus(work.Id, (WorkStatus)loopStatus, "maintenance");    
                    }
                    
                }
                
            }
            
            Console.WriteLine(JsonConvert.SerializeObject(closedOrders));
            Console.WriteLine(JsonConvert.SerializeObject(toCloseWorks.Select(x=>x.Id)));
            
        }
    }
}