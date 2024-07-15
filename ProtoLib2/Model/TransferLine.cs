namespace ProtoLib2.Model;

public class TransferLine
{
    public long Id { get; set; }

    public string Article { get; set; }
    public long OrderNumber { get; set; }
    public int OrderLineNumber { get; set; }
    public int Count { get; set; }

    public string ProductionLine { get; set; }

    public bool IsTransfered { get; set; }
    public int TransferedCount { get; set; }
    public string Remark { get; set; }

    public long SourceWorkId { get; set; }

    public decimal SourceWorkCost { get; set; }

    public long? TargetWorkId { get; set; }
}