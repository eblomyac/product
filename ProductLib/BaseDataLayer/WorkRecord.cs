using System;
using System.ComponentModel.DataAnnotations;

namespace ProductLib.BaseDataLayer
{
    public class WorkRecord
    {
        //store only
        public long Id { get; set; }
        [MaxLength(64)]
        public string Article { get; set; }
        [MaxLength(16)]
        public string Post { get; set; }
        public long OrderNumber { get; set; }
        [MaxLength(32)]
        public string ProductionLine { get; set; }
        public long? ParentId { get; set; }
        public DateTime Created { get; set; }
        public long CreatedBy { get; set; }
        public float Cost { get; set; }
        
        
        //modified fields
        public float Count { get; set; }
        public int Status { get; set; }
        public DateTime? EndStamp { get; set; }
    }
    
}