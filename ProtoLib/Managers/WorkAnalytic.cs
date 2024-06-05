using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class WorkAnalytic
    {
        public class WorkStartSuggestion
        {
            public long OrderNumber { get; set; }
            public int OrderLineNumber { get; set; }
            public string Article { get; set; }
            public List<string> AvailablePosts { get; set; }
            public List<string> SelectedPosts { get; set; }
        }
        
        public WorkAnalytic()
        {
            
        }

        public List<WorkStartSuggestion> UnstartedSuggestions(List<Work> works)
        {

            var result = new List<WorkStartSuggestion>();
            var order = works.GroupBy(x => x.OrderNumber);
            foreach (var orderGroup in order)
            {
                var article = orderGroup.GroupBy(x => x.Article);
                foreach (var articleGroup in article)
                {
                    var line = articleGroup.GroupBy(x => x.OrderLineNumber);

                    foreach (var lineGroup in line)
                    {
                        WorkStartSuggestion uws = new WorkStartSuggestion();
                        uws.Article = articleGroup.Key;
                        uws.OrderNumber = orderGroup.Key;
                        uws.OrderLineNumber = lineGroup.Key;
                        uws.AvailablePosts = lineGroup.Select(x => x.Post).OrderBy(x=>x.ProductOrder).Select(x=>x.Name).ToList();
                        uws.SelectedPosts = new List<string>();
                        result.Add(uws);
                    }
                   
                }
            }

            return result;

        }

        public List<Work> UnstartedWorks(List<Work> works)
        {
            var result = new List<Work>();
            var orderGroup = works.GroupBy(x => x.OrderNumber);
            foreach (var order in orderGroup)
            {
                var articleGroup = order.GroupBy(x => x.Article);
                foreach (var article in articleGroup)
                {
                    using (BaseContext c = new BaseContext(""))
                    {
                        
                        bool isUnstarted = c.Works.Where(x => x.Article == article.Key && x.OrderNumber == order.Key)
                            .All(x => x.Status == WorkStatus.hidden || x.Status == WorkStatus.unkown);
                        if (isUnstarted)
                        {
                            result.AddRange(c.Works.AsNoTracking().Include(x=>x.Post).Where(x => x.Article == article.Key && x.OrderNumber == order.Key).ToList());
                        }
                    }
                }
            }

            return result;

        }
        public List<Work> UnstartedWorks(DateTime startStamp)
        {
            
            var suggestableWorkList = this.WorksCreatedAfterStamp(startStamp, new [] {WorkStatus.hidden, WorkStatus.unkown});
            return UnstartedWorks(suggestableWorkList);
        }
        private List<Work> WorksCreatedAfterStamp(DateTime stamp,WorkStatus[] statusWhere)
        {
            using (BaseContext c = new BaseContext(""))
            {
                return c.Works.AsNoTracking().Where(x => x.CreatedStamp > stamp && statusWhere.Contains(x.Status)).ToList();
            }
        }
    }

  

    public class WorkAnalyticFacade
    {
        public List<Work> PostWorks(string accName, string postId)
        {
            using (BaseContext c = new BaseContext(accName))
            {
                var user = c.Users.AsNoTracking().Include(x=>x.Roles).FirstOrDefault(x => x.AccName == accName);
                if (user == null)
                {
                    return new List<Work>();
                }
                string selectedPost = null;
                List<string> availablePosts = user.PostIdMaster;
                if (availablePosts.Count == 1)
                {
                    selectedPost = availablePosts[0];
                }else if (availablePosts.Count > 1)
                {
                    selectedPost = availablePosts.FirstOrDefault(x => x == postId);
                }
                if (string.IsNullOrEmpty(selectedPost))
                {
                    return new List<Work>();
                }
                if (user.IsMaster)
                {
                    var works = c.Works.Include(x=>x.Issues).Where(x => x.PostId ==selectedPost &&
                                       x.Status != WorkStatus.ended &&
                                       x.Status != WorkStatus.hidden &&
                                       x.Status != WorkStatus.unkown).ToList();
                    List<long> orders = works.Select(x => x.OrderNumber).Distinct().ToList();
                    WorkPriorityManager wpm = new WorkPriorityManager();
                    var pr = wpm.WorkPriorityList(orders);
                    foreach (var w in works)
                    {
                        var orderPriority =
                            pr.FirstOrDefault(x => x.OrderNumber == w.OrderNumber && x.Article.Length < 1);
                        var articlePriority =
                            pr.FirstOrDefault(x => x.OrderNumber == w.OrderNumber && x.Article == w.Article);
                        if (orderPriority == null && articlePriority == null)
                        {
                            w.Priority = 10;
                        }
                        if (orderPriority != null)
                        {
                            w.Priority = orderPriority.Priority;
                        }
                        if (articlePriority != null)
                        {
                            w.Priority = articlePriority.Priority;
                        }
                        
                    }
                    return works.OrderByDescending(x=>x.Priority).ThenBy(x=>x.DeadLine).ToList();
                }
                else
                {
                    return new List<Work>();
                }
                
            }
        }
        public bool StartWorkOperator(List<WorkAnalytic.WorkStartSuggestion> suggestions, string accName)
        {
            bool IsOperator = false;
            string post = null;
            using (BaseContext c = new BaseContext(accName))
            {
                var user = c.Users.Include(x => x.Roles).FirstOrDefault(x => x.AccName== accName);
                if (user == null)
                {
                    return false;
                }
                else
                {
                    IsOperator = user.IsOperator;
             
                }
            }
            WorkStarter ws = new WorkStarter(accName);
            if (IsOperator)
            {

                return ws.StartWorksOperator(suggestions, accName);    
            }
           

            return false;
        }
        public List<WorkAnalytic.WorkStartSuggestion> UnstartedWokSuggestions(DateTime stampFrom)
        {
            WorkAnalytic wa = new WorkAnalytic();
            var list = wa.UnstartedWorks(stampFrom);
            return wa.UnstartedSuggestions(list);
        }
        public List<WorkAnalytic.WorkStartSuggestion> UnstartedWokSuggestions(List<Work> works)
        {
            WorkAnalytic wa = new WorkAnalytic();
            var list = wa.UnstartedWorks(works);
            return wa.UnstartedSuggestions(list);
        }

        public List<object> GetMoveSuggestions(List<Work> works,string accName)
        {
            var result = new List<object>();
            using (BaseContext c = new BaseContext(""))
            {
                var user=  c.Users.Include(x => x.Roles).FirstOrDefault(x=>x.AccName == accName);
                if (user == null || !user.IsMaster)
                {
                    return new List<object>();
                }
                foreach (var work in works)
                {
                    /*
                    var allWorks = c.Works.AsNoTracking().Where(x => x.Article == work.Article && x.OrderNumber == work.OrderNumber);
                    var backwardDirection = allWorks
                       .Where(x => (x.Status == WorkStatus.sended || x.Status == WorkStatus.ended) && x.MovedTo == work.PostId)
                       .Select(x=>x.PostId)
                       .Where(x=>x!=work.PostId)
                       .Distinct().ToList();
                    
                    var forwardDirection =
                        allWorks
                            .Where(x => x.Status == WorkStatus.hidden || x.Status == WorkStatus.unkown)
                            .Select(x=>x.PostId)
                            .Where(x=>x!=work.PostId)
                            .Distinct().ToList();
                    if (forwardDirection.Count == 0)
                    {
                        forwardDirection.Add(Constants.Work.EndPosts.JustEnd);
                            /*if (allWorks.All(x => x.Status == WorkStatus.ended || x.Status == WorkStatus.sended))
                        {
                            forwardDirection.Add(Constants.Work.EndPosts.TotalEnd);    
                        }
                        else
                        {
                           
                        }
                        
                    }*/
                    List<string> backwardDirection = new List<string>();
                    List<string> forwardDirection = new List<string>();
                    backwardDirection.AddRange(c.Posts.Select(x=>x.Name).ToList());
                    forwardDirection.AddRange(c.Posts.Select(x=>x.Name).ToList());
                    var resultRecord = new
                        {Work = work, Forward = forwardDirection,
                            Backward = backwardDirection};
                    result.Add(resultRecord);
                }    
            }

            return result;

        }
        
    }
}