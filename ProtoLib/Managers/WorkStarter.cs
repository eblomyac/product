using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class WorkStarter
    {
        private WorkSaveManager wsm;

        public WorkStarter(string accName)
        {
            wsm = new WorkSaveManager(accName);
        }
        public bool StartWorksOperator(List<WorkAnalytic.WorkStartSuggestion> toStart, string accName)
        {
            WorkStatusChanger wss = new WorkStatusChanger();
            using (BaseContext c = new BaseContext(accName))
            {

                List<Work> allWorks = new List<Work>();
                foreach (var suggestion in toStart)
                {
                    foreach (var post in suggestion.SelectedPosts)
                    {
                        var works = c.Works.AsNoTracking().Where(x =>
                            x.Article == suggestion.Article && x.OrderNumber == suggestion.OrderNumber &&
                            x.PostId == post).ToList();
                        if (works.Count > 0)
                        {
                            foreach (var work in works)
                            {
                                work.MovedFrom = "ИТР";
                            }
                            wss.ChangeStatus(works, WorkStatus.waiting,accName);
                            allWorks.AddRange(works);                         
                        }
                        
                        
                        
                    }
                }
                return wsm.SaveWorks(allWorks).Count >0;
                
            }

            return false;

        }


        public bool MoveWorkMaster(long workId, string toPostId, string accName, List<string> startOnPosts, string oldPostId)
        {
            WorkStatusChanger wss = new WorkStatusChanger();
            using (BaseContext c = new BaseContext(accName))
            {
                //List<Work> allWorks = new List<Work>();
                var currentWork = c.Works.Include(x=>x.AdditionalCosts)
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == workId);

                if (currentWork.Status != WorkStatus.sended)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(oldPostId))
                {
                   
                    var previousStartedWorks = c.Works.AsNoTracking().Where(x =>
                        x.Article == currentWork.Article && x.OrderNumber == currentWork.OrderNumber &&
                        x.MovedFrom == currentWork.PostId && x.Status== WorkStatus.income).ToList();
                    foreach (var prevStarted in previousStartedWorks)
                    {
                        prevStarted.MovedFrom = "";
                        wss.ChangeStatus(prevStarted, WorkStatus.hidden, accName);
                    
                    }
                    wsm.SaveWorks(previousStartedWorks);
                }
                
               
                
                
                currentWork.MovedTo = toPostId;

                //if (toPostId == Constants.Work.EndPosts.JustEnd)
                //{
                    //JUST SET 50 status
//                    wss.ChangeStatus(currentWork, WorkStatus.ended, accName);
//                }
                if (toPostId == Constants.Work.EndPosts.TotalEnd || toPostId == Constants.Work.EndPosts.OTK)
                {
                    wss.ChangeStatus(currentWork, WorkStatus.ended, accName);
                    //JUST SET 50 status
                }

                var nextWorks = c.Works.AsNoTracking().Include(x=>x.Issues).Where(x =>
                    x.Article == currentWork.Article && x.OrderNumber == currentWork.OrderNumber && 
                    x.Count==currentWork.Count && x.OrderLineNumber == currentWork.OrderLineNumber &&
                    (startOnPosts.Contains(x.PostId)||x.PostId==toPostId) && x.Status==WorkStatus.hidden).ToList();

                if (currentWork.OrderNumber==100)
                {
                    
                    if (currentWork.Article.Contains("Возврат"))
                    {
                        var issueId = long.Parse(currentWork.AdditionalCosts.First().Comment);
                        IssueManager im = new();
                        im.ResolveIssue(issueId, accName);
                    }
                }

                if (nextWorks.Count == 0 && toPostId!=Constants.Work.EndPosts.TotalEnd)
                {
                        var PostKeys = c.PostKeys.Where(x => x.PostId == toPostId).Select(x => x.Key).ToList();
                        WorkTemplateLoader wtl = new WorkTemplateLoader();
                        var template = wtl.LoadForPostKeys(currentWork.OrderNumber.ToString(), currentWork.OrderLineNumber, PostKeys);
                   
                        var sharedWork = new Work();
                        sharedWork.Article = currentWork.Article;
                        sharedWork.Status = WorkStatus.income;
                        sharedWork.MovedTo = "";
                        sharedWork.OrderNumber = currentWork.OrderNumber;
                        sharedWork.OrderLineNumber = currentWork.OrderLineNumber;
                        sharedWork.Comments = new List<string>();
                        sharedWork.Description = currentWork.Description;
                        sharedWork.PostId = toPostId;
                        sharedWork.ProductLineId = currentWork.ProductLineId;
                        sharedWork.MovedFrom = currentWork.PostId;
                        sharedWork.CreatedStamp = DateTime.Now;
                        sharedWork.SingleCost =template.Count>0? template.Sum(x => x.SingleCost):0;
                        sharedWork.Count = currentWork.Count;
                        sharedWork.DeadLine = currentWork.DeadLine;
                        sharedWork.CommentMap =template.Count>0? string.Join('\t', template.Select(x => x.Comment)):"Произв. плана не найдено";

                        if (toPostId == Constants.Work.EndPosts.OTK)
                        {
                            sharedWork.Status = WorkStatus.running;
                        }
                        
                        c.Works.Add(sharedWork);
                        c.SaveChanges();
                    
                }
                
                foreach (var nextWork in nextWorks)
                {
                    nextWork.MovedFrom = currentWork.PostId;
                    if (nextWork.Issues.Count > 0)
                    {
                        IssueManager ism = new IssueManager();
                        foreach (var workIssue in nextWork.Issues)
                        {
                            if (workIssue.Resolved == null)
                            {
                                ism.ResolveIssue(workIssue.Id, accName);
                            }    
                        }
                        
                    }
                    wss.ChangeStatus(nextWork, WorkStatus.income, accName);
                }
                
                nextWorks.Add(currentWork);
                return wsm.SaveWorks(nextWorks).Count > 0;


            }

            
            }

       
    }
}