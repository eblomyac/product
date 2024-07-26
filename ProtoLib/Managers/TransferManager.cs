using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class TransferFilter
{
    public DateTime? from { get; set; }
    public DateTime? to { get; set; }
    public string articleId { get; set; }
    public string fromPost { get; set; }
    public string toPost { get; set; }
    public long? orderNumber { get; set; }
}
public class TransferManager
{
    public string Print(long id)
    {
        using (BaseContext c = new BaseContext(""))
        {
            var t  = c.Transfers.Include(x=>x.Lines).FirstOrDefault(x => x.Id == id);
            if (t!=null)
            {
                TransferPrinter tp = new TransferPrinter(Path.Combine(Environment.CurrentDirectory,"pdf"));
                string s = tp.MakeTransferPdf(t);
                return Path.Combine(Environment.CurrentDirectory,"pdf",s);
            }

            return "";
        }
    }
    public Transfer Create(string postIdFrom, string postIdTo, List<Work> works, string accName)
    {
        
        Transfer t = new Transfer();
        t.CreatedStamp= DateTime.Now;
        t.Created = DateTime.Today;
        t.PostFromId = postIdFrom;
        t.PostToId= postIdTo;
        
        t.Lines = new System.Collections.Generic.List<TransferLine>();
        
        int seed = 0;
        using (BaseContext c = new BaseContext(""))
        {
            seed=c.Transfers.Count(x => x.Created == DateTime.Today && x.PostFromId == postIdFrom && x.PostToId == postIdTo)+1;
            string fromShortName = c.Posts.FirstOrDefault(x => x.Name == postIdFrom).TableName;
            string toShortName = c.Posts.FirstOrDefault(x => x.Name == postIdTo).TableName;
            t.CreatedBy= c.Users.FirstOrDefault(x => x.AccName == accName)?.Name;
            t.ClosedBy = "";
            t.PaperId = $"{fromShortName}-{toShortName}.{seed}.{DateTime.Today:ddMM}";
        }

        WorkManagerFacade wmf = new WorkManagerFacade(accName);
        
        
        foreach (var w in works)
        {
            if (wmf.MoveToPostRequest(w.Id, postIdTo, new List<string>(), ""))
            {
                TransferLine tl = new TransferLine();
                tl.OrderLineNumber = w.OrderLineNumber;
                tl.OrderNumber = w.OrderNumber;
                tl.Article = w.Article;
                tl.IsTransfered = false;
                tl.Count = w.Count;
                tl.Remark = "";
                tl.ProductionLine = w.ProductLineId;
                tl.TransferedCount = 0;
                tl.SourceWorkId = w.Id;
                tl.SourceWorkCost = w.TotalCost;
                tl.TargetWorkId = null;
                t.Lines.Add(tl);
            }
         
        }

        using (BaseContext c = new BaseContext(accName))
        {
            c.Transfers.Add(t);
            c.SaveChanges();
        }
        return t;
    }

    public void ApplyTransfer(Transfer t,string accName)
    {
        WorkManagerFacade wmf = new WorkManagerFacade(accName);
        
        using (BaseContext c = new BaseContext(accName))
        {
            WorkSaveManager wsm = new WorkSaveManager();
            WorkStatusChanger wss = new WorkStatusChanger();
            var works = c.Works.AsNoTracking().Where(x => x.Status == WorkStatus.income && x.PostId == t.PostToId).ToList();
            
            foreach (var notFullTransfer in t.Lines.Where(x=>x.TransferedCount!= x.Count))
            {
                if (notFullTransfer.TransferedCount != 0)
                {
                    //частично вернем работу
                    var toSplitWork = works.FirstOrDefault(x =>
                        x.PostId == t.PostToId && x.Article == notFullTransfer.Article &&
                        x.OrderNumber == notFullTransfer.OrderNumber &&
                        x.OrderLineNumber == notFullTransfer.OrderLineNumber);
                    if (toSplitWork != null)
                    {
                        var newWorks = wmf.SplitWork(toSplitWork,
                            notFullTransfer.Count - notFullTransfer.TransferedCount);
                        var myWork = newWorks[0]; //эту двинем дальше ниже
                        var returnWork = newWorks[1]; //эту надо вернуть в прогноз


                        wmf.MoveToPostRequest(returnWork.Id, t.PostFromId, new List<string>(),
                            notFullTransfer.Remark);

                        Debug.WriteLine(JsonConvert.SerializeObject(newWorks));
                    }
                    else
                    {
                        Debug.WriteLine("ApplyTansfer: work to split is null");
                    }
                }
                else
                {
                    //полностью вернуть работу
                    
                   
                    var currentWork =  works.First(x =>
                        x.PostId == t.PostToId && x.Article == notFullTransfer.Article &&
                        x.OrderNumber == notFullTransfer.OrderNumber &&
                        x.OrderLineNumber == notFullTransfer.OrderLineNumber);
                    wmf.MoveToPostRequest(currentWork.Id, t.PostFromId, new List<string>(),
                        notFullTransfer.Remark);
                   // wss.ChangeStatus(currentWork.Id, WorkStatus.hidden, accName);

                 //   wsm.SaveWorks(new List<Work>() { currentWork });
                }
            }
            works = c.Works.AsNoTracking().Include(x=>x.Issues).Where(x => x.Status == WorkStatus.income && x.PostId == t.PostToId).ToList();
            var transfer = c.Transfers.Include(x=>x.Lines).FirstOrDefault(x => x.Id == t.Id);
            List<Work> toPost = new List<Work>();
            IssueManager im = new IssueManager();
           
            foreach (var stl in t.Lines.Where(x=>x.TransferedCount!=0))
            {
                var existTl = transfer.Lines.FirstOrDefault(x => x.Id == stl.Id);
                existTl.IsTransfered = stl.IsTransfered;
                existTl.TransferedCount = stl.TransferedCount;
                existTl.Remark = stl.Remark;

                var work = works.FirstOrDefault(x =>
                    x.Article == stl.Article && x.PostId == t.PostToId && x.Status == WorkStatus.income &&
                    x.OrderNumber == stl.OrderNumber && x.Count == existTl.TransferedCount && x.OrderLineNumber == stl.OrderLineNumber );
                if (work != null)
                {
                    existTl.TargetWorkId = work.Id;
                    toPost.Add(work);
                    try
                    {
                        foreach (var issue in work.Issues.Where(z => z.Resolved == null))
                        {
                            im.ResolveIssue(issue.Id, accName);
                        }
                    }
                    catch (Exception exc)
                    {
                    }
                }
                else
                {
                    Debug.WriteLine("ApplyTansfer: work to assign is null");
                } 

            }

            
            wss.ChangeStatus(toPost, WorkStatus.waiting, accName);
            
            
            
         
            wsm.SaveWorks(toPost);
           

            transfer.ClosedStamp = DateTime.Now;
            transfer.Closed = DateTime.Today;
            transfer.ClosedBy = c.Users.FirstOrDefault(x => x.AccName == accName)?.Name;
            
            c.SaveChanges();
        }

       
        
    }

