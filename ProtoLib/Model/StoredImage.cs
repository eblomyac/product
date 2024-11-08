using System;

namespace ProtoLib.Model
{
    public class StoredImage
    {
        public Guid Id { get; set; }
        public string InitialFileName { get; set; }
        public string UploadedBy { get; set; }
        public string PostId { get; set; }
        public string LocalPath { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        
        public Guid TechCardId { get; set; }
        public TechCard TechCard { get; set; }
     
    }
}