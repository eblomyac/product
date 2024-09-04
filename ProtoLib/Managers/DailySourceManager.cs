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

    public List<DailySource> LastValues(string postId, bool addTodayAsEmpty)
    {
        using (BaseContext c = new BaseContext("system"))
        {
            var prodLines = c.ProductionLines.ToList();
            DateTime d = DateTime.Today;

            var vals = new List<DailySource>();
            for (int loop = 0; loop < 4; loop++)
            {
                var before = d.AddDays(loop*-1);
                var values = c.DailySources
                    .Where(x => x.Year == before.Year && x.Month == before.Month && x.Day == before.Day && x.PostId == postId).ToList();
                if (values.Count > 0)
                {
                    vals.AddRange(values);
                }
                else
                {
                    if (loop == 0 && addTodayAsEmpty)
                    {
                        foreach (var pl in prodLines)
                        {
                            DailySource ds = new DailySource();
                            ds.ProductLineId = pl.Id;
                            ds.PostId = postId;
                            ds.Day = d.Day;
                            ds.Month = d.Month;
                            ds.Year = d.Year;
                            ds.Value = 0;
                            ds.FilledBy = "";
                            ds.Id = int.MaxValue;
                            vals.Add(ds);
                        }
                    }
                }
                
            }

            return vals.OrderByDescending(x=>x.Id).ToList();
        }
    }

    public List<DailySource> TodayValues(string postId)
    {
        using (BaseContext c = new BaseContext("system"))
        {
            

            var prodLines = c.ProductionLines.ToList();
            
            int year = DateTime.Today.Year;
            int month = DateTime.Today.Month;
            int day = DateTime.Today.Day;
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

    public List<DailySource>  UpdateValues(List<DailySource> dailySources, string accName)
    {
        List<DailySource> r = new List<DailySource>();
        using (BaseContext c = new BaseContext(accName))
        {
            foreach (var ds in dailySources)
            {
                var exist = c.DailySources.FirstOrDefault(x =>
                    x.Year == ds.Year && x.Day == ds.Day && x.Month == ds.Month && x.PostId == ds.PostId &&
                    x.ProductLineId == ds.ProductLineId);
                if (exist == null)
                {
                    exist = new DailySource();
                    exist.Day = ds.Day;
                    exist.Year = ds.Year;
                    exist.Month = ds.Month;
                    exist.ProductLineId = ds.ProductLineId;
                    exist.PostId = ds.PostId;
                    c.DailySources.Add(exist);
                }

                exist.FilledBy = accName;
                exist.Value = ds.Value;
                r.Add(exist);

            }
            c.SaveChanges();
        }

        return r.OrderByDescending(x => x.Id).ToList();
    }
}