    public List<Transfer> ListIn(string postId){
        using(BaseContext c = new BaseContext(""))
        {
            var result  = c.Transfers.Include(x=>x.Lines).
                Where(x => x.Closed.HasValue==false && x.PostToId == postId).ToList().OrderByDescending(x=>x.Id).ToList();
            DateTime d = DateTime.Today.AddDays(-7);
            
            var closedLast = c.Transfers.Include(x => x.Lines)
                .Where(x => x.Closed.HasValue && x.CreatedStamp > d && x.PostToId==postId)
                .OrderByDescending(x=>x.CreatedStamp).Take(30).ToList();

            if (closedLast.Count > 0)
            {
                result.AddRange(closedLast);
            }
            return result;
        }
    } 
    public List<Transfer> ListOut(string postId){
        using(BaseContext c = new BaseContext(""))
        {
            var notClosed= c.Transfers.Include(x=>x.Lines)
                .Where(x => x.Closed.HasValue==false && x.PostFromId == postId).ToList().OrderByDescending(x=>x.CreatedStamp).ToList();
            DateTime d = DateTime.Today.AddDays(-7);
            var closedLast = c.Transfers.Include(x => x.Lines)
                .Where(x => x.Closed.HasValue && x.CreatedStamp > d && x.PostFromId==postId)
                .OrderByDescending(x=>x.CreatedStamp).Take(30).ToList();
            if (closedLast.Count > 0)
            {
                notClosed.AddRange(closedLast);
            }

            return notClosed;

        }
    }

    public async Task<List<Transfer>> List(int offset, TransferFilter tf)
    {
        
        

        using (BaseContext c = new BaseContext())
        {
            var q = c.Transfers.Include(x=>x.Lines).Where(x=>x.Id>0).OrderByDescending(x=>x.Id);
            if (tf.from.HasValue)
            {
                 q = q.Where(x => x.CreatedStamp.Date >= tf.from.Value.AddHours(3).Date).OrderByDescending(x=>x.Id);
            }

            if (tf.to.HasValue)
            {
                q = q.Where(x => x.CreatedStamp.Date <= tf.to.Value.AddHours(3).Date).OrderByDescending(x=>x.Id);
            }

            if (!string.IsNullOrWhiteSpace(tf.fromPost))
            {
                q = q.Where(x => x.PostFromId == tf.fromPost).OrderByDescending(x=>x.Id);
            }

            if (!string.IsNullOrWhiteSpace(tf.toPost))
            {
                q = q.Where(x => x.PostToId == tf.toPost).OrderByDescending(x=>x.Id);
            }

            if (tf.orderNumber.HasValue && tf.orderNumber.Value != 0)
            {
                q = q.Where(x => x.Lines.Count(z => z.OrderNumber == tf.orderNumber.Value) > 0).OrderByDescending(x=>x.Id);
            }

            if (!string.IsNullOrWhiteSpace(tf.articleId))
            {
                q = q.Where(x => x.Lines.Count(z => z.Article.Contains(tf.articleId)) > 0).OrderByDescending(x=>x.Id);
            }

            var result =  await q.Skip(offset).Take(10).ToListAsync();
            return result;
        }
    }
}