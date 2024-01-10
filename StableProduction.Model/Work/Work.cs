using System;

namespace StableProduction.Work
{
    public enum WorkStatus
    {
        Creating,
        Hidden,
        Input,
        Pause,
        Run,
        Out,
        Complete
    }
    public class Work
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Post Post { get; set; }
    }
}