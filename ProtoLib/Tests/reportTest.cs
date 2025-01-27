using System.Diagnostics;
using ClosedXML.Excel;
using KSK_LIB.Excel;
using Newtonsoft.Json;
using NUnit.Framework;
using ProtoLib.Managers;

namespace ProtoLib.Tests;

[TestFixture]
public class reportTest


{
    [Test]
    public async Task MakeDailyReport()
    {

        ReportManager rm = new ReportManager();
        var pDateFrom = new DateTime(2024, 2, 5);
        var pDateTo = new DateTime(2024, 2, 5);

        var request = await rm.PeriodReport(pDateFrom,pDateTo, true);
        rm.StoreDailyReports(request);
        
     

        /*
         *    var s = rm.DailyReportsToExcel(pDateFrom, pDateTo);
        ExcelExporter ee = new ExcelExporter("full-report.xlsx");
        ee.ExportSet(s);
     */
    }
    [Test]
    public async Task MakeCostReport()
    {
        AnalyticManager am = new AnalyticManager();
        var rep = await am.CostReport();
        am.SaveTodayCostReport(rep);
        
    }
    
    [Test]
    public async Task CostRep()
    {
        AnalyticManager am = new AnalyticManager();
        var report = await am.CostReport();
        var table = am.CostReportToTable(report);
        
        Console.WriteLine(JsonConvert.SerializeObject(report));
        Console.WriteLine(JsonConvert.SerializeObject(table));
    }
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