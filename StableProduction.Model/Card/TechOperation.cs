using System;
using System.Collections.Generic;

namespace StableProduction
{
    public class TechOperation
    {
        public Guid Id { get; set; }
        public long PostId { get; set; }
        public Post Post { get; set; }
        public int Order { get; set; }
        
     
        public string Comment { get; set; }
        public string OprationText { get; set; }
        public List<Device> UsedDevices { get; set; }
        public Bench? UsedBench { get; set; }
        public List<StoredImage> Images { get; set; }
        
        public decimal Time { get; set; }
        public decimal TimeRun { get; set; }
        
    }
}