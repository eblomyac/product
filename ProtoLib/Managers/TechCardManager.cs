using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Text;
using KSK_LIB.Maconomy;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto.Parameters;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class TechCardManager
    {
        private string articleCacheFileName = "article_cache.data";
        public List<string> ArticleList()
        {
            string cacheFile = Path.Combine(Environment.CurrentDirectory, articleCacheFileName);
            if (File.Exists(cacheFile))
            {
                DateTime lastWrite = File.GetLastWriteTime(cacheFile);
                if ((DateTime.Now - lastWrite).Hours < 1)
                {
                    return File.ReadAllLines(articleCacheFileName).ToList();
                }
                
            }
            //CrpManager crpManager = new CrpManager();
            //crpManager.CrpArticles();
            using (MaconomyBase mb = new MaconomyBase())
            {
                var dt = mb.getTableFromDB("SELECT ITEMNUMBER FROM ITEMINFORMATION");
                List<string> list = dt.Rows.OfType<DataRow>()
                    .Select(dr => dr.Field<string>(0)).ToList();
                File.WriteAllLines(cacheFile, list);
                return list;
            }

        }
        
        public static string rootImagesDir = "\\\\kweb.kck2.ksk.ru\\h$\\product.ksk.ru";
        public TechCard? Get(string identity)
        {
            using (BaseContext c = new BaseContext(""))
            {
                return c.TechCards
                    .Include(x => x.PostParts).ThenInclude(x => x.Lines)
                    .Include(x => x.PostParts)
                    .Include(x=>x.Images)
                    .FirstOrDefault(x => x.Article == identity);

            }
        }

        public TechCard? GetFromCrp(string identity)
        {
            DataTable maconomyData;
            using (MaconomyBase mb = new MaconomyBase())
            {
                maconomyData = mb.getTableFromDB($"SELECT ItemInformation.ItemNumber, ITEMPROPERTYLINE.PROPERTYNAME," +
                                                 $" ITEMPROPERTYLINE.REMARK1  from ItemInformation left join ItemPropertyLine on ItemInformation.ItemNumber = ItemPropertyLine.ItemNumber " +
                                                 $"where lower(ItemInformation.Itemnumber)=lower('{identity}') or lower(ItemInformation.Barcode)=lower('{identity}')");
            }

            if (maconomyData.Rows.Count == 0)
            {
                
                return null;
            }
            string article = maconomyData.Rows[0]["ItemNumber"].ToString();
            var imageRow = maconomyData.Select("PROPERTYNAME='Photo HQ'");
            string imagePath = "https://img.ksk.ru/ArticlesPhotos/na.jpg";
            if (imageRow.Length > 0)
            {
                imagePath = imageRow[0][2].ToString();
            }
            CrpManager crp = new CrpManager();
            var crpData = crp.LoadCardTable(article);
            if (crpData.Rows.Count == 0)
            {
                return null;
            }
            
            Dictionary<string, List<TechCardLine>> CrpLines = new();
            
            
            foreach (DataRow row in crpData.Rows)
            {
                TechCardLine tcl = new TechCardLine();
                tcl.IsCustom = false;
                tcl.Cost = decimal.Parse(row["cost"].ToString());
                tcl.Device = row["Device"].ToString();
                tcl.Equipment = row["Equipment"].ToString();
                tcl.EquipmentInfo = row["EquipmentInfo"].ToString();
                tcl.Operation = row["Operation"].ToString();


                string postKey = row["post_key"].ToString();
                if (CrpLines.ContainsKey(postKey))
                {
                    CrpLines[postKey].Add(tcl);
                }
                else
                {
                    CrpLines.Add(postKey,new List<TechCardLine>(){tcl});
                }
            }

            TechCard tc = new TechCard();
            
          
            tc.Images = new List<StoredImage>();
            StoredImage si = new StoredImage();
            if (imagePath.Contains("/na.jpg"))
            {  si.Description = "Фотография не доступна";
                
            }
            else
            {
                si.Description = "Фотография изделия";
            }
          
            si.Url = imagePath;
            
            
            tc.Images.Add(si);
            tc.Article = article;
            tc.PostParts = new List<TechCardPost>();

            using (BaseContext d = new BaseContext(""))
            {
                var posts = d.Posts.Include(x => x.PostCreationKeys).ToList();
                
                foreach (var postKey in CrpLines.Keys)
                {
                    var p = d.PostKeys.FirstOrDefault(x => x.Key == postKey);
                    if (p == null)
                    {
                        continue;
                    }
                    var existPart = tc.PostParts.FirstOrDefault(x => x.PostId == p.PostId);
                    if (existPart == null)
                    {
                        TechCardPost tcp = new TechCardPost();
                        tcp.Lines = CrpLines[postKey];
                        tcp.PostId = p.PostId;
                        tcp.Post =posts.FirstOrDefault(x => x.Name == p.PostId);
                        tc.PostParts.Add(tcp);
                    }
                    else
                    {
                        existPart.Lines.AddRange(CrpLines[postKey]);
                    }
                }
            }

            tc.PostParts = tc.PostParts.OrderBy(x => x.Post.ProductOrder).ToList();
            return tc;
        }
        

        public TechCard LoadAdditionalLocal(TechCard tc)
        {
            if (tc == null)
            {
                return null;
            }
            var localCard = Get(tc.Article);
            if (localCard == null)
            {
                return tc;
            }
            else
            {
                tc.Id = localCard.Id;
                if (localCard.Images != null && localCard.Images.Count>0)
                {
                    foreach (var img in localCard.Images)
                    {
                        tc.Images.Add(img);
                    }
                }

                foreach (var localPost in localCard.PostParts)
                {
                    var toAdd = tc.PostParts.FirstOrDefault(x => x.PostId == localPost.PostId);
                    if (toAdd == null)
                    {
                        tc.PostParts.Add(localPost);
                    }
                    else
                    {
                        toAdd.Id = localPost.Id;
                      

                        foreach (var localLine in toAdd.Lines)
                        {
                            toAdd.Lines.Add(localLine);
                        }
                    }
                }
            }

            return tc;
        }

     
        public bool SavePhotoData(string article, string initialFileName, string userId, string postId,
            string description, string localPath)
        {
            using (BaseContext b = new BaseContext())
            {
                var post =b.Posts.FirstOrDefault(x=>x.Name == postId);
                var existTechCard = b.TechCards.Include(x=>x.Images).FirstOrDefault(x => x.Identity == article);
                if (existTechCard == null)
                {
                    existTechCard = new TechCard();
                    existTechCard.Identity = article;
                    existTechCard.Article = article;
                    existTechCard.Id = Guid.NewGuid();
                    existTechCard.Description = "";
                    existTechCard.Images = new List<StoredImage>();
                    b.TechCards.Add(existTechCard);
                    b.SaveChanges();

                }
                
                StoredImage img = new StoredImage();
                img.PostId = post == null ? "" : post.Name;
                img.Description = post==null?description:$"{post.Name}: {description}";
               
                img.LocalPath = localPath;
                img.InitialFileName = initialFileName;
                img.Id = Guid.NewGuid();
                img.UploadedBy = userId;
                img.TechCardId = existTechCard.Id;
                img.Url = localPath.Replace(rootImagesDir,"/CustomImage").Replace("\\","/");

                b.Images.Add(img);
                return b.SaveChanges()>0;
            }
        }
        public string UploadCustomPhoto(string article,string fileName, string fileData, string postId, string description)
        {
            string dirFolder = Path.Combine(Environment.CurrentDirectory, "uploaded-image-temp");
            Directory.CreateDirectory(dirFolder);
            
            string imageData = fileData.Substring(fileData.IndexOf(',')+1);
            string tempFileName = Path.Combine(dirFolder, fileName);
            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }
            File.WriteAllBytes(tempFileName, Convert.FromBase64String(imageData));


            Image image = Image.FromFile(tempFileName);

            while (image.Width>2000 || image.Height>2000)
            {
                Bitmap b = new Bitmap(image.Width/2, image.Height/2);
              
                Graphics g = Graphics.FromImage(b);
                g.DrawImage(image, 0,0, image.Width/2, image.Height/2);
                image.Dispose();
                image = b;
                if (File.Exists(tempFileName))
                {
                    File.Delete(tempFileName);
                }
                image.Save(tempFileName, ImageFormat.Jpeg);
                g.Dispose();
            }
            image.Dispose();

            string normalizedArticleName = article;
            foreach (char replaceChar in Path.GetInvalidFileNameChars())
            {
                normalizedArticleName = normalizedArticleName.Replace(replaceChar, '_');
            }
            string fileStorageName = Path.Combine(rootImagesDir,normalizedArticleName);
            Directory.CreateDirectory(fileStorageName);
            using (BaseContext b = new BaseContext())
            {
                var post = b.Posts.FirstOrDefault(x => x.Name == postId);
                if (post != null)
                {
                    fileStorageName = Path.Combine(fileStorageName, post.Name);
                    Directory.CreateDirectory(fileStorageName);
                }

                int fileCount = Directory.GetFiles(fileStorageName).Length;
                fileStorageName = Path.Combine(fileStorageName, fileCount + ".jpg");
            }
            File.Move(tempFileName, fileStorageName);
            return fileStorageName;
        }

        public DataTable ItemComposition(string article)
        {
            using (MaconomyBase mb = new MaconomyBase())
            {
                string query = @"SELECT BOMPART.LINENUMBER, BOMPART.BOMITEMPARTNUMBER, BOMPART.NUMBEROF, POPUPITEM.NAME, ITEMINFORMATION.SUPPLEMENTARYTEXT4,
       ITEMINFORMATION.PICTUREREFERENCE, WAREHOUSEINFORMATION.AVAILABLEINVENTORY,  INVENTORY.INVENTORYNAME
FROM BOMPART
    left join ItemInformation on BOMPART.BOMITEMPARTNUMBER = ITEMINFORMATION.ITEMNUMBER
    left join popupitem on ITEMINFORMATION.ITEMPOPUP3 = popupitem.POPUPITEMNUMBER and POPUPTYPENAME='ItemPopupType3'
    left join WAREHOUSEINFORMATION on BOMITEMPARTNUMBER=WAREHOUSEINFORMATION.ITEMNUMBER  and WAREHOUSEINFORMATION.AVAILABLEINVENTORY>0
    left join INVENTORY on INVENTORY.INVENTORYNUMBER = WAREHOUSEINFORMATION.INVENTORYNUMBER where BOMPART.ITEMNUMBER=:article and BOMPART.BOM=1 ORDER BY BOMPART.LINENUMBER";
                OracleCommand oc = new OracleCommand(query);
                oc.Parameters.Add(new OracleParameter("article", $"{article}"));
                
                var table = mb.getTableFromDB(oc);

                DataTable t = new DataTable();
                t.Columns.Add("item");
                t.Columns.Add("itemName");
                t.Columns.Add("type");
                t.Columns.Add("count", typeof(decimal));
                t.Columns.Add("image");
                t.Columns.Add("stock");
                
                for (int loop = 1; loop <= table.Rows.Count+1; loop++)
                {
                    var lineRows = table.Select($"LINENUMBER='{loop}'");
                    if (lineRows.Length == 0)
                    {
                       continue;
                    }
                    else
                    {
                        
                        DataRow newRow = t.NewRow();
                        newRow[0] = lineRows[0]["BOMITEMPARTNUMBER"].ToString();
                        newRow[1] = MaconomyBase.makeStringRu(lineRows[0]["SUPPLEMENTARYTEXT4"].ToString());
                        newRow[2] =  MaconomyBase.makeStringRu(lineRows[0]["NAME"].ToString());
                        newRow[3] = lineRows[0]["NUMBEROF"];
                        newRow[4] = lineRows[0]["PICTUREREFERENCE"].ToString();
                        List<string> sb = new(); 
                        foreach (var invRow in lineRows)
                        {
                            sb.Add($"{MaconomyBase.makeStringRu(invRow["INVENTORYNAME"].ToString())}: { invRow["AVAILABLEINVENTORY"]}");
                        }

                        newRow[5] = string.Join("\r\n",sb);
                        t.Rows.Add(newRow);
                    }
                    
                }

                return t;
            }
        }

        public DataTable ItemPartMemberOf(string article)
        {
            using (MaconomyBase mb = new MaconomyBase())
            {
                string query = @"SELECT BOMPART.BOMITEMPARTNUMBER, BOMPART.NUMBEROF, piBom.NAME, bomInfo.SUPPLEMENTARYTEXT4, BomPart.ITEMNUMBER,
       WAREHOUSEINFORMATION.AVAILABLEINVENTORY,  INVENTORY.INVENTORYNAME, articleInfo.SUPPLEMENTARYTEXT4 , piArtcile.NAME
FROM BOMPART
    left join ItemInformation bomInfo on BOMPART.BOMITEMPARTNUMBER = bomInfo.ITEMNUMBER
    left join popupitem piBom on bomInfo.ITEMPOPUP3 = piBom.POPUPITEMNUMBER and POPUPTYPENAME='ItemPopupType3'
    left join ITEMINFORMATION articleInfo on BOMPART.ITEMNUMBER = articleInfo.ITEMNUMBER
    left join popupitem piArtcile on articleInfo.ITEMPOPUP3 = piArtcile.POPUPITEMNUMBER and piArtcile.POPUPTYPENAME='ItemPopupType3'
    left join WAREHOUSEINFORMATION on BOMITEMPARTNUMBER=WAREHOUSEINFORMATION.ITEMNUMBER  and WAREHOUSEINFORMATION.AVAILABLEINVENTORY>0
    left join INVENTORY on INVENTORY.INVENTORYNUMBER = WAREHOUSEINFORMATION.INVENTORYNUMBER
where BOMPART.BOMITEMPARTNUMBER=:article and BOMPART.BOM=1 ORDER BY BOMPART.LINENUMBER";
                OracleCommand oc = new OracleCommand(query);
                oc.Parameters.Add(new OracleParameter("article", $"{article}"));

                var table = mb.getTableFromDB(oc);

                DataTable t = new DataTable();
                t.Columns.Add("item");
                t.Columns.Add("itemName");
                t.Columns.Add("itemType");
                t.Columns.Add("count", typeof(decimal));
                t.Columns.Add("partOf");
                t.Columns.Add("partOfName");
                t.Columns.Add("partOfType");
                t.Columns.Add("stock");

                var arts = table.DefaultView.ToTable(true, "ITEMNUMBER");
                foreach (DataRow artRow in arts.Rows)
                {
                    var lineRows = table.Select($"ITEMNUMBER='{artRow[0]}'");
                    if (lineRows.Length == 0)
                    {
                        continue;
                    }
                    else
                    {
                        
                        DataRow newRow = t.NewRow();
                        newRow[0] = lineRows[0]["BOMITEMPARTNUMBER"].ToString();
                        newRow[1] = MaconomyBase.makeStringRu(lineRows[0]["SUPPLEMENTARYTEXT4"].ToString());
                        newRow[2] = MaconomyBase.makeStringRu(lineRows[0]["NAME"].ToString());
                        newRow[3] = lineRows[0]["NUMBEROF"];
                        newRow[4] = lineRows[0]["ITEMNUMBER"].ToString();
                        newRow[5] = MaconomyBase.makeStringRu(lineRows[0]["SUPPLEMENTARYTEXT41"].ToString());
                        newRow[6] = MaconomyBase.makeStringRu(lineRows[0]["NAME1"].ToString());
                    
                        List<string> sb = new(); 
                        foreach (var invRow in lineRows)
                        {
                            sb.Add($"{MaconomyBase.makeStringRu(invRow["INVENTORYNAME"].ToString())}: { invRow["AVAILABLEINVENTORY"]}");
                        }

                        newRow[7] = string.Join("\r\n",sb);
                        t.Rows.Add(newRow);
                    }    
                }
                

                return t;
            }
        }

    }
}