using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class PostWorkManager
{
    private string _postId;
    private string _accName;
    public PostWorkManager(string postId, string accName)
    {
        _postId = postId;
        _accName = accName;
    }

     public void CreateWork(long orderNumber, int orderLineNumber, int count, string fromPost)
    {
        using (BaseContext c = new BaseContext(_accName))
        {
            using (var transaction = c.Database.BeginTransaction())
            {
                try
                {
                    WorkTemplateLoader wtl = new WorkTemplateLoader();
                    var keys = c.PostKeys.Where(x => x.PostId == this._postId).Select(x=>x.Key).ToList();
                    wtl.LoadForPostKeys(orderNumber.ToString(), orderLineNumber, keys);
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                }

            }
        }
    }
    public void TakeWorkForProduction(long orderNumber, int orderLineNumber, int count, string fromPost)
    {
        using (BaseContext c = new BaseContext(_accName))
        {
            using (var transaction = c.Database.BeginTransaction())
            {
                try
                {


                    var currentSameWorks = c.Works.AsNoTracking().Include(x => x.Issues).Where(x =>
                            x.OrderNumber == orderNumber && x.OrderLineNumber == orderLineNumber &&
                            x.PostId == _postId &&
                            (x.Status == WorkStatus.hidden || x.Status == WorkStatus.income))
                        .ToList();
                    if (currentSameWorks.Count > 0)
                    {
                        var exactCount = currentSameWorks.FirstOrDefault(x => x.Count == count);
                        Work work = null;
                        if (exactCount == null)
                        {
                            var workToSplit = currentSameWorks.FirstOrDefault(x => x.Count > count);
                            if (workToSplit != null)
                            {
                                WorkManagerFacade wmf = new WorkManagerFacade(c);
                                work = wmf.SplitWork(workToSplit, count)[0];
                            }
                        }
                        else
                        {
                            work = exactCount;
                        }

                        if (work != null)
                        {
                            if (work.Issues != null)
                            {
                                IssueManager im = new IssueManager(c);
                                foreach (var issue in work.Issues.Where(x => x.Resolved == null))
                                {
                                    im.ResolveIssue(issue.Id, _accName);
                                }
                            }

                            WorkSaveManager wsm = new WorkSaveManager(c);
                            WorkStatusChanger wsc = new WorkStatusChanger(c);
                            wsc.ChangeStatus(work, WorkStatus.waiting, _accName, moveFrom: fromPost);
                            wsm.SaveWorks(new List<Work>() { work });

                        }
                    }
                    else
                    {
                        
                    }
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                }

            }
        }
    }

    public void TakeWorkForRework(long orderNumber, int orderLineNumber, int count, string fromPost, Issue issue)
    {
        
    }
    
}