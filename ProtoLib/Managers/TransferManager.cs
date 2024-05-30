﻿using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class TransferManager
{
    public string Print(long id)
    {
        using (BaseContext c = new BaseContext())
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
        using (BaseContext c = new BaseContext())
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
                tl.OrderNumber = w.OrderNumber;
                tl.Article = w.Article;
                tl.IsTransfered = false;
                tl.Count = w.Count;
                tl.Remark = "";
                tl.ProductionLine = w.ProductLine;
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
            var works = c.Works.AsNoTracking().Where(x => x.Status == WorkStatus.income && x.PostId == t.PostToId).ToList();
            
            foreach (var notFullTransfer in t.Lines.Where(x=>x.TransferedCount!= x.Count))
            {
                var toSplitWork = works.FirstOrDefault(x =>
                    x.PostId == t.PostToId && x.Article == notFullTransfer.Article &&
                    x.OrderNumber == notFullTransfer.OrderNumber);
                if (toSplitWork != null)
                {
                    var newWorks = wmf.SplitWork(toSplitWork, notFullTransfer.Count - notFullTransfer.TransferedCount);
                    var myWork = newWorks[0]; //эту двинем дальше ниже
                    var returnWork = newWorks[1]; //эту надо вернуть в прогноз
                

                    wmf.MoveToPostRequest(returnWork.Id,t.PostFromId, new List<string>(),
                        notFullTransfer.Remark);
                    
                    Debug.WriteLine(JsonConvert.SerializeObject(newWorks));
                }
                else
                {
                    Debug.WriteLine("ApplyTansfer: work to split is null");
                } 
            }
            works = c.Works.AsNoTracking().Where(x => x.Status == WorkStatus.income && x.PostId == t.PostToId).ToList();
            var transfer = c.Transfers.Include(x=>x.Lines).FirstOrDefault(x => x.Id == t.Id);
            List<Work> toPost = new List<Work>();
            foreach (var stl in t.Lines)
            {
                var existTl = transfer.Lines.FirstOrDefault(x => x.Id == stl.Id);
                existTl.IsTransfered = stl.IsTransfered;
                existTl.TransferedCount = stl.TransferedCount;
                existTl.Remark = stl.Remark;

                var work = works.FirstOrDefault(x =>
                    x.Article == stl.Article && x.PostId == t.PostToId && x.Status == WorkStatus.income &&
                    x.OrderNumber == stl.OrderNumber && x.Count == existTl.TransferedCount);
                if (work != null)
                {
                    existTl.TargetWorkId = work.Id;
                    toPost.Add(work);
                }
                else
                {
                    Debug.WriteLine("ApplyTansfer: work to assign is null");
                } 

            }

            WorkSaveManager wsm = new WorkSaveManager();
            WorkStatusChanger wss = new WorkStatusChanger();
            wss.ChangeStatus(toPost, WorkStatus.waiting, accName);
            wsm.SaveWorks(toPost);

            transfer.ClosedStamp = DateTime.Now;
            transfer.Closed = DateTime.Today;
            transfer.ClosedBy = c.Users.FirstOrDefault(x => x.AccName == accName)?.Name;
            
            c.SaveChanges();
        }

       
        
    }

    public List<Transfer> ListIn(string postId){
        using(BaseContext c = new BaseContext())
        {
            return c.Transfers.Include(x=>x.Lines).Where(x => x.Closed == null && x.PostToId == postId).ToList();
        }
    } 
    public List<Transfer> ListOut(string postId){
        using(BaseContext c = new BaseContext())
        {
            var notClosed= c.Transfers.Include(x=>x.Lines).Where(x => x.Closed == null && x.PostFromId == postId).ToList();
            DateTime d = DateTime.Today.AddDays(-4);
            var closedLast = c.Transfers.Include(x => x.Lines).Where(x => x.Closed.HasValue && x.ClosedStamp > d && x.PostFromId==postId)
                .Take(10).ToList();
            if (closedLast.Count > 0)
            {
                notClosed.AddRange(closedLast);
            }

            return notClosed;

        }
    } 
}