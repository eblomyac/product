using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using KSK_LIB.Maconomy;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto.Parameters;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class TechCardManager
    {
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

    }
}