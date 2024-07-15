using System.ComponentModel.DataAnnotations;

namespace ProtoLib2.Model;

public class WorkPriority
{
    public long Id { get; set; }
    public long OrderNumber { get; set; }

    [MaxLength(64)] public string Article { get; set; }

    public int Priority { get; set; }
    public DateTime DateChange { get; set; }
}