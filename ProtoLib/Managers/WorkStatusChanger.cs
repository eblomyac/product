using System.Collections.Generic;
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
                    var sendedWorks = c.Works.Where(x =>
                        x.Article == w.Article && x.OrderNumber == w.OrderNumber && x.Status == WorkStatus.sended && w.Id!=x.Id && x.Count == w.Count)
                        .ToList();
                    this.ChangeStatus(sendedWorks, WorkStatus.ended, accName);
                    c.SaveChanges();
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