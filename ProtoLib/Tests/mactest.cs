using System.Diagnostics;
using DocumentFormat.OpenXml.ExtendedProperties;
using iText.Kernel.Crypto.Securityhandler;
using KSK_LIB.Maconomy;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProtoLib.Tests;

[TestFixture]
public class mactest

{
    [Test]
    public async Task TestMacArticles()
    {
     AnalyticManager am = new AnalyticManager();
     var a = await am.OrderStat(544326 , new List<string>());
     int g = 0;
    }
    
    [Test]
    public void Composition()
    {
        TechCardManager tcm = new TechCardManager();
        Console.WriteLine(JsonConvert.SerializeObject(tcm.ItemComposition("V2728-0/3S")));
    }
    [Test]
    public void MemberOf()
    {
        TechCardManager tcm = new TechCardManager();
        Console.WriteLine(JsonConvert.SerializeObject(tcm.ItemPartMemberOf("AV0012P")));
    }
    
    [Test]
    public async Task TestLineCount()
    {
        MaconomyOrderMaxCountManager m = new MaconomyOrderMaxCountManager("543647");
        Console.WriteLine(await m.GetCount(543647 , 1));
        Console.WriteLine(await m.GetCount(543647 , 5));
    }
    
    [Test]
    public void FillComment()
    {
        using (BaseContext c = new BaseContext())
        {
            var works  = c.Works.Include(x=>x.Post).ThenInclude(x=>x.PostCreationKeys).Where(x => x.CommentMap.Length <2 || x.SingleCost==0).ToList();
            var artilces = works.Select(x => x.Article).ToList();
            CrpManager crp = new CrpManager();
            var data = crp.LoadWorkData(artilces);
            int lastIndexOfTwo = 0;
            int index = 0;
            foreach (var work in works)
            {
                var keys = work.Post.PostCreationKeys.Select(x=>x.Key).ToList();
                var workCreateTemplates =data.Where(x => keys.Contains(x.PostKey) && x.Article==work.Article).ToList();
                if (workCreateTemplates.Count > 1)
                {
                    lastIndexOfTwo = index;
                }
                work.Comments = workCreateTemplates.SelectMany(x => x.Comment.Split('\r', StringSplitOptions.RemoveEmptyEntries)).ToList();
                work.SingleCost = workCreateTemplates.Sum(x => x.SingleCost);
                index++;
            }

            c.SaveChanges();
        }
    }
    
    [Test]
    public void FillOrderLine()
    {
        using (BaseContext c = new BaseContext(""))
        {
            using (MaconomyBase mb = new MaconomyBase())
            {
                var works = c.Works.Where(x=>x.OrderLineNumber ==0).ToList();
                var orderGroup = works.GroupBy(x => x.OrderNumber);
                int totalOrders = orderGroup.Count();
                int currentOrder = 1;
                foreach (var og in orderGroup)
                {
                    Debug.WriteLine($"Order: {currentOrder}/{totalOrders+1}");
                    var t = mb.getTableFromDB(
                        $"SELECT LINENUMBER, ITEMNUMBER FROM ProductionLine WHERE TransactionNumber = '{og.Key}' ");

                    var artGroup = og.GroupBy(x => x.Article);
                    
                    foreach (var article in artGroup)
                    {
                        var row = t.Select($"ITEMNUMBER='{article.Key}'");
                        if (row.Length > 0)
                        {
                            foreach (var work in article)
                            {
                                work.OrderLineNumber = int.Parse(row[0]["LINENUMBER"].ToString());
                            }
                        }

                        
                    }
                    currentOrder++;

                    c.SaveChanges();
                }
             
               
            }

            var transfers = c.TransferLines.Where(x => x.OrderLineNumber == 0).ToList();
            var transferWorks = c.Works.Where(x => transfers.Select(x => x.SourceWorkId).Contains(x.Id)).ToList();

            int currentCount = 0;
            foreach (var w in transferWorks)
            {
                var workTransfers = transfers.Where(x => x.SourceWorkId == w.Id).ToList();
                foreach (var workTransfer in workTransfers)
                {
                    workTransfer.OrderLineNumber = w.OrderLineNumber;
                }

                c.SaveChanges();
                currentCount++;
                Debug.WriteLine($"{currentCount}\\{transferWorks.Count}");
            }
            
        }
    }
    
    
}