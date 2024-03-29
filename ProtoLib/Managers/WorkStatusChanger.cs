﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class WorkStatusChanger
    {
        public void ChangeStatus(Work w, WorkStatus newStatus,string accName)
        {
            var oldStatus = w.Status;
            w.Status = newStatus;
            if (newStatus == WorkStatus.waiting && oldStatus == WorkStatus.income)
            {
                //перевести переднные работы в завершенные
                using (BaseContext c = new BaseContext(accName))
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
            }

            if (newStatus == WorkStatus.sended && oldStatus == WorkStatus.running && !string.IsNullOrEmpty(w.MovedTo))
            {
                // возвращенная работа: сразу в статус завершено и разрешить события
            
                w.Status = WorkStatus.ended;

                using (BaseContext c = new BaseContext())
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

            }
            
        }

        public bool ChangeStatus(long id, WorkStatus newStatus,string accName)
        {
            using (BaseContext c = new BaseContext(accName))
            {
                var work = c.Works.FirstOrDefault(x => x.Id == id);
                if (work == null)
                {
                    //
                }
                else
                {
                    ChangeStatus(work,newStatus,accName);
                }

                int r = c.SaveChanges();
                return r > 0;
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