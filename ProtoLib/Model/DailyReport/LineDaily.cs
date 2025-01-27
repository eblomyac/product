using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ProtoLib.Model.DailyReport;

[Table("LineDaily",Schema="DailyReport")]
public class LineDaily
{
    public long Id { get; set; }
    public DateTime Stamp { get; set; }
    public string Line { get; set; }
    public decimal ItemsDone { get; set; }
    public decimal CostCompleted { get; set; }
    public decimal AdditionalCostCompleted { get; set; }
    public decimal ItemsDoneCost { get; set; }
    public decimal ItemsReceived { get; set; }
    public decimal DailyBudget { get; set; }
    
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