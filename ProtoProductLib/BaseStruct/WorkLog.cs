using System;

namespace ProtoProductLib.BaseStruct
{
    public class WorkLog
    {
        public long Id { get; set; }
        public long WorkId { get; set; }
        public Work Work { get; set; }
        public WorkStatus PrevStatus { get; set; }
        public WorkStatus NewStatus { get; set; }
        public DateTime Stamp { get; set; }
        public User EditedBy { get; set; }
    }
}