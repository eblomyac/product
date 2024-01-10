using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using KSK_LIB.Maconomy;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class TechCardManager
    {
        public TechCard? Get(string identity)
        {
            using (BaseContext c = new BaseContext())
            {
                return c.TechCards
                    .Include(x => x.PostParts).ThenInclude(x => x.Lines)
                    .Include(x => x.PostParts).ThenInclude(x => x.ImageSet)
                    .Include(x => x.ImageSet)
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
            string imagePath = "http://www.kckelectro.ru/KSKSITEPICS/Pictures/na.jpg";
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
            
            ImageSet s = new ImageSet();
            s.Images = new List<StoredImage>();
            StoredImage si = new StoredImage();
            si.Description = "Фотография изделия";
            si.Url = imagePath;
            
            
            s.Images.Add(si);
            tc.ImageSet =s;
            tc.Article = article;
            tc.PostParts = new List<TechCardPost>();

            using (BaseContext d = new BaseContext())
            {
                foreach (var postKey in CrpLines.Keys)
                {
                    var p = d.PostKeys.FirstOrDefault(x => x.Key == postKey);
                    var existPart = tc.PostParts.FirstOrDefault(x => x.PostId == p.PostId);
                    if (existPart == null)
                    {
                        TechCardPost tcp = new TechCardPost();
                        tcp.Lines = CrpLines[postKey];
                        tcp.PostId = p.PostId;
                        tc.PostParts.Add(tcp);
                    }
                    else
                    {
                        existPart.Lines.AddRange(CrpLines[postKey]);
                    }
                }
            }
            return tc;
        }

        public TechCard LoadAdditionalLocal(TechCard tc)
        {
            var localCard = Get(tc.Article);
            if (localCard == null)
            {
                return tc;
            }
            else
            {
                tc.Id = localCard.Id;
                if (localCard.ImageSet != null)
                {
                    foreach (var img in localCard.ImageSet.Images)
                    {
                        tc.ImageSet.Images.Add(img);
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
                        if (localPost.ImageSet != null)
                        {
                            if (toAdd.ImageSet == null)
                            {
                                toAdd.ImageSet = new ImageSet();
                            }

                            if (toAdd.ImageSet.Images == null)
                            {
                                toAdd.ImageSet.Images = new List<StoredImage>();
                            }
                            foreach (var img in localPost.ImageSet.Images)
                            {
                                toAdd.ImageSet.Images.Add(img);
                            }
                        }

                        foreach (var localLine in toAdd.Lines)
                        {
                            toAdd.Lines.Add(localLine);
                        }
                    }
                }
            }

            return null;
        }

        public TechCard GetWithCrp(string identity)
        {
            DataTable maconomyData;
            using (MaconomyBase mb = new MaconomyBase())
            {
                maconomyData = mb.getTableFromDB($"SELECT ItemInformation.ItemNumber, ITEMPROPERTYLINE.PROPERTYNAME, ITEMPROPERTYLINE.REMARK1  from ItemInformation left join ItemPropertyLine on ItemInformation.ItemNumber = ItemPropertyLine.ItemNumber where lower(ItemInformation.Itemnumber)=lower('{identity}') or lower(ItemInformation.Barcode)=lower('{identity}')");
            }

            if (maconomyData.Rows.Count == 0)
            {
                
                return null;
            }

            string article = maconomyData.Rows[0]["ItemNumber"].ToString();
            var tc = Get(article);

            return tc;

        }
        
    }
}