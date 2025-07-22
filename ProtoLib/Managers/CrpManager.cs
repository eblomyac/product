using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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

        public class CrpTarget
        {
            public string TargetName {
                get
                {
                    return $"{TargetCrpCenter}-{TargetCrpPost}-{TargetCrpPostDescription}";
                }}
    
           
            public string TargetCrpCenter { get; set; }
           
            public string TargetCrpPost { get; set; }
    
         
            public string TargetCrpCenterDescription { get; set; }
           
            public string TargetCrpPostDescription { get; set; }

        }
        public Dictionary<string,List<CrpTarget>> GetTargetHrList(List<string> keys)
        {
            Dictionary<string, List<CrpTarget>> result = new();
            string q =string.Join(',', keys.Select(x => "'" + x + "'"));
            string query = $"SELECT Center_Name as cn, Post_Name as pn, Center_Description as cd, Post_Description as pd FROM tbl_Centers left join tbl_Posts on tbl_Centers.IDC = tbl_Posts.IDC  where Center_Name in ({q}) order by tbl_Centers.SortNum";
            using (SqlConnection sql =
                   new SqlConnection(Constants.Database.CrpConnectionString))
            {
                SqlDataAdapter sda = new SqlDataAdapter(query, sql);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    if(!result.ContainsKey(row[0].ToString()))
                    {
                        result.Add(row[0].ToString(), new List<CrpTarget>());
                    }

                    CrpTarget ct = new CrpTarget();
                    ct.TargetCrpCenterDescription = row["cd"].ToString();
                    ct.TargetCrpPostDescription = row["pd"].ToString();
                    ct.TargetCrpCenter = row["cn"].ToString();
                    ct.TargetCrpPost = row["pn"].ToString();
                    
                    result[row[0].ToString()].Add(ct);
                }

                return result;
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
            string s = "SELECT DISTINCT (ViewRep_Item_Duration_Posts.PartID) as Article" +
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
                SqlCommand sc = new SqlCommand(s, sql);
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
        public List<List<double>> CostInfoBatch(List<string >articles)
        {
            using (SqlConnection sql =
                   new SqlConnection(Constants.Database.CrpConnectionString))
            {

                var queryParams = string.Join(",", articles.Select((x, i) => $"@art{i}"));
                string s = "SELECT ViewRep_Item_Duration_Posts.PartID as Article," +
                           " ViewRep_Item_Duration_Posts.Center_Name as Post," +
                           " SUM(ViewRep_Item_Duration_Posts.Dur) as Duration" +
                           $" FROM crp.dbo.ViewRep_Item_Duration_Posts ViewRep_Item_Duration_Posts  where PartId in({queryParams}) Group By PartId,Center_Name";
                SqlCommand sc = new SqlCommand(s, sql);
                foreach (var art in articles)
                {
                    SqlParameter articleParam = new SqlParameter($"@art{sc.Parameters.Count}", SqlDbType.NVarChar);
                    articleParam.Value = art;
                    sc.Parameters.Add(articleParam);    
                }
                
                SqlDataAdapter sda = new SqlDataAdapter(sc);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                if (post_list.Count == 0)
                {
                    this.LoadPostList();
                }


                List<List<double>> allResult = new();
                foreach (var art in articles)
                {
                    List<double> result = new List<double>();
                    foreach (var post in post_list)
                    {
                        DataRow[] cost = dt.Select($"Post='{post}' AND Article='{art}'");
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
                    allResult.Add(result);
                }
                
                return allResult;
            }
        }

        public List<WorkCreateTemplate> LoadAticleDatas(List<string> articles)
        {
            int loop = 0;
            List<WorkCreateTemplate> result = new();
            do
            {
                var part = articles.Skip(999 * loop).Take(999).ToList();
                if (part.Count == 0)
                {
                    return result;
                }
                result.AddRange(_loadArticleDatas(part));
                loop++;
            } while (true);
        }
        private List<WorkCreateTemplate> _loadArticleDatas(List<string> articles)
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
                    decimal sc = 0;
                    decimal.TryParse(row["cost"].ToString(), out sc);
                    wct.SingleCost = sc;
                    wct.Comment = row["Comments"].ToString();
                    //wct.Description = row["Oper_Description"].ToString();
                    result.Add(wct);
                }

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
                    decimal val = 0;
                    decimal.TryParse(row["cost"].ToString(), out val);
                    wct.SingleCost =(val);
                    wct.Comment = row["Comments"].ToString();
                    //wct.Description = row["Oper_Description"].ToString();
                    result.Add(wct);
                }

                return result;
            }
        }

        public List<WorkCreateTemplate> LoadWorkDataForPost(List<string> articles, List<string> PostKeys)
        {
            if (articles.Count == 0)
            {
                return new List<WorkCreateTemplate>();
            }

            using (SqlConnection sql =
                   new SqlConnection(Constants.Database.CrpConnectionString))
            {
                string articleData = string.Join(',', articles.Select(x => $"'{x}'"));
                string postKeys = string.Join(',', PostKeys.Select(x => $"'{x}'"));
                string query =
                    $"SELECT PartId as ItemNumber, Center_Name as post_key , SUM(Dur) as cost from ViewRep_Item_Duration_Posts where PartID in ({articleData}) GROUP BY PartID, Center_Name";

                string query2 =
                    $"SELECT PartId as ItemNumber , Center_Name as post_key," +
                    $" SUM(Duration) as cost,   (SELECT Oper_Description+CHAR(32)+sub.NPartID+CHAR(32)+ISNULL(sub.NSchema,'')+CHAR(32)+ ISNULL(sub.ProcSymbol,'') +CHAR(13) FROM ViewRep_PartOperations sub WHERE ViewRep_PartOperations.PartId = sub.PartID and sub.Center_Name = ViewRep_PartOperations.Center_Name FOR XML PATH(''), TYPE ).value('.','VARCHAR(MAX)') AS Comments  " +
                    $"from ViewRep_PartOperations where PartId in ({articleData}) and Center_Name in ({postKeys}) GROUP BY PartID, Center_Name";
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

                List<WorkCreateTemplate> newResult = new();
                foreach (var art in articles)
                {
                    var toGroup = result.Where(x => x.Article == art);
                    if (toGroup.Count() > 0)
                    {
                        newResult.Add(new WorkCreateTemplate()
                        {
                            Article = art,
                            Comment = string.Join('\r', toGroup.Select(x => x.Comment)),
                            SingleCost = toGroup.Sum(x => x.SingleCost),
                            PostKey = toGroup.First().PostKey
                        });
                    }
                }

                return newResult;
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