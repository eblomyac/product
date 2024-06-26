using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace ProtoLib.Model
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
                case WorkStatus.sended:
                    return Constants.Work.Statuses.Sended;
                default:
                    return "";
            }
        }
    }
    public enum WorkStatus
    {
        unkown=-10,
        hidden=0,
        income=10,
        waiting=20,
        running = 30,
        sended=40,
        ended=50,
      
            
    }
    public class Work:IComparable<Work>, IEquatable<Work>, ICloneable
    {
        public long Id { get; set; }
        public long OrderNumber { get; set; }
        public int OrderLineNumber { get; set; }
        [MaxLength(64)]
        public string Article { get; set; }
        [MaxLength(32)]
        public string PostId { get; set; }
        public Post? Post { get; set; }
        public decimal SingleCost { get; set; }
        public int Count { get; set; }
        public WorkStatus Status { get; set; }
        
       
        [MaxLength(32)]
        public string ProductLineId { get; set; }
        [MaxLength(256)]
        public string Description { get; set; }
        public DateTime DeadLine { get; set; }
        
        public DateTime CreatedStamp { get; set; }
        [MaxLength(32)]
        public string? MovedFrom { get; set; }
        [MaxLength(32)]
        public string? MovedTo { get; set; }

        public virtual List<WorkIssue>? Issues{
            get;
            set;
        }
        
        public virtual ICollection<AdditionalCost> AdditionalCosts { get; set; }

        [NotMapped]
        public int Priority { get; set; }

        [NotMapped]
        public List<string> Comments
        {
            get
            {
                if (string.IsNullOrEmpty(this.CommentMap))
                {
                    return new List<string>();
                }

                else
                {
                    
                    return CommentMap.Split('\t').ToList();
                }
            }
            set
            {
                this.CommentMap = string.Join('\t',
                    value.Select(x => x.Replace("\r", "").Replace("\n", "").Replace("\t", "")));
                
            }
        }

        [JsonIgnore]
        public string CommentMap { get; set; } = "";

        [NotMapped] public bool CanClosed { get; set; } = false;

        [NotMapped]
        public string StatusString
        {
            get { return WorkStatusMapper.Map(this.Status); }
        }
        [NotMapped]
        public decimal TotalCost
        {
            get { return Count * SingleCost; }
        }

        public void TakeNewInfo(Work other)
        {
            this.Status = other.Status;
            this.Count = other.Count;
            this.MovedFrom = other.MovedFrom;
            this.MovedTo = other.MovedTo;
        }

        public int CompareTo(Work? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var orderNumberComparison = OrderNumber.CompareTo(other.OrderNumber);
            if (orderNumberComparison != 0) return orderNumberComparison;
            var articleComparison = string.Compare(Article, other.Article, StringComparison.Ordinal);
            if (articleComparison != 0) return articleComparison;
            return string.Compare(PostId, other.PostId, StringComparison.Ordinal);
        }

        public bool Equals(Work? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return OrderNumber == other.OrderNumber && Article == other.Article && PostId == other.PostId;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Work) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OrderNumber, OrderLineNumber);
        }

        public object Clone()
        {
            Work w = new Work();
            w.OrderLineNumber = this.OrderLineNumber;
            w.Article = this.Article;
            w.Count = this.Count;
            w.PostId = this.PostId;
            w.OrderNumber = this.OrderNumber;
            w.SingleCost = this.SingleCost;
            w.Status = this.Status;
            w.Description = this.Description;
            w.ProductLineId = this.ProductLineId;
            w.CreatedStamp = DateTime.Now;
            w.MovedFrom = this.MovedFrom;
            w.MovedTo = this.MovedTo;
            w.CommentMap = this.CommentMap;
            w.Priority = this.Priority;
            w.DeadLine = this.DeadLine;
            return w;
        }
    }
    
    
}