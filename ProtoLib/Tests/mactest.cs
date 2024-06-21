using System.Diagnostics;
using DocumentFormat.OpenXml.ExtendedProperties;
using iText.Kernel.Crypto.Securityhandler;
using KSK_LIB.Maconomy;
using NUnit.Framework;
using ProtoLib.Model;

namespace ProtoLib.Tests;

[TestFixture]
public class mactest
{
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