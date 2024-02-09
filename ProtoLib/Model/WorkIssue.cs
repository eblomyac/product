using System;
using Newtonsoft.Json;

namespace ProtoLib.Model
{
    public class WorkIssue
    {
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Resolved { get; set; }
        
        public long WorkId { get; set; }
        [JsonIgnore]
        public Work? Work { get; set; }
        
        public string Description { get; set; }
        public long TemplateId { get; set; }
        [JsonIgnore]
        public WorkIssueTemplate? Template { get; set; }
        
        public string ReturnBackPostId { get; set; }
        public string ReturnedFromPostId { get; set; }
    }

    public class WorkIssueTemplate
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }
    }
}