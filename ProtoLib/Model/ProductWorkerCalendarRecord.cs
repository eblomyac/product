namespace ProtoLib.Model;

public class ProductWorkerCalendarRecord
{
    public long Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public long ProductWorkerId { get; set; }
    public string ProductionLineId { get; set; }
    public string PostId { get; set; }
    public decimal PlannedHours { get; set; }
    
    
}