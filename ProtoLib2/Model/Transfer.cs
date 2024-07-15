using System.ComponentModel.DataAnnotations;

namespace ProtoLib2.Model;

public class Transfer
{
    public long Id { get; set; }

    [MaxLength(64)] public string PaperId { get; set; }

    [MaxLength(32)] public string PostFromId { get; set; }

    public Post PostFrom { get; set; }

    [MaxLength(32)] public string PostToId { get; set; }

    public Post PostTo { get; set; }
    public DateTime Created { get; set; }
    public DateTime CreatedStamp { get; set; }
    public DateTime? Closed { get; set; }
    public DateTime? ClosedStamp { get; set; }
    public string CreatedBy { get; set; }
    public string ClosedBy { get; set; }

    public List<TransferLine> Lines { get; set; }
}