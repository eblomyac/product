using System;

namespace StableProduction
{
    public class StoredImage
    {
        public Guid Id { get; set; }
        public string LocalPath { get; set; }
        public string Url { get; set; }
        
    }
}