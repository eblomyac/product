using System.ComponentModel.DataAnnotations;

namespace ProtoLib.Model;

public class OperatorCountChangeRecord
{
    public long Id { get; set; }
    public long WorkId { get; set; }
    [MaxLength(64)]
    public string EditBy { get; set; }
    public DateTime Stamp { get; set; }
    public long OrderNumber { get; set; }
    public int LineNumber { get; set; }
    [MaxLength(128)]
    public string Article { get; set; }
    public int OldCount { get; set; }
    public int NewCount { get; set; }
    
    public WorkStatus StatusWhenChanged { get; set; }

}