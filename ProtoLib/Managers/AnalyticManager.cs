using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class AnalyticManager
    {
        public DataTable TotalOrderToExcel(dynamic data)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Заказ");
            dt.Columns.Add("Артикул");
            dt.Columns.Add("Дата сдачи");
            dt.Columns.Add("Количество");

            dt.Columns.Add("Текущий участок");
            
            dt.Columns.Add("Норматив всего", typeof(decimal));
            dt.Columns.Add("Норматив выполнено", typeof(decimal));
            dt.Columns.Add("% выполнено всего", typeof(double));

            dt.Columns.Add("Производство");
            dt.Columns.Add("Комментарий");

            dt.Columns.Add("События");


            var firstLine = data.ArticleStat[0];
            foreach (var postData in firstLine.ByPosts)
            {
                dt.Columns.Add(postData.Name + " %");
            }

            foreach (var line in data.ArticleStat)
            {
                DataRow row = dt.NewRow();
                row["Артикул"] = line.Article;
                row["Заказ"] = line.Order;
                row["Количество"] = line.Count;
                row["Норматив всего"] = line.TotalCost;
                row["Норматив выполнено"] = line.CompletedCost;
                row["Производство"] = line.ProductLine;
                row["Комментарий"] = line.Comment;
                row["% выполнено всего"] = double.PositiveInfinity;
                row["Дата сдачи"] = line.DeadLine;
                if (line.TotalCost != 0)
                {
                    row["% выполнено всего"] = Math.Round((line.CompletedCost/line.TotalCost)*100,2);
                }

                if (line.TotalCost == line.CompletedCost)
                {
                    row["% выполнено всего"] = 100;
                }

                StringBuilder sb = new();
                List<string> lastPosts = new List<string>();
                foreach (var postData in line.ByPosts)
                {
                    string value = "";
                   
                    if (postData.Issues > 0)
                    {
                        foreach (var issueText in postData.IssuesText)
                        {
                            sb.AppendLine($"{postData.Name}:{issueText}");
                        }
                    }
                    if (postData.IsEmpty)
                    {
                        value = "[green]>>";
                    }

                    else
                    {
                       
                        if (postData.IsCompleted)
                        {
                            value = "[green]100";
                        }
                        else if (postData.Issues == 0)
                        {
                            value = (Math.Round(postData.Percentage,2)).ToString() ;
                            lastPosts.Add(postData.Name);
                        }
                        else
                        {
                            value = "[yellow]" + (Math.Round(postData.Percentage,2)).ToString();
                            lastPosts.Add(postData.Name);
                        }
                    }

                    row[postData.Name + " %"] = value;
                }

                row["События"] = sb.ToString();
                if (lastPosts.Count == 0)
                {
                    row["Текущий участок"] = "[завершено]";    
                }
                else
                {
                    row["Текущий участок"] = lastPosts.First();
                }
                
                dt.Rows.Add(row);
            }

            return dt;
        }

        public object PostStatus()
        {
            dynamic result = new ExpandoObject();

            var actualOrders = ActualOrders();
            var statusList = Enum.GetValues(typeof(WorkStatus))
                .Cast<WorkStatus>().ToList().Where(x => x != WorkStatus.unkown && x != WorkStatus.ended);
            result.ActualOrders = actualOrders;
            result.StatusNames = statusList.Select(x => WorkStatusMapper.Map(x));
            result.Posts = new List<object>();
            using (BaseContext c = new BaseContext(""))
            {
                var posts = c.Posts.OrderBy(x => x.ProductOrder).AsNoTracking().ToList();


                foreach (var post in posts)
                {
                    dynamic postStat = new ExpandoObject();
                    result.Posts.Add(postStat);
                    postStat.Name = post.Name;


                    var actualWorks = c.Works.AsNoTracking().Include(x => x.Issues)
                        .Where(x => x.Status != WorkStatus.ended && x.PostId == post.Name)
                        .ToList();

                    postStat.WorkCount = actualWorks.Count;
                    postStat.ItemCount = actualWorks.Sum(x => x.Count);
                    postStat.Cost = actualWorks.Sum(x => x.TotalCost);
                    postStat.Issues = actualWorks.SelectMany(x => x.Issues).Where(x => x.Resolved == null).Count();

                    postStat.OrderLoads = new List<object>();

                    var orderList = ActualOrders();
                    postStat.ActualOrders = orderList;


                    foreach (var order in orderList)
                    {
                        dynamic OrderLoad = new ExpandoObject();
                        postStat.OrderLoads.Add(OrderLoad);
                        OrderLoad.OrderNumber = order;
                        OrderLoad.Load = actualWorks.Where(x => x.OrderNumber == order).Sum(x => x.TotalCost);
                        OrderLoad.Statuses = new List<object>();

                        foreach (var status in statusList)
                        {
                            dynamic StatusLoad = new ExpandoObject();
                            OrderLoad.Statuses.Add(StatusLoad);

                            StatusLoad.Name = WorkStatusMapper.Map(status);
                            StatusLoad.Cost = actualWorks.Where(x => x.OrderNumber == order && x.Status == status)
                                .Sum(x => x.TotalCost);
                        }
                    }


                    postStat.Statuses = new List<object>();
                    foreach (var status in statusList)
                    {
                        dynamic statusStat = new ExpandoObject();

                        statusStat.Name = WorkStatusMapper.Map(status);
                        var statusWorks = actualWorks.Where(x => x.Status == status && x.PostId == post.Name).ToList();
                        statusStat.WorkCount = statusWorks.Count;
                        statusStat.ItemCount = statusWorks.Sum(x => x.Count);
                        statusStat.Cost = statusWorks.Sum(x => x.TotalCost);
                        statusStat.Issues =
                            statusWorks.SelectMany(x => x.Issues).Where(x => x.Resolved == null).Count();

                        postStat.Statuses.Add(statusStat);
                    }
                }
            }

            return result;
        }

        public List<long> ActualOrders()
        {
            using (BaseContext c = new BaseContext(""))
            {
                return c.Works.Where(x => x.Status != WorkStatus.ended).Select(x => x.OrderNumber).Distinct().ToList()
                    .OrderBy(x => x).ToList();
            }
        }

        public object OrderStat(long orderId)
        {
            dynamic result = new ExpandoObject();

            using (BaseContext c = new BaseContext(""))
            {
                var posts = c.Posts.OrderBy(x => x.ProductOrder).ToList();
                var orderWorks = c.Works.AsNoTracking().Include(x => x.Issues).Where(x => x.OrderNumber == orderId)
                    .ToList();
                var completedWorks = orderWorks.Where(x => x.Status == WorkStatus.ended).ToList();
                result.Order = orderId;
                result.Line = orderWorks.FirstOrDefault()?.ProductLine;
                result.TotalCost = orderWorks.Sum(x => x.TotalCost);
                result.TotalWorks = orderWorks.Count;
                result.TotalArticles = orderWorks.Select(x => x.Article).Distinct().Count();
                result.TotalWorkItems = orderWorks.Sum(x => x.Count);
                result.Issues = orderWorks.SelectMany(x => x.Issues).Count(x => x.Resolved == null);
                result.IssuesText = String.Join("\r\n", orderWorks.SelectMany(x => x.Issues)
                    .Where(x => x.Resolved == null)
                    .Select(x => $"{x.Work.PostId}: {x.Work.Article} - {x.Description}").ToList());
                result.CompletedWorks = completedWorks.Count();
                result.CompletedCost = completedWorks.Sum(x => x.TotalCost);

                result.PostCost = new List<object>();
                result.PostStatus = new List<object>();

                foreach (var p in posts)
                {
                    var postWorks = orderWorks.Where(x => x.PostId == p.Name).ToList();
                    var postCompletedWorks = postWorks.Where(x => x.Status == WorkStatus.ended).ToList();
                    dynamic postCost = new ExpandoObject();
                    result.PostCost.Add(postCost);
                    postCost.Name = p.TableName;
                    postCost.Cost = postWorks.Sum(x => x.TotalCost);

                    dynamic postStatus = new ExpandoObject();
                    result.PostStatus.Add(postStatus);
                    postStatus.Name = p.TableName;
                    postStatus.TotalCost = postWorks.Sum(x => x.TotalCost);
                    postStatus.CompletedCost = postCompletedWorks.Sum(x => x.TotalCost);

                    postStatus.Income = postWorks.Where(x => x.Status == WorkStatus.income).Sum(x => x.TotalCost);
                    postStatus.Waiting = postWorks.Where(x => x.Status == WorkStatus.waiting).Sum(x => x.TotalCost);
                    postStatus.Running = postWorks.Where(x => x.Status == WorkStatus.running).Sum(x => x.TotalCost);
                    postStatus.Sended = postWorks.Where(x => x.Status == WorkStatus.sended).Sum(x => x.TotalCost);

                    postStatus.Unstarted = postWorks.Where(x => x.Status == WorkStatus.hidden).Sum(x => x.TotalCost);
                    postStatus.Ended = postWorks.Where(x => x.Status == WorkStatus.ended).Sum(x => x.TotalCost);

                    postStatus.Issues = postWorks.SelectMany(x => x.Issues).Count(x => x.Resolved == null);
                }

                var articles = orderWorks.Select(x => x.Article).Distinct();

                result.ArticleStat = new List<object>();
                foreach (var article in articles)
                {
                    dynamic articleStat = new ExpandoObject();
                    articleStat.Article = article;

                    articleStat.ByPosts = new List<object>();
                    result.ArticleStat.Add(articleStat);
                    var articleWorks = orderWorks.Where(x => x.Article == article).ToList();
                    var completedArticleWorks = articleWorks.Where(x => x.Status == WorkStatus.ended);
                    articleStat.WorkCount = articleWorks.Count;
                    articleStat.Count = articleWorks.Max(x => x.Count);
                    articleStat.TotalCost = articleWorks.Sum(x => x.TotalCost);
                    articleStat.CompletedCost = completedArticleWorks.Sum(x => x.TotalCost);
                    articleStat.Issues = articleWorks.SelectMany(x => x.Issues).Count(x => x.Resolved == null);


                    foreach (var post in posts)
                    {
                        var articlePostWorks = articleWorks.Where(x => x.PostId == post.Name).ToList();
                        var completedArticlePostWorks = articlePostWorks.Where(x => x.Status == WorkStatus.ended);
                        dynamic postStat = new ExpandoObject();
                        articleStat.ByPosts.Add(postStat);

                        postStat.Name = post.TableName;
                        postStat.TotalCost = articlePostWorks.Sum(x => x.TotalCost);
                        postStat.CompletedCost = completedArticlePostWorks.Sum(x => x.TotalCost);
                        if (postStat.TotalCost > 0)
                        {
                            postStat.Percentage = (postStat.CompletedCost / postStat.TotalCost) * 100;
                        }
                        else
                        {
                            postStat.Percentage = 0;
                        }


                        postStat.IsCompleted = postStat.TotalCost == postStat.CompletedCost;
                        postStat.IsEmpty = articlePostWorks.Count() == 0;


                        postStat.Issues = articlePostWorks.SelectMany(x => x.Issues).Count(x => x.Resolved == null);
                        postStat.IssuesText = articlePostWorks.SelectMany(x => x.Issues).Where(x => x.Resolved == null)
                            .Select(x => x.Description).ToList();
                    }
                }
            }

            return result;
        }

        public object PostRetroStatus(DateTime from, DateTime to)
        {
            using (BaseContext c = new BaseContext())
            {
                dynamic result = new ExpandoObject();
                var statistics = c.PostStatistics.Where(x => x.Stamp > from && x.Stamp < to).ToList();
                result.dataByOrder = statistics;
                result.stampPoints = statistics.Select(x => x.Stamp).Distinct();
                result.orders = statistics.Select(x => x.OrderNumber.ToString()).Distinct();
                result.posts = statistics.Select(x => x.PostId).Distinct();

                result.dataByStatus = new List<object>();
                var stamps = statistics.GroupBy(x => x.Stamp);
                foreach (var stamp in stamps)
                {
                    var posts = stamp.GroupBy(x => x.PostId);
                    foreach (var post in posts)
                    {
                        dynamic postStatus = new ExpandoObject();
                        postStatus.post = post.Key;
                        postStatus.predict = post.Sum(x => x.PredictCost);
                        postStatus.income = post.Sum(x => x.IncomeCost);
                        postStatus.waiting = post.Sum(x => x.WaitingCost);
                        postStatus.running = post.Sum(x => x.RunningCost);
                        postStatus.sended = post.Sum(x => x.SendedCost);
                        postStatus.total = post.Sum(x => x.TotalCost);
                        result.dataByStatus.Add(postStatus);
                    }
                }


                return result;
            }
        }

        public object OrderTimeLine(long orderNumber)
        {
            dynamic result = new ExpandoObject();
            result.OrderNumber = orderNumber;
            result.Articles = new List<object>();

            using (BaseContext c = new BaseContext("system"))
            {
                var data = c.WorkStatusLogs.Where(x => x.OrderNumber == orderNumber)
                    .OrderBy(x => x.Stamp)
                    .ToList();
                var issues = c.WorkIssueLogs.Where(x => x.OrderNumber == orderNumber).ToList();

                var articleGroup = data.GroupBy(x => x.Article);
                foreach (var article in articleGroup)
                {
                    dynamic Article = new ExpandoObject();
                    Article.Name = article.Key;
                    result.Articles.Add(Article);

                    Article.Data = new List<object>();
                    Article.Issues = new List<object>();
                    var articleIssues = issues.Where(x => x.Article == article.Key).ToList();
                    foreach (var issue in articleIssues)
                    {
                        dynamic iss = new ExpandoObject();

                        Article.Issues.Add(iss);
                        iss.Start = issue.Start;
                        if (issue.End.HasValue)
                        {
                            iss.End = issue.End.Value;
                        }
                        else
                        {
                            iss.End = DateTime.Now;
                        }

                        iss.Type = issue.Type;
                        iss.Description = issue.Description;
                    }

                    var postGroup = article
                        .GroupBy(x => x.PostId);
                    foreach (var post in postGroup)
                    {
                        dynamic postData = new ExpandoObject();
                        postData.Post = post.Key;
                        Article.Data.Add(postData);
                        postData.Values = new List<object>();
                        DateTime? lastEvent = null;
                        bool isEnded = false;
                        var works = post.Where(x =>
                            x.PrevStatus !=
                            WorkStatus.unkown); //чтобы отсечь прогнозируемые работы но при это сохранить все артикула
                        postData.Issues = new List<object>();
                        var postIssues = articleIssues.Where(x => x.PostId == post.Key);

                        WorkStatus lastState = WorkStatus.unkown;

                        foreach (var postLog in works)
                        {
                            if (lastEvent == null)
                            {
                                lastEvent = postLog.Stamp;
                                lastState = postLog.NewStatus;
                                continue;
                            }

                            dynamic record = new ExpandoObject();
                            postData.Values.Add(record);

                            record.Start = lastEvent.Value;
                            record.End = postLog.Stamp;
                            record.DeltaMinutes = (postLog.Stamp - lastEvent.Value).TotalMinutes;
                            record.DeltaHours = (postLog.Stamp - lastEvent.Value).TotalHours;
                            record.Comment = WorkStatusMapper.Map(postLog.PrevStatus);
                            record.Status = postLog.PrevStatus;
                            lastState = postLog.NewStatus;
                            lastEvent = postLog.Stamp;
                            isEnded = postLog.NewStatus == WorkStatus.ended;
                            if (isEnded)
                            {
                                lastEvent = null;
                            }
                        }

                        if (!isEnded)
                        {
                            if (lastEvent == null)
                            {
                                //значит и не начато
                                continue;
                            }

                            dynamic record = new ExpandoObject();
                            postData.Values.Add(record);

                            record.Start = lastEvent.Value;

                            record.End = DateTime.Now;
                            record.DeltaMinutes = (DateTime.Now - lastEvent.Value).TotalMinutes;
                            record.DeltaHours = (DateTime.Now - lastEvent.Value).TotalHours;
                            record.Comment = WorkStatusMapper.Map(lastState);
                            record.Status = lastState;
                        }

                        if (postData.Values.Count > 0)
                        {
                            postData.TotalPeriod = new ExpandoObject();
                            postData.TotalPeriod.Start = ((List<dynamic>) postData.Values).First().Start;
                            postData.TotalPeriod.End = ((List<dynamic>) postData.Values).Last().End;
                            postData.TotalPeriod.DeltaMinutes =
                                (postData.TotalPeriod.End - postData.TotalPeriod.Start).TotalMinutes;
                            postData.TotalPeriod.DeltaHours =
                                (postData.TotalPeriod.End - postData.TotalPeriod.Start).TotalHours;
                        }

                        foreach (var postIssue in postIssues)
                        {
                            dynamic iss = new ExpandoObject();
                            postData.Issues.Add(iss);
                            iss.Start = postIssue.Start;
                            if (postIssue.End.HasValue)
                            {
                                iss.End = postIssue.End.Value;
                            }
                            else
                            {
                                iss.End = DateTime.Now;
                            }

                            iss.Type = postIssue.Type;
                            iss.Description = postIssue.Description;
                            iss.DeltaMinutes = (iss.End - iss.Start).TotalMinutes;
                            iss.DeltaHours = (iss.End - iss.Start).TotalHours;
                        }
                    }
                }
            }

            return result;
        }

        public object TotalOrderStat(string orderFilter = "", string articleFilter = "")
        {
            using (BaseContext c = new BaseContext(""))
            {
                var posts = c.Posts.OrderBy(x => x.ProductOrder).ToList();
                var orders = c.Works.Where(x => x.Status != WorkStatus.ended).Select(x => x.OrderNumber).Distinct()
                    .ToList().OrderBy(x => x).ToList();
                if (!string.IsNullOrEmpty(orderFilter))
                {
                    orders = orders.Where(x => x.ToString().Contains(orderFilter)).ToList();
                }

                dynamic result = new ExpandoObject();
                result.ArticleStat = new List<object>();
                foreach (var orderId in orders)
                {
                    var orderWorks = c.Works.AsNoTracking()
                        .Include(x => x.Issues)
                        .Include(x=>x.Post)
                        .Where(x => x.OrderNumber == orderId)
                        .ToList();
                    var articles = orderWorks.Select(x => x.Article).Distinct();
                    if (!string.IsNullOrEmpty(articleFilter))
                    {
                        articles = articles.Where(x => x.Contains(articleFilter));
                    }
                    
                    foreach (var article in articles)
                    {
                        dynamic articleStat = new ExpandoObject();

                        articleStat.Order = orderId;
                        articleStat.Article = article;
                        
                       
                        result.ArticleStat.Add(articleStat);
                        var articleWorks = orderWorks.Where(x => x.Article == article).ToList();
                        var completedArticleWorks = articleWorks.Where(x => x.Status == WorkStatus.ended);
                        
                        if (articleWorks.Count > 0)
                        {
                            articleStat.DeadLine = articleWorks.FirstOrDefault().DeadLine; 
                            articleStat.ProductLine = articleWorks.FirstOrDefault().ProductLine;
                            articleStat.Comment = articleWorks.FirstOrDefault().Description;
                        }

                        articleStat.WorkCount = articleWorks.Count;
                        articleStat.Count = articleWorks.Max(x => x.Count);
                        articleStat.TotalCost = articleWorks.Sum(x => x.TotalCost);
                        articleStat.CompletedCost = completedArticleWorks.Sum(x => x.TotalCost);
                        articleStat.Issues = articleWorks.SelectMany(x => x.Issues).Count(x => x.Resolved == null);

                        articleStat.ByPosts = new List<object>();
                        List<string> postCurrent = new List<string>();
                        foreach (var post in posts)
                        {
                            var articlePostWorks = articleWorks.Where(x => x.PostId == post.Name).ToList().OrderBy(x=>x.Post.ProductOrder);
                            var completedArticlePostWorks = articlePostWorks.Where(x => x.Status == WorkStatus.ended).OrderBy(x=>x.Post.ProductOrder);
                            dynamic postStat = new ExpandoObject();
                            articleStat.ByPosts.Add(postStat);

                            postStat.Name = post.Name;
                            postStat.TotalCost = articlePostWorks.Sum(x => x.TotalCost);
                            postStat.CompletedCost = completedArticlePostWorks.Sum(x => x.TotalCost);
                            if (postStat.TotalCost > 0)
                            {
                                postStat.Percentage = (postStat.CompletedCost / postStat.TotalCost) * 100;
                            }
                            else
                            {
                                postStat.Percentage = 0;
                            }
                           

                            postStat.IsCompleted = postStat.TotalCost == postStat.CompletedCost;
                            postStat.IsEmpty = articlePostWorks.Count() == 0;

                            if (articleWorks.Count() != 0 && postStat.TotalCost != postStat.CompletedCost)
                            {
                                postCurrent.Add(post.Name);
                            }
                            
                            postStat.Issues = articlePostWorks.SelectMany(x => x.Issues).Count(x => x.Resolved == null);
                            postStat.IssuesText = articlePostWorks.SelectMany(x => x.Issues)
                                .Where(x => x.Resolved == null)
                                .Select(x => x.Description).ToList();
                        }

                        if (postCurrent.Count > 0)
                        {
                            articleStat.CurrentPost = postCurrent.First();
                        }
                        else
                        {
                            articleStat.CurrentPost = "[завершено]";
                        }
                       

                    }
                }

                return result;
            }
        }
    }
}