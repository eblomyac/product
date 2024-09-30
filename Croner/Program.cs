// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Net;
using System.Net.Http.Headers;
using KSK_LIB.DataStructure.MQRequest;
using Microsoft.EntityFrameworkCore;
using ProtoLib;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProductCroner
{
    public static class Program
    {
        public static string HtmlRoot = "https://product.ksk.ru";
        public static async Task Main(string[] args)
        {
            if (args.Contains("--maintenance"))
            {
                MaintenanceManager mm = new MaintenanceManager();

                

                try
                {
                    mm.FillWorkCostAndComment();
                }
                catch
                {
                    
                }

                try
                {
                    mm.CheckIssues();
                }
                catch
                {
                    
                }
                try
                {
                    mm.ResolveStackIssues();
                }
                catch
                {
                    
                }
                
                
            //    mm.FillWorkLog();
            }

            if (args.Contains("--week-maintenance"))
            {
                MaintenanceManager mm = new MaintenanceManager(); 
                mm.CleanClosedPrediction();
                
                WorkTemplateLoader wtl = new WorkTemplateLoader();
                wtl.MaconomyProductionDateUpdate();
                
            }
            if (args.Contains("--daily-report"))
            {
                await Report(DateTime.Today);    
            }
            if (args.Contains("--daily-report-yesterday"))
            {
                await Report(DateTime.Today.AddDays(-1));    
            }

            if (args.Contains("--auto-close"))
            {
               // await SyncMac();    
               CloseTodayEnd();
            }
            
            

        }

        public static void CloseTodayEnd()
        {
            WorkManagerFacade wmf = new WorkManagerFacade("system");
            using (BaseContext c = new BaseContext())
            {
                var works = c.Works.Include(x=>x.Post)
                    .Include(x=>x.Issues).AsNoTracking().Where(x => x.Status == WorkStatus.sended && x.Post.CanEnd).ToList();
                foreach (var work in works)
                {
                    var result = wmf.MoveToPostRequest(work.Id, Constants.Work.EndPosts.TotalEnd, new List<string>(),"");    
                }
                
            }
        }

        public static async Task Report(DateTime d)
        {
            ReportManager rm = new ReportManager();
            var request = rm.DailyReportMail(d);
            foreach (var r in request)
            {
                await EmailNotificatorSingleton.Instance.Send(r);    
            }
            
        }

        public static async Task SyncMac()
        {
            
            WorkCleaner wc = new WorkCleaner();
            var s = wc.CleanByMovement(new DateTime(2024,06,18));
            await EmailNotificatorSingleton.Instance.Send(new MailRequest()
            {
                Bcc = new List<string>(), Body = s, From = "produkt@ksk.ru", CopyTo = new List<string>(),
                IsBodyHtml = false,
                MailAttachments = new List<MailAttachment>(), Subject = "ДП. Закрытие сданных работ", To = new List<string>() {"po@Ksk.ru"}
            });
            
        } 
   
    }
    
}
