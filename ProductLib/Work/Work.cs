using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductLib.Work
{

   
        public enum WorkStatus
        {
            hidden=0,
            income=10,
            waiting=20,
            running = 30,
            ended=40
            
        }

        public static class WorkStatusMapper
        {
            public static string Map(WorkStatus status)
            {
                switch (status)
                {
                    case WorkStatus.hidden:
                        return Constants.Work.Statuses.Hidden;
                    case WorkStatus.income:
                        return Constants.Work.Statuses.Income;
                    case WorkStatus.waiting:
                        return Constants.Work.Statuses.Waiting;
                    case WorkStatus.running:
                        return Constants.Work.Statuses.Running;
                    case WorkStatus.ended:
                        return Constants.Work.Statuses.Ended;
                    default:
                        return "";
                }
            }
        }
        
       public abstract class Work
       {
           //modified work lifetime
           private WorkStatus _status;
           public int Count { get;  set; }
           public DateTime? EndStamp { get; protected private set; }
           public WorkStatus Status
           {
               get { return this._status;}
               set
               {
                   if (this.ChildWorks != null)
                   {
                       foreach (var childWork in this.ChildWorks)
                       {
                           childWork.Status = value;
                       }
                   }

                   if (value == WorkStatus.ended)
                   {
                       this.EndStamp = DateTime.Now;
                   }
                   this._status = value;
               }
           }
           //createonly
           public float SelfSingleCost { get; set; }
           private protected string Prefix { get;  set; }
           public DateTime CreateStamp { get; protected private set; }
          
           public Work? Parent { get; protected private set; }
           public ICollection<Work> ChildWorks { get; protected private set; }
           public virtual string FullName => string.Join(' ', Prefix);
           public string StatusString => WorkStatusMapper.Map(this.Status);
           public float TotalSelfCost => Count * SelfSingleCost;
           public float TotalCost
           {
               get
               {
                   if (this.ChildWorks != null)
                   {
                       return this.ChildWorks.Sum(x => x.TotalCost) + this.TotalSelfCost;
                   }
                   else
                   {
                       return this.TotalSelfCost;
                   }
               }
           }
           /// <summary>
           /// 
           /// </summary>
           public Work()
           {
               this.CreateStamp=DateTime.Now;
           }
       }
      
   
       
       

   
}