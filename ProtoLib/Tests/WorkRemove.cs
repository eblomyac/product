using System.Data;
using System.Diagnostics;
using KSK_LIB.Excel;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProtoLib.Tests;

[TestFixture]
public class WorkRemove
{
    [Test]
    public void takeAllWorksFromIncBuffer()
    {
        WorkStatusChanger wss = new WorkStatusChanger();
        using (BaseContext c = new BaseContext(""))
        {
            var works = c.Works.AsNoTracking().Where(x => x.Status == WorkStatus.income).Select(x=>x.Id).ToList();
            foreach (var work in works)
            {
                wss.ChangeStatus(work, WorkStatus.waiting, "system");
            }
        }
    }
    
    [Test]
    public void removeWorks2()
    {
        string FilePath = @"C:\Users\yande\OneDrive\Desktop\Новый текстовый документ.txt";
        string s = File.ReadAllText(FilePath);
        var orders = s.Split(',');
        using (BaseContext c = new BaseContext(""))
        {
            foreach (var order in orders)
            {
                try
                {
                    string orderNormalize = order.Trim();
                    if (long.TryParse(orderNormalize, out var on))
                    {
                        var works = c.Works.Where(x => x.OrderNumber == on).Include(x => x.Issues).ToList();
                        var stat = c.WorkStatusLogs.Where(x => x.OrderNumber == on).ToList();
                        var issLog = c.WorkIssueLogs.Where(x => x.OrderNumber == on).ToList();
                        if (stat.Count > 0)
                        {
                            c.WorkStatusLogs.RemoveRange(stat);
                        }

                        if (issLog.Count > 0)
                        {
                            c.WorkIssueLogs.RemoveRange(issLog);
                        }

                        foreach (var w in works)
                        {
                            if (w.Issues != null && w.Issues.Count > 0)
                            {
                                c.Issues.RemoveRange(w.Issues);
                            }

                            c.Works.Remove(w);
                        }
                    }

                    c.SaveChanges();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"{order}: {e.Message}");
                }

            }    
        }
        
    }


    [Test]
    public void removeWorks()
    {
        string FilePath = @"C:\Users\yande\OneDrive\Desktop\Книга1.xlsx";
        ExcelReader er = new ExcelReader();
        var t = er.ReadTable(FilePath);
        using (BaseContext c = new BaseContext(""))
        {
            int d = 0;
            int e = 0;
            int n = 0;
            foreach (DataRow r in t.Rows)
            {
                string art = r[0].ToString();
                string order = r[3].ToString();
                long orderNum = long.Parse(order);

                var w = c.Works.FirstOrDefault(x => x.Article == art && x.OrderNumber == orderNum);
                if (w != null)
                {
                    c.Works.Remove(w);
                    try
                    {
                        c.SaveChanges();
                        d++;
                    }
                    catch (Exception exc)
                    {
                        int g = 0;
                        e++;
                    }
                }
                else
                {
                    int h = 0;
                    n++;
                }

            }  
            Console.WriteLine(d);
            Console.WriteLine(e);
            Console.WriteLine(n);
        }
        
    }
}