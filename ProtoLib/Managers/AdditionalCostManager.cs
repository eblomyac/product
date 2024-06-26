using ProtoLib.Model;

namespace ProtoLib.Managers;

public class AdditionalCostManager
{
    public List<AdditionalCostTemplate> List(bool showDisabled=false)
    {
        using (BaseContext c = new BaseContext())
        {
            if (showDisabled)
            {
                return c.AdditionalCostTemplates.ToList();    
            }
            else
            {
                return c.AdditionalCostTemplates.Where(x=>!x.Disabled).ToList();
            }
            
        }
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

                exist.Disabled = add.Disabled;
                result.Add(exist);
                
            }

            c.SaveChanges();
            return result;
        }
    }
}