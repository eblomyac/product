using ProtoLib.Model;

namespace ProtoLib.Managers;

public class WorkRemover
{
    public void RemoveWorks(List<Tuple<long,string>> toRemove)
    {
        using (BaseContext c = new BaseContext())
        {
            foreach (var tuple in toRemove)
            {
                var works = c.Works.Where(x => x.Article == tuple.Item2 && x.OrderNumber == tuple.Item1);
                var isAllHidden = works.All(x => x.Status == WorkStatus.hidden);
                if (isAllHidden)
                {
                    c.Works.RemoveRange(works);
                }
                
            }

            c.SaveChanges();
        }
    }
}