using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class WorkStatusChanger
    {
        
        private BaseContext? _c = null;
        public WorkStatusChanger(BaseContext? c=null)
        {
            _c = c;
        }
        public void ChangeStatus(Work w, WorkStatus newStatus, string accName, string moveTo = "",
            string moveFrom = "")
        {
            var oldStatus = w.Status;
            w.Status = newStatus;
            if (moveTo.Length > 0 && string.IsNullOrWhiteSpace(w.MovedTo))
            {
                w.MovedTo = moveTo;
            }

            if (moveFrom.Length > 0 && string.IsNullOrWhiteSpace(w.MovedFrom))
            {
                w.MovedFrom = moveFrom;
            }
            if (newStatus == WorkStatus.waiting && oldStatus == WorkStatus.income)
            {
                //перевести переднные работы в завершенные
                BaseContext c = _c ?? new BaseContext(accName);
                {
                    //только на предыдущий
                    
                    var sendedWorks = c.Works.Where(x =>
                        x.Article == w.Article && x.OrderNumber == w.OrderNumber 
                                               && x.Status == WorkStatus.sended
                                               && w.Id!=x.Id 
                                               && x.Count == w.Count )
                                                   //&& x.MovedTo == w.MovedFrom )
                        .ToList();
                  //  var endedWorks = c.Works.Where(x =>
                  //          x.Article == w.Article && x.OrderNumber == w.OrderNumber && x.Status == WorkStatus.ended && w.Id!=x.Id && x.Count == w.Count && x.MovedTo == w.MovedFrom)
                  //      .ToList();
                    this.ChangeStatus(sendedWorks, WorkStatus.ended, accName);
                    //this.ChangeStatus(endedWorks)
                    c.SaveChanges();
                }
                if (_c == null)
                {
                    c.Dispose();
                }
            }

            if (newStatus == WorkStatus.sended && oldStatus == WorkStatus.running && !string.IsNullOrWhiteSpace(w.MovedTo))
            {
                // возвращенная работа: сразу в статус завершено и разрешить события
            
                w.Status = WorkStatus.ended;

                BaseContext c = _c ?? new BaseContext(accName);
                {
                   var nextPostIssue= c.Issues.AsNoTracking().FirstOrDefault(x => x.Work.Article == w.Article 
                       && x.Work.OrderNumber == w.OrderNumber
                       && x.ReturnBackPostId == w.PostId 
                       && x.Work.PostId == w.MovedTo);
                   if (nextPostIssue != null)
                   {
                       IssueManager im = new IssueManager();
                       im.ResolveIssue(nextPostIssue.Id, accName);
                   }
                }
                if (_c == null)
                {
                    c.Dispose();
                }

            }
            
        }
        

        public bool ChangeStatus(long id, WorkStatus newStatus,string accName, string moveTo="",string moveFrom="")
        {
            BaseContext c = _c ?? new BaseContext(accName);
            try
            {
                var work = c.Works.Include(x => x.Issues).FirstOrDefault(x => x.Id == id);
                if (work == null)
                {
                    //
                }
                else
                {
                    var actualIssues = work.Issues.Where(x => x.Resolved == null).ToList();
                    if ((newStatus == WorkStatus.sended || newStatus == WorkStatus.running) && actualIssues.Count>0)
                    {
                        return false;
                    }
                    else
                    {
                        ChangeStatus(work, newStatus, accName, moveTo, moveFrom);
                    }
                }

                int r = c.SaveChanges();
                return r > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (_c == null)
                {
                    c.Dispose();
                }
            }
        }
        public void ChangeStatus(List<Work> w, WorkStatus newStatus, string accName)
        {
            foreach (var work in w)
            {
                this.ChangeStatus(work,newStatus,accName);
            }
        }
    }
}