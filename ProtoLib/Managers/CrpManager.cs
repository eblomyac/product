using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class CrpManager
    {
        public List<WorkCreateTemplate> LoadWorkData(List<string> articles)
        {
            if (articles.Count == 0)
            {
                return new List<WorkCreateTemplate>();
            }
            using (SqlConnection sql =
                new SqlConnection(Constants.Database.CrpConnectionString))
            {
                string articleData = string.Join(',', articles.Select(x => $"'{x}'"));
                string query =
                    $"SELECT PartId as ItemNumber, Center_Name as post_key , SUM(Dur) as cost from ViewRep_Item_Duration_Posts where PartID in ({articleData}) GROUP BY PartID, Center_Name";

                string query2 =
                    $"SELECT PartId as ItemNumber, Center_Name as post_key ," +
                    $" SUM(Duration) as cost,   (SELECT Oper_Description+CHAR(32)+sub.NPartID+CHAR(32)+ISNULL(sub.NSchema,'')+CHAR(32)+ ISNULL(sub.ProcSymbol,'') +CHAR(13) FROM ViewRep_PartOperations sub WHERE ViewRep_PartOperations.PartId = sub.PartID and sub.Center_Name = ViewRep_PartOperations.Center_Name FOR XML PATH(''), TYPE ).value('.','VARCHAR(MAX)') AS Comments  " +
                    $"from ViewRep_PartOperations where PartId in ({articleData})  GROUP BY PartID, Center_Name";
                SqlDataAdapter sda = new SqlDataAdapter(query2, sql);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                var result = new List<WorkCreateTemplate>();
                foreach (DataRow row in dt.Rows)
                {
                    WorkCreateTemplate wct = new WorkCreateTemplate();
                    wct.Article = row["ItemNumber"].ToString();
                    wct.PostKey = row["post_key"].ToString();
                    wct.SingleCost = decimal.Parse(row["cost"].ToString());
                    wct.Comment = row["Comments"].ToString();
                    //wct.Description = row["Oper_Description"].ToString();
                    result.Add(wct);
                }

                return result;
            }
        }

        public DataTable LoadCardTable(string article)
        {
            using (SqlConnection sql =
                new SqlConnection(Constants.Database.CrpConnectionString))
            {
                

                string query2 =
                    $"SELECT PartId as ItemNumber, Center_Name as post_key ," +
                    $" SUM(Duration) as cost,   (SELECT Oper_Description+CHAR(32)+sub.NPartID+CHAR(32)+ISNULL(sub.NSchema,'')+CHAR(32)+ ISNULL(sub.ProcSymbol,'') +CHAR(13) FROM ViewRep_PartOperations sub WHERE ViewRep_PartOperations.PartId = sub.PartID and sub.Center_Name = ViewRep_PartOperations.Center_Name FOR XML PATH(''), TYPE ).value('.','VARCHAR(MAX)') AS Comments  " +
                    $"from ViewRep_PartOperations where PartId = '{article}'  GROUP BY PartID, Center_Name";

                string query = "SELECT PartId as ItemNumber, Center_Name as post_key," +
                               " Duration as cost, Oper_description as Operation, NPartId as Equipment, ISNULL(NSchema,'') as EquipmentInfo, IsNull(ProcSymbol,'') as Device FROM ViewRep_PartOperations " +
                               $" where ViewRep_PartOperations.PartId = '{article}'";
                SqlDataAdapter sda = new SqlDataAdapter(query, sql);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
        }
    }
}