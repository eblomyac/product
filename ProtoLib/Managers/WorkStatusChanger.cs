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

            if (newStatus == WorkStatus.sended && oldStatus == WorkStatus.running && w.OrderNumber==100)
            {
                // возвращенная работа: сразу в статус завершено и разрешить события
            
                w.Status = WorkStatus.ended;

                BaseContext c = _c ?? new BaseContext(accName);
                {
                    
                    string s = w.Description;
                    if (s.Contains("Заказ: ") && s.Contains("Артикул: "))
                    {
                        s = s.Replace("Заказ: ", "");
                        s = s.Replace(" Артикул: ", "");

                        string[] sp = s.Split(",");

                        string art = sp[1];    
                        string order = sp[0];

                        string[] sp2 = sp[1].Split(" x ");
                        string realArt = sp2[0];
                        string count = sp2[1];
                       
                        long orderLong = long.Parse(order);
                        decimal countDecimal = decimal.Parse(count);
                        
                        var nextPostIssue= c.Issues.Include(x=>x.Work).AsNoTracking().FirstOrDefault(x => x.Work.Article == realArt
                            && x.Work.OrderNumber == orderLong && x.Work.Count == countDecimal
                            && x.ReturnBackPostId == w.PostId);
                        if (nextPostIssue != null)
                        {
                            IssueManager im = new IssueManager();
                            im.ResolveIssue(nextPostIssue.Id, accName);
                        }
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
        public void ChangeStatus(List<Work> w, WorkStatus newStatus, string accName, string moveFrom="")
        {
            foreach (var work in w)
            {
                this.ChangeStatus(work,newStatus,accName, moveFrom:moveFrom);
            }
        }
    }
}