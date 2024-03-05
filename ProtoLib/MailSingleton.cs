using KSK_LIB.DataStructure.MQRequest;
using KSK_LIB.RabbitMQ.Base;
using KSK_LIB.RabbitMQ.Client;

namespace ProtoLib;

public class MailSender
{
    private Mail Mail;
    public MailSender()
    {
        Mail = new Mail();
        KSKConnection.MakeProducerKSKChain(Mail);
    }

    public async Task Send(MailRequest mr)
    {
        await this.Mail.Send(mr);
    }
}

public class EmailNotificatorSingleton
{
  
    private static MailSender instance = null;
    private static readonly object padlock = new object();

    EmailNotificatorSingleton()
    {
    }

    public static MailSender Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new MailSender();
                }
                return instance;
            }
        }
    }
    
}