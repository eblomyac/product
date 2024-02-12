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
    public class WorkTemplateLoader
    {
        public List<WorkCreateTemplate> Load(string orderNumber)
        {
            CrpManager crp = new CrpManager();
            var macTemplates = MaconomyPrepare(orderNumber);
            var crpTemplates = crp.LoadWorkData(macTemplates.Select(x => x.Article).ToList());

            foreach (var crpTemplate in crpTemplates)
            {
                var macTemplate = macTemplates.First(x => x.Article == crpTemplate.Article);
                crpTemplate.OrderNumber = macTemplate.OrderNumber;
                crpTemplate.Count = macTemplate.Count;
                crpTemplate.Description = macTemplate.Description;
                crpTemplate.ProductLine = macTemplate.ProductLine;
                crpTemplate.DeadLine = macTemplate.DeadLine;
            }

            return  crpTemplates.OrderBy(x => x.Article).ToList();
            
        }

        private List<WorkCreateTemplate> MaconomyPrepare(string order)
        {
            using (MaconomyBase mb = new MaconomyBase())
            {
                List<WorkCreateTemplate> result = new List<WorkCreateTemplate>();
                var macOrderTable =
                    mb.getTableFromDB(
                        $"SELECT ITEMNUMBER, ENTRYTEXT, PRODUCTIONDATE, FINISHEDITEMLOCATION, SUM(NUMBEROF) as numberof from ProductionLine left join ProductionVoucher on ProductionLine.TransactionNumber = ProductionVoucher.TransactionNumber where ProductionLine.TransactionNumber='{order}' group By ItemNumber, ENTRYTEXT, FINISHEDITEMLOCATION, PRODUCTIONDATE");
                foreach (DataRow row in macOrderTable.Rows)
                {
                    WorkCreateTemplate wct = new WorkCreateTemplate();
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
                using (BaseContext c = new())
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