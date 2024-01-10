using System;
using System.Collections.Generic;

namespace StableProduction
{

    public class TechCard
    {
        public Guid Id { get; set; }
        public ICollection<TechOperation> Operations { get; set; }
        public string Name { get; set; }
        public List<StoredImage> Images { get; set; }
    }
}