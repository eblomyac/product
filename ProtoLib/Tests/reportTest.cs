using ClosedXML.Excel;
using NUnit.Framework;
using ProtoLib.Managers;

namespace ProtoLib.Tests;

[TestFixture]
public class reportTest
{
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