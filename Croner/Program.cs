﻿// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using KSK_LIB.DataStructure.MQRequest;
using KSK_LIB.Excel;
using Microsoft.EntityFrameworkCore;
using ProtoLib;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProductCroner
{
    
    public static class Program
    {
        public async static Task CheckCrp()
        {
            PlanChecker pc = new PlanChecker();

            var check = pc.Check();

            var lumarData = pc.CheckReport(check, "LUMAR");
            var svetonData = pc.CheckReport(check, "SVETON");

            string lumarFile = Path.Combine(Environment.CurrentDirectory, "lumar-data.xlsx");
            string svetonFile = Path.Combine(Environment.CurrentDirectory, "sveton-data.xlsx");

            if (File.Exists(lumarFile))
            {
                File.Delete(lumarFile);
            }

            if (File.Exists(svetonFile))
            {
                File.Delete(svetonFile);
            }
            
            ExcelExporter ee = new ExcelExporter(lumarFile);
            ee.ExportSet(lumarData);

            ExcelExporter ee2 = new ExcelExporter(svetonFile);
            ee2.ExportSet(svetonData);
            
            await EmailNotificatorSingleton.Instance.Send(new MailRequest()
            {
                Bcc =new List<string>() {"po@Ksk.ru"}, Body = "Во вложении отчет с расхождениями нормативов между макономи и crp, товаров LUMAR", From = "produkt@ksk.ru", CopyTo = new List<string>(),
                IsBodyHtml = false,
                MailAttachments = new List<MailAttachment>(){new MailAttachment(lumarFile)}, Subject = "Проверка нормативов Maconomy\\CRP (LUMAR)", 
                //To = new List<string>(){"po@ksk.ru"}
                 To = new List<string>() {"Konstantin.Iurev@ksk.ru","Alexey.Sarmak@vitaluce.ru","artur.vagapov@ksk.ru","Anatoliy.Kalinichenko@vitaluce.ru","Oleg.Topalov@vitaluce.ru","Victor.Romanchuk@vitaluce.ru"}
            });  
            await EmailNotificatorSingleton.Instance.Send(new MailRequest()
            {
                Bcc = new List<string>() {"po@Ksk.ru"}, Body = "Во вложении отчет с расхождениями нормативов между макономи и crp, товаров SVETON", From = "produkt@ksk.ru", CopyTo = new List<string>(),
                IsBodyHtml = false,
                MailAttachments = new List<MailAttachment>(){new MailAttachment(svetonFile)}, Subject = "Проверка нормативов Maconomy\\CRP (SVETON)", 
             //   To = new List<string>(){"po@ksk.ru"}
               To = new List<string>() {"Konstantin.Iurev@ksk.ru","Alexey.Sarmak@vitaluce.ru","stocks@sveton.ru","Nikolay.Rezenov@sveton.ru","artur.vagapov@ksk.ru","Anatoliy.Kalinichenko@vitaluce.ru","Oleg.Topalov@vitaluce.ru","Victor.Romanchuk@vitaluce.ru"}
            });
            
        }
        public static string HtmlRoot = "https://product.ksk.ru";
        public static async Task Main(string[] args)
        {
            if (args.Contains("--make-all-time-report"))
            {
                
                await MakeAllReportHistory(DateTime.ParseExact(args[1], "dd.MM.yyyy", CultureInfo.InvariantCulture));
                return;
            }
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
               AnalyticManager am = new AnalyticManager();
                var rep = await am.CostReport();
                am.SaveTodayCostReport(rep);
                await StoreYesterdayReport();
            }

            if (args.Contains("--auto-close"))
            {
               // await SyncMac();    
               CloseTodayEnd();
            }

            if (args.Contains("--check-crp"))
            {
               await CheckCrp();
                
            }
            

        }

        public static async Task StoreYesterdayReport()
        {
            ReportManager rm = new ReportManager();

            var request = await rm.PeriodReport(DateTime.Today.AddDays(-1),
                DateTime.Today.AddDays(-1), true, true);
    
            rm.StoreDailyReports(request);
            var s = rm.DailyReportsToExcel(new DateTime(2024,1,1), DateTime.Today.AddDays(-1));
            ExcelExporter ee = new ExcelExporter(Constants.FileStorage.ReportFullFileName);
            ee.ExportSet(s);
            
        }

        public static async Task MakeAllReportHistory(DateTime start)
        {
            ReportManager rm = new ReportManager();
           
            while (start < DateTime.Today)
            {
                Console.WriteLine($"Taking report from: {start:dd.MM.yyyy}");
                var request = await rm.PeriodReport(start,
                    start, true,true);
                Console.WriteLine("Saving");
                rm.StoreDailyReports(request);
                start = start.AddDays(1);
            }
            var s = rm.DailyReportsToExcel(new DateTime(2024,1,1), DateTime.Today.AddDays(-1));
            ExcelExporter ee = new ExcelExporter(Constants.FileStorage.ReportFullFileName);
            ee.ExportSet(s);
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
                    var result = wmf.MoveToPostRequest(work.Id, Constants.Work.EndPosts.TotalEnd, new List<string>(),"", out  var errorInfo);    
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
                Bcc = new List<string>(), Body = s, From = "product@ksk.ru", CopyTo = new List<string>(),
                IsBodyHtml = false,
                MailAttachments = new List<MailAttachment>(), Subject = "ДП. Закрытие сданных работ", To = new List<string>() {"po@Ksk.ru"}
            });
            
        } 
   
    }
    
}
