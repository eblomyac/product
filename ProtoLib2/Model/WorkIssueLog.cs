using System.ComponentModel.DataAnnotations;

namespace ProtoLib2.Model;

public class WorkIssueLog
{
    public long Id { get; set; }

    [MaxLength(32)] public string PostId { get; set; }

    [MaxLength(64)] public string Article { get; set; }

    public long OrderNumber { get; set; }
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }

    [MaxLength(64)] public string Type { get; set; }

    [MaxLength(256)] public string Description { get; set; }

    public string ReturnedToPost { get; set; }

    public long SourceIssueId { get; set; }
}