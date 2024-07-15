namespace ProtoLib2.Model;

public class MaconomyMovementTransaction
{
    public long Id { get; set; }
    public long TransactionId { get; set; }
    public string LogFile { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime Stamp { get; set; }
}