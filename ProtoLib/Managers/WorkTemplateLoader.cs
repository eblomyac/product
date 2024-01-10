using System.Collections.Generic;
using System.Data;
using System.Linq;
using KSK_LIB.Maconomy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
                        $"SELECT ITEMNUMBER, ENTRYTEXT, FINISHEDITEMLOCATION, SUM(NUMBEROF) as numberof from ProductionLine left join ProductionVoucher on ProductionLine.TransactionNumber = ProductionVoucher.TransactionNumber where ProductionLine.TransactionNumber='{order}' group By ItemNumber, ENTRYTEXT, FINISHEDITEMLOCATION");
                foreach (DataRow row in macOrderTable.Rows)
                {
                    WorkCreateTemplate wct = new WorkCreateTemplate();
                    wct.OrderNumber = long.Parse(order);
                    wct.Article = row["ITEMNUMBER"].ToString();
                    wct.Count = int.Parse(row["numberof"].ToString());
                    wct.ProductLine = row["FINISHEDITEMLOCATION"].ToString().Contains("LUMAR") ? "LUMAR" : "SVETON";
                    wct.Description = MaconomyBase.makeStringRu(row["ENTRYTEXT"].ToString());
                    
                    result.Add(wct);
                }

                return result;
            }
            
        }
     
    }
}