using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using KSK_LIB.Maconomy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class WorkPrepareGroupResult
    {
        public string Article { get; set; }
        public long OrderNumber { get; set; }
        public int OrderLineNumber { get; set; }

        public decimal TotalCost
        {
            get
            {
                return SingleCost * Count;
            }
        }

        public decimal SingleCost { get; set; }
        public List<string> Comments { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public string ProductionLine { get; set; }
        public List<string> StartPosts { get; set; }
        public string StartOnDefault { get; set; }
        public List<Work> Source { get; set; }

   
        public static List<WorkPrepareGroupResult> GroupForPrepare(List<Work> works)
        {
            var result = new List<WorkPrepareGroupResult>();
            var posts = new List<Post>();
            using (BaseContext c = new BaseContext("system"))
            {
                posts =c.Posts.OrderBy(x => x.ProductOrder).ToList();
            }

            var orderGroup = works.GroupBy(x => x.OrderNumber);
            foreach (var order in orderGroup)
            {
                var orderLine = order.GroupBy(x => x.OrderLineNumber);
                foreach (var line in orderLine)
                {
                    var lineWorks = line.Where(x => !string.IsNullOrEmpty(x.PostId)).ToList();
                    if (lineWorks.Count() == 0)
                    {
                        continue;
                    }
                    decimal cost = lineWorks.Sum(x => x.SingleCost);
                    List<string> comment = lineWorks.SelectMany(x => x.Comments).ToList();

                    
                    var r = new WorkPrepareGroupResult();
                    r.OrderNumber = order.Key;
                    r.OrderLineNumber = line.Key;
                    r.Article = lineWorks.First().Article;
                    r.Description = lineWorks.First().Description;
                    r.ProductionLine = lineWorks.First().ProductLineId;
                    r.Comments = comment;
                    r.SingleCost = cost;
                    r.Count = lineWorks.First().Count;
                    r.StartPosts = lineWorks.OrderBy(x=>x.Post?.ProductOrder).Select(x => x.PostId).ToList();
                    foreach (var p in posts)
                    {
                        if (r.StartPosts.Contains(p.Name))
                        {
                            r.StartOnDefault = p.Name;
                            break;
                        }    
                    }
                    
                    r.Source = lineWorks;
                    result.Add(r);
                }
            }
            
            return result;
        }
        
    }
    public class WorkTemplateLoader
    {
        public List<WorkCreateTemplate> LoadOnlyCrp(List<string> articles)
        {
            CrpManager crp = new CrpManager();
            var workCreateTemplates = crp.LoadAticleDatas(articles);
            
            return workCreateTemplates;
        }
        public List<WorkCreateTemplate> LoadForPostKeys(string orderNumber, int lineNumber, List<string> PostKeys)
        {
            CrpManager crp = new CrpManager();
            var macTemplates = MaconomyPrepare(orderNumber);
            var crpTemplates = crp.LoadWorkDataForPost(macTemplates.Select(x => x.Article).ToList(), PostKeys.Where(x=>!x.Contains(',')).ToList());
            
            List<WorkCreateTemplate> result = new List<WorkCreateTemplate>();
            foreach (var macTemplate in macTemplates.Where(x=>x.OrderLineNumber==lineNumber))
            {
                var articleCrpTemplates = crpTemplates.Where(x => x.Article == macTemplate.Article);

                foreach (var articleDesign in articleCrpTemplates.Where(x=>PostKeys.Contains(x.PostKey)))
                {
                    WorkCreateTemplate wct = new WorkCreateTemplate();
                    wct.OrderLineNumber = macTemplate.OrderLineNumber;
                    wct.OrderNumber = macTemplate.OrderNumber;
                    wct.Article = macTemplate.Article;
                    wct.Description = macTemplate.Description;
                    wct.DeadLine = macTemplate.DeadLine;
                    wct.ProductLine = macTemplate.ProductLine;
                    wct.Count = macTemplate.Count;
                    wct.PostKey = articleDesign.PostKey;
                    wct.SingleCost = articleDesign.SingleCost;
                    wct.Comment = articleDesign.Comment;
                    result.Add(wct);
                }
            }

            return result.ToList();
        }
        public List<WorkCreateTemplate> Load(string orderNumber)
        {
            CrpManager crp = new CrpManager();
            var macTemplates = MaconomyPrepare(orderNumber);
            var crpTemplates = crp.LoadWorkData(macTemplates.Select(x => x.Article).ToList());


            List<WorkCreateTemplate> result = new List<WorkCreateTemplate>();
            foreach (var macTemplate in macTemplates)
            {
                var articleCrpTemplates = crpTemplates.Where(x => x.Article == macTemplate.Article);

                foreach (var articleDesign in articleCrpTemplates)
                {
                    WorkCreateTemplate wct = new WorkCreateTemplate();
                    wct.OrderLineNumber = macTemplate.OrderLineNumber;
                    wct.OrderNumber = macTemplate.OrderNumber;
                    wct.Article = macTemplate.Article;
                    wct.Description = macTemplate.Description;
                    wct.DeadLine = macTemplate.DeadLine;
                    wct.ProductLine = macTemplate.ProductLine;
                    wct.Count = macTemplate.Count;
                    wct.PostKey = articleDesign.PostKey;
                    wct.SingleCost = articleDesign.SingleCost;
                    wct.Comment = articleDesign.Comment;
                    result.Add(wct);
                }
            }
            

            return  result.OrderBy(x => x.Article).ToList();
            
        }
       
        private List<WorkCreateTemplate> MaconomyPrepare(string order)
        {
            using (MaconomyBase mb = new MaconomyBase())
            {
                List<WorkCreateTemplate> result = new List<WorkCreateTemplate>();
                var macOrderTable =
                    mb.getTableFromDB(
                        $"SELECT LINENUMBER, ITEMNUMBER, ENTRYTEXT, PRODUCTIONDATE, FINISHEDITEMLOCATION, NUMBEROF as numberof from ProductionLine left join ProductionVoucher on ProductionLine.TransactionNumber = ProductionVoucher.TransactionNumber where ProductionLine.TransactionNumber='{order}' ");
                foreach (DataRow row in macOrderTable.Rows)
                {
                    WorkCreateTemplate wct = new WorkCreateTemplate();
                    wct.OrderLineNumber = int.Parse(row["LINENUMBER"].ToString());
                    wct.OrderNumber = long.Parse(order);
                    wct.Article = row["ITEMNUMBER"].ToString();
                    wct.Count = int.Parse(row["numberof"].ToString());
                    
                    wct.ProductLine = row["FINISHEDITEMLOCATION"].ToString().Contains("LUMAR") ? "LUMAR" : "SVETON";
                    wct.Description = MaconomyBase.makeStringRu(row["ENTRYTEXT"].ToString());
                    wct.DeadLine = DateTime.ParseExact(row["PRODUCTIONDATE"].ToString(), "yyyy.MM.dd", CultureInfo.InvariantCulture);
                    result.Add(wct);
                }

                return result;
            }
            
        }

        public int MaconomyProductionDateUpdate()
        {
            using (MaconomyBase mb = new())
            {
                using (BaseContext c = new(""))
                {
                    
                    var orders = c.Works.Where(x => x.Status != WorkStatus.ended).Select(x => x.OrderNumber).ToList().Distinct();
                    string orderQuery = String.Join(',',orders.Select(x => $"'{x}'"));
                    var actualData =
                        mb.getTableFromDB(
                            $"SELECT ITEMNUMBER,PRODUCTIONDATE, TransactionNumber from ProductionLine where TransactionNumber in ({orderQuery}) ");

                    var workOrders = c.Works.Where(x => orders.Contains(x.OrderNumber)).ToList();
                    foreach (DataRow r  in actualData.Rows)
                    {
                        var article = r["ITEMNUMBER"].ToString();
                        var order = long.Parse(r["TransactionNumber"].ToString());
                        var date = DateTime.ParseExact(r["PRODUCTIONDATE"].ToString(), "yyyy.MM.dd", CultureInfo.InvariantCulture);

                        var works = workOrders.Where(x => x.OrderNumber == order && x.Article == article);
                        foreach (var w in works)
                        {
                            if (w.DeadLine!=date)
                            {
                                w.DeadLine = date;
                            }
                        }
                    }

                    return c.SaveChanges();
                }
            }
        }
     
    }
}