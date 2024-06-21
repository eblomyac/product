using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class DailySourceManager
{
    public List<DailySource> DateValue(string postId, DateTime date)
    {

        using (BaseContext c = new BaseContext(""))
        {
            var result = c.DailySources.Where(x =>
                x.PostId == postId && x.Year == date.Year && x.Month == date.Month && x.Day == date.Day).ToList();
            if (result.Count == 0)
            {
                var prodLines = c.ProductionLines.ToList();
                foreach (var pl in prodLines)
                {
                    var prodLine = new DailySource();
                    prodLine.Year = date.Year;
                    prodLine.Month = date.Month;
                    prodLine.Day = date.Day;
                    prodLine.ProductLineId = pl.Id;
                    prodLine.PostId = postId;
                    prodLine.Value = 0;
                    prodLine.FilledBy = "system";
                    result.Add(prodLine);
                }
            }
            return result;
        }
    }

    public Dictionary<string, decimal?> DateValues(DateTime date)
    {
        Dictionary<string, decimal?> result = new();
        using (BaseContext c = new BaseContext(""))
        {
            var posts = c.Posts.ToList().Select(x=>x.Name).ToList();
            var values = c.DailySources.Where(x => x.Year == date.Year && x.Month == date.Month && x.Day == date.Day).ToList();

            foreach (var post in posts)
            {
                var value = values.FirstOrDefault(x => x.PostId == post)?.Value;
                result.Add(post,value);
            }

            return result;
        }
    }

    public List<DailySource> TodayValues(string postId)
    {
        using (BaseContext c = new BaseContext(""))
        {
            int year = DateTime.Today.Year;
            int month = DateTime.Today.Month;
            int day = DateTime.Today.Day;

            var prodLines = c.ProductionLines.ToList();
            
            var values = c.DailySources.Where(x => x.Year == year && x.Month == month && x.Day == day && x.PostId == postId).ToList();

            if (values.Count < prodLines.Count)
            {
                List<DailySource> result = new List<DailySource>();
                foreach (var pl in prodLines)
                {
                    var prodLine = new DailySource();
                    prodLine.Year = year;
                    prodLine.Month = month;
                    prodLine.Day = day;
                    prodLine.ProductLineId = pl.Id;
                    prodLine.PostId = postId;
                    prodLine.Value = 0;
                    prodLine.FilledBy = "system";
                    result.Add(prodLine);
                }

                return result;
            }
            else
            {
                return values;
            }
          
        }
    }

    public DailySource FillValue(string postId, DateTime date, decimal value, string filledBy, string productionLine)
    {
        using (BaseContext c = new BaseContext(""))
        {
            var result = c.DailySources.FirstOrDefault(x =>
                x.PostId == postId && x.Year == date.Year && x.Month == date.Month && x.Day == date.Day && x.ProductLineId == productionLine);
            if (result == null)
            {
                result = new DailySource();
                result.Day = date.Day;
                result.Month = date.Month;
                result.Year = date.Year;
                result.Value = value;
                result.PostId = postId;
                result.FilledBy = filledBy;
                result.ProductLineId = productionLine;
                c.DailySources.Add(result);
            }
            else
            {
                result.Value = value;
                result.FilledBy = filledBy;
            }

            c.SaveChanges();
            return result;
        }
    }
}