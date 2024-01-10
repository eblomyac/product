using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtoLib.Model
{
    public class PostStatistic
    {
        public long Id { get; set; }
        public DateTime Stamp { get; set; }
        public string PostId { get; set; }
        public long OrderNumber { get; set; }

        [NotMapped]
        public decimal TotalCost
        {
            get
            {
                return PredictCost + IncomeCost + WaitingCost + RunningCost + SendedCost;
            }
        }

        public decimal PredictCost { get; set; }
        public decimal IncomeCost { get; set; }
        public decimal WaitingCost { get; set; }
        public decimal RunningCost { get; set; }
        public decimal SendedCost { get; set; }
        
        public int ActualEvents { get; set; }
        
    }
}