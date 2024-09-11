using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class AdditionalCostManager
{
    public List<AdditionalCostTemplate> ListAll()
    {
        using (BaseContext c = new BaseContext())
        {
           
            return c.AdditionalCostTemplates.AsNoTracking().ToList();
           
            
        }
    }
    public List<AdditionalCostTemplate> ListForPost()
    {
        using (BaseContext c = new BaseContext())
        {
           
                return c.AdditionalCostTemplates.AsNoTracking().Where(x=>x.CanPost).ToList();
           
            
        }
    }
    public List<AdditionalCostTemplate> ListForItem()
    {
        using (BaseContext c = new BaseContext())
        {
            return c.AdditionalCostTemplates.AsNoTracking().Where(x=>x.CanItem).ToList();
            
        }
    }

    public Work CreateForPost(string postId, string lineId, string accName, List<AdditionalCost> costs, string article="",string description="")
    {
        WorkCreateManager wcm = new WorkCreateManager();
        var work = wcm.CreateAdditionalWork(postId, lineId, accName, costs.Select(x=>x.AdditionalCostTemplate.Name +" " + x.Description).ToList(), description, article);
        foreach (var ac in costs)
        {
            ac.WorkId = work.Id;
            CreateForWork(ac);
        }

        return work;

    }

    public AdditionalCost CreateForWork(AdditionalCost ac)
    {
        using (BaseContext c = new BaseContext())
        {
            AdditionalCost acn = new AdditionalCost();
            acn.AdditionalCostTemplateId = ac.AdditionalCostTemplateId;
            acn.Description = ac.Description;
            acn.WorkId = ac.WorkId;
            acn.Cost = ac.Cost;
            acn.Comment = ac.Comment;
            c.AdditionalCosts.Add(acn);
            c.SaveChanges();
            acn.AdditionalCostTemplate =
                c.AdditionalCostTemplates.FirstOrDefault(x => x.Id == acn.AdditionalCostTemplateId);
            return acn;
        }
    }

    public List<AdditionalCostTemplate> SaveAndCreate(List<AdditionalCostTemplate> adds)
    {
        using (BaseContext c = new BaseContext())
        {
            List<AdditionalCostTemplate> result = new List<AdditionalCostTemplate>();
            foreach (var add in adds)
            {
                string name = add.Name.ToLower();
                var exist = c.AdditionalCostTemplates.FirstOrDefault(x => x.Name.ToLower() == name);
                if (exist == null)
                {
                    exist = new AdditionalCostTemplate();
                    exist.Name = add.Name;
                    c.AdditionalCostTemplates.Add(exist);
                }

                exist.CanPost = add.CanPost;
                exist.CanItem = add.CanItem;
                result.Add(exist);
                
            }

            c.SaveChanges();
            return result;
        }
    }
}