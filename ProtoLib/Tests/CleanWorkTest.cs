using System.Diagnostics;
using KSK_LIB.DataStructure.MQRequest;
using NUnit.Framework;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProtoLib.Tests;

[TestFixture]
public class CleanWorkTest

{
    [Test]
    public void FindBadCountWorks()
    {
        AnalyticManager am = new AnalyticManager();
        var orders = am.ActualOrders();
        using (BaseContext c = new BaseContext())
        {
            var worksToCheck = c.Works.Where(x => orders.Contains(x.OrderNumber)).ToList();
            var groupedByPost = worksToCheck.GroupBy(x => x.PostId);
            

        }
    }
    
    [Test]
    public void fillWorkLog()
    {
        
        using (BaseContext c = new BaseContext())
        {
        
            while (true)
            {
                var toFix = c.WorkStatusLogs.Where(x =>
                        (x.ProductionLineId == null || x.Count == null || x.SingleCost == null || x.MovedFrom == null || x.MovedTo==null ||
                         x.OrderLineNumber == null) && x.WorkId > 0)
                    .Take(500).ToList();
                if (toFix.Count == 0)
                {
                    break;
                }
                var workIds = toFix.Select(x => x.WorkId).Distinct().ToList();
                var works = c.Works.Where(x => workIds.Contains(x.Id)).ToList();
                var toDelete = new List<WorkStatusLog>();
                foreach (var log in toFix)
                {
                    var work = works.FirstOrDefault(x => x.Id == log.WorkId);
                    if (work == null)
                    {
                        toDelete.Add(log);
                        continue;
                    }

                    log.SingleCost = work.SingleCost;
                    log.Count = work.Count;
                    log.ProductionLineId = work.ProductLineId;
                    log.OrderLineNumber = work.OrderLineNumber;
                    log.MovedFrom = work.MovedFrom;
                    log.MovedTo = work.MovedTo;
                }

                c.WorkStatusLogs.RemoveRange(toDelete);

                c.SaveChanges();
            }
        }
    }
    
    [Test]
    public void predictCloseTest()
    {
        MaintenanceManager mm = new MaintenanceManager();
        mm.CleanClosedPrediction();
        
    }
    
    [Test]
    public void Resolve()
    {
        MaintenanceManager mm = new MaintenanceManager();
        mm.ResolveStackIssues();


    }
    [Test]
    public async Task ErrorRemake()
    {
        WorkCleaner wc = new WorkCleaner();
        wc.RemoveError();

    }

    [Test]
    public void CleanTest2()
    {
        WorkCleaner wc = new WorkCleaner();
        var s = wc.CleanByMovement(new DateTime(2024, 6, 18));
        string path = Path.Combine(Environment.CurrentDirectory, "clean", Guid.NewGuid().ToString() + ".log");
        Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "clean")); ;
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.WriteLine(s);    
        }
        
        Debug.WriteLine(path);
        Console.WriteLine(path);
    }

    [Test]
    public async Task CleanTest()
    {
        WorkCleaner wc = new WorkCleaner();
        var s = wc.Clean();
        await EmailNotificatorSingleton.Instance.Send(new MailRequest()
        {
            Bcc = new List<string>(), Body = s, From = "produkt@ksk.ru", CopyTo = new List<string>(),
            IsBodyHtml = false,
            MailAttachments = new List<MailAttachment>(), Subject = "Закрытие", To = new List<string>() {"po@Ksk.ru"}
        });

    }
}