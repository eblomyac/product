using System.Data;
using KSK_LIB.Excel;
using NUnit.Framework;
using ProtoLib.Model;

namespace ProtoLib.Tests;

[TestFixture]
public class WorkRemove
{

    [Test]
    public void removeWorks()
    {
        string FilePath = @"C:\Users\yande\OneDrive\Desktop\Книга1.xlsx";
        ExcelReader er = new ExcelReader();
        var t = er.ReadTable(FilePath);
        using (BaseContext c = new BaseContext())
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