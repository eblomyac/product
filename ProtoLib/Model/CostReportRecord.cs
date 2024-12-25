namespace ProtoLib.Model;

public class CostReportRecord
{
    public long Id { get; set; }
    public DateTime Stamp { get; set; }
    public string PostId { get; set; }
    public decimal CurrentMyWait { get; set; }
    public decimal CurrentMyWork { get; set; }
    public decimal CurrentMyEnd { get; set; }
    public decimal CurrentUncompleteWait { get; set; }
    public decimal CurrentUncompleteWork { get; set; }
    public decimal CurrentUncompleteEnd { get; set; }
    
}