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
                            wss.ChangeStatus(works, WorkStatus.income,accName);
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
                var currentWork = c.Works.AsNoTracking().FirstOrDefault(x => x.Id == workId);

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

                if (toPostId == Constants.Work.EndPosts.JustEnd)
                {
                    //JUST SET 50 status
                    wss.ChangeStatus(currentWork, WorkStatus.ended, accName);
                }else if (toPostId == Constants.Work.EndPosts.TotalEnd)
                {
                    wss.ChangeStatus(currentWork, WorkStatus.ended, accName);
                    //JUST SET 50 status
                }

                var nextWorks = c.Works.AsNoTracking().Where(x =>
                    x.Article == currentWork.Article && x.OrderNumber == currentWork.OrderNumber &&
                    (startOnPosts.Contains(x.PostId)||x.PostId==toPostId)).ToList();


                if (nextWorks.Count == 0)
                {
                    bool? shared = c.Posts.FirstOrDefault(x => x.Name == toPostId)?.IsShared;
                    if (shared.HasValue && shared.Value)
                    {
                        var sharedWork = new Work();
                        sharedWork.Article = currentWork.Article;
                        sharedWork.Status = WorkStatus.income;
                        sharedWork.MovedTo = null;
                        sharedWork.OrderNumber = currentWork.OrderNumber;
                        sharedWork.Comments = new List<string>();
                        sharedWork.Description = currentWork.Description;
                        sharedWork.PostId = toPostId;
                        sharedWork.ProductLine = currentWork.ProductLine;
                        sharedWork.MovedFrom = currentWork.PostId;
                        sharedWork.CreatedStamp = DateTime.Now;
                        sharedWork.SingleCost = 1;
                        sharedWork.Count = currentWork.Count;
                        c.Works.Add(sharedWork);
                        c.SaveChanges();
                    }
                }
                
                foreach (var nextWork in nextWorks)
                {
                    nextWork.MovedFrom = currentWork.PostId;
                    wss.ChangeStatus(nextWork, WorkStatus.income, accName);
                }
                
                nextWorks.Add(currentWork);
                return wsm.SaveWorks(nextWorks).Count > 0;


            }

            
        }

       
    }
}