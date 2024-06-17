using KSK_LIB.DataStructure.MQRequest;
using NUnit.Framework;
using ProtoLib.Managers;

namespace ProtoLib.Tests;

[TestFixture]
public class CleanWorkTest
{
    [Test]
    public async Task ErrorRemake()
    {
        WorkCleaner wc = new WorkCleaner();
        wc.RemoveError();

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