using Org.BouncyCastle.Math.EC;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class OperatorManager
{
    public List<Work> StartWork(List<Work> allArticleWork, string postId, string accName)
    {
        WorkManagerFacade wmf = new WorkManagerFacade(accName);
        var works = allArticleWork.Where(x => x.PostId == postId).ToList();
        var createdWorks =  wmf.CreateWorks(works);
        WorkStatusChanger wss = new WorkStatusChanger();
        WorkSaveManager wsm = new WorkSaveManager();
        wss.ChangeStatus(createdWorks, WorkStatus.waiting, accName, "ИТР");
        wsm.SaveWorks(createdWorks);
        return createdWorks;

    }

    public List<Work> StartAllWorks(List<WorkPrepareGroupResult> groupWorks, string accName)
    {
        var result = new List<Work>();
        foreach (var group in groupWorks.Where(x=>!string.IsNullOrWhiteSpace(x.StartOnDefault)))
        {
            result.AddRange(StartWork(group.Source, group.StartOnDefault, accName));
        }

        return result;
    }
}