using System.Diagnostics;
using KSK_LIB.DataStructure.MQRequest;
using NUnit.Framework;
using ProtoLib.Managers;

namespace ProtoLib.Tests;

[TestFixture]
public class CleanWorkTest

{
    [Test]
    public void Resolve()
    {
        IssueManager im = new IssueManager();
        im.ResolveIssue(692, "system");
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