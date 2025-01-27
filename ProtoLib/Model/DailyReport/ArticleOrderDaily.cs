using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProtoLib.Model.DailyReport;

[Table("ArticleOrderDaily",Schema="DailyReport")]
public class ArticleOrderDaily
{
    public long Id { get; set; }
    public DateTime Stamp { get; set; }

    public string Line { get; set; }
    public string Post { get; set; }
    public long OrderNumber { get; set; }
    public int OrderLineNumber { get; set; }
    public int MaxOrderLineNumber { get; set; }
    public string Article { get; set; }
    public decimal CompletedCount { get; set; }
    public decimal OrderCount { get; set; }
    public decimal CostCompleted { get; set; }
    public decimal AdditionalCompletedCost { get; set; }
    public decimal Weight { get; set; }
    public decimal CompletedWeight { get; set; }
    public decimal RemainsWeight { get; set; }
    public decimal TotalCompletedWeight { get; set; }
    public double RemainPart { get; set; }
    
    [NotMapped]
    public decimal CompletedPercentWeight{
        get
        {
            return CompletedWeight / Weight;
        }}
    
    
    
    [NotMapped]
    public int Year
    {
        get { return Stamp.Year; }
    }
    [NotMapped]
    public int Month {
        get { return Stamp.Month; 
        }}
    [NotMapped]
    public int Day {
        get { return Stamp.Day; 
        }}
    [NotMapped]
    public string WeekDay {
        get { return Stamp.ToString("dddd",CultureInfo.GetCultureInfo("ru-RU")); 
        }}

    [NotMapped]
    public int Week
    {
        get
        {
            return CultureInfo.GetCultureInfo("ru-RU").Calendar
                .GetWeekOfYear(Stamp, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
    }
}