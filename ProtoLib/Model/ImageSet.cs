using System;
using System.Collections.Generic;

namespace ProtoLib.Model
{
    public class ImageSet
    {
        public Guid Id { get; set; }
        public Guid ImageId { get; set; }
        public List<StoredImage> Images { get; set; }
    }
    
}