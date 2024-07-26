using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtoLib.Model;

public class Transfer
{
    public long Id { get; set; }
    [MaxLength(64)]
    public string PaperId { get; set; }
    [MaxLength(32)]
    public string PostFromId { get; set; }
    public Post PostFrom { get; set; }
    [MaxLength(32)]
    public string PostToId { get; set; }
    public Post PostTo { get; set; }
    public DateTime Created { get; set; }
    public DateTime CreatedStamp { get; set; }
    public DateTime? Closed { get; set; }
    public DateTime? ClosedStamp { get; set; }
    public string CreatedBy { get; set; }
    public string ClosedBy { get; set; }
    
    public List<TransferLine> Lines { get; set; }
    
    [NotMapped]
    public string Orders {
        get
        {
            if (this.Lines != null)
            {
                return string.Join(", ", this.Lines.Select(x => x.OrderNumber).Distinct());
            }

            return "";
        }
    }

    [NotMapped]
    public int TotalItemsCount
    {
        get
        {
            if (this.Lines != null)
            {
                return this.Lines.Sum(x => x.Count);
            }

            return 0;
        }
    }

    [NotMapped]
    public int TotalItemsTransfered
    {
        get
        {
            if (this.Lines != null)
            {
                return this.Lines.Sum(x => x.TransferedCount);
            }

            return 0;
        }
    }
    
}