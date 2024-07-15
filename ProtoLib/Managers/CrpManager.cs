using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class CrpManager
    {
        private List<string> post_list = new List<string>();

        private void LoadPostList()
        {
            using (SqlConnection sql =
                   new SqlConnection(Constants.Database.CrpConnectionString))
            {
                string s2 = "SELECT Center_Name, SortNum from crp.dbo.tbl_Centers order by sortNum";
                SqlDataAdapter sda2 = new SqlDataAdapter(s2, sql);
                DataTable posts = new DataTable();
                sda2.Fill(posts);
                post_list = posts.Rows.OfType<DataRow>().Where(dr => dr.Field<short?>(1).HasValue)
                    .Select(x => x.Field<string>(0)).ToList();
                post_list.AddRange(posts.Rows.OfType<DataRow>().Where(dr => !dr.Field<short?>(1).HasValue)
                    .Select(x => x.Field<string>(0)).ToList());
            }
        }
        public List<string> CrpPosts()
        {
            if (post_list.Count == 0)
            {
                this.LoadPostList();
                
            }
            return this.post_list;


        }
        public List<string> CrpArticles()
        {
            string s = "SELECT DISTINCT (ViewRep_Item_Duration_Posts.PartID) as Article"  +
                       " FROM crp.dbo.ViewRep_Item_Duration_Posts ViewRep_Item_Duration_Posts";
            using (SqlConnection sql =
                   new SqlConnection(Constants.Database.CrpConnectionString))
            {
                SqlDataAdapter sda = new SqlDataAdapter(s, sql);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                List<string> list = dt.Rows.OfType<DataRow>()
                    .Select(dr => dr.Field<string>(0)).ToList();
                return list;
            }
        }
        public List<double> CostInfo(string article)
        {
            using (SqlConnection sql =
                   new SqlConnection(Constants.Database.CrpConnectionString))
            {
                string s = "SELECT ViewRep_Item_Duration_Posts.PartID as Article," +
                           " ViewRep_Item_Duration_Posts.Center_Name as Post," +
                           " SUM(ViewRep_Item_Duration_Posts.Dur) as Duration" +
                           $" FROM crp.dbo.ViewRep_Item_Duration_Posts ViewRep_Item_Duration_Posts  where PartId=@Article Group By PartId,Center_Name";
                SqlCommand sc = new SqlCommand(s,sql);
                SqlParameter articleParam = new SqlParameter("@Article", SqlDbType.NVarChar);
                articleParam.Value = article;
                sc.Parameters.Add(articleParam);
                SqlDataAdapter sda = new SqlDataAdapter(sc);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                
                if (post_list.Count == 0)
                {
                    this.LoadPostList();
                }
                

                List<double> result = new List<double>();
                foreach (var post in post_list)
                {
                    DataRow[] cost = dt.Select($"Post='{post}'");
                    if (cost.Length == 0)
                    {
                        result.Add(0);
                    }
                    else
                    { 
                        result.Add(cost[0].Field<double>("Duration"));
                    }
                }
                result.Add(result.Sum());
                return result;

            }

        }
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