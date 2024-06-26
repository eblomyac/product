using System.Diagnostics;
using ClosedXML.Excel;
using Newtonsoft.Json;
using NUnit.Framework;
using ProtoLib.Managers;

namespace ProtoLib.Tests;

[TestFixture]
public class reportTest
{
    [Test]
    public void TestOrderStatRep()
    {
        AnalyticManager am = new AnalyticManager();
        

        dynamic data = am.TotalOrderStat("543426", "V3840-8/1S");
        Console.WriteLine(JsonConvert.SerializeObject(data));
    }
    [Test]
    public async Task TestRep()
    {
        ReportManager rm = new ReportManager();
        DateTime pDate = new DateTime(2024,6,10);
        var request = rm.DailyReportMail(pDate);
        foreach (var r in request)
        {
            await EmailNotificatorSingleton.Instance.Send(r);    
        }    
    }
    

}