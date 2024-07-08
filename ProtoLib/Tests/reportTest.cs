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
        DateTime pDate = new DateTime(2024,7,6);
        var request = rm.DailyReportMail(pDate);
        var request2 = rm.DailyReportMail2(pDate);
        foreach (var r in request)
        {
            await EmailNotificatorSingleton.Instance.Send(r);    
        }    
        foreach (var r in request2)
        {
            await EmailNotificatorSingleton.Instance.Send(r);    
        }   
    }
    

}