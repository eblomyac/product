using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductLibPrototype;

namespace ProtoProductLib.BaseStruct
{
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
    public enum WorkStatus
    {
        hidden=0,
        income=10,
        waiting=20,
        running = 30,
        ended=40
            
    }
    public class Work
    {
        public long Id { get; set; }
        public long OrderNumber { get; set; }
        [MaxLength(64)]
        public string Article { get; set; }
        [MaxLength(32)]
        public string PostId { get; set; }
        public Post Post { get; set; }
        public float SingleCost { get; set; }
        public int Count { get; set; }
        public WorkStatus Status { get; set; }

      
        

        [NotMapped]
        public string StatusString
        {
            get { return WorkStatusMapper.Map(this.Status); }
        }
        [NotMapped]
        public float TotalCost
        {
            get { return Count * SingleCost; }
        }

    }
}