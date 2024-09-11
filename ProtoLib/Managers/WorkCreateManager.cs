using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Math;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class WorkSplitter
    {
        private BaseContext? _c = null;
        public WorkSplitter(BaseContext? c = null)
        {
            _c = c;
        }
        public List<Work> SplitWork(Work w, int splitCount, string accName)
        {
            List<Work> changedWorks = new List<Work>();
            if (splitCount >= w.Count)
            {
                throw new ArgumentException("Количество работы не может быть нулевым или отрицательным");
            }
            Work newWork = (Work) w.Clone();
            int oldCount = w.Count;
            newWork.Count = splitCount;
            w.Count -= splitCount;
            changedWorks.Add(w);
            changedWorks.Add(newWork);
            if (w.Status == WorkStatus.income)
            {
                //если работа сплититься при приемке расплитим ее и на сдаче
                BaseContext c = _c ?? new BaseContext(accName);
                {
                    var prevWork = c.Works.AsNoTracking().FirstOrDefault(x =>
                        x.Article == w.Article &&
                        x.OrderNumber == w.OrderNumber &&
                        x.Status == WorkStatus.sended &&
                        x.PostId == w.MovedFrom &&
                        x.MovedTo == w.PostId && 
                        x.Count == oldCount);
                    if (prevWork != null)
                    {
                        changedWorks.AddRange(SplitWork(prevWork, splitCount,accName));
                    }
                }
                if (_c == null)
                {
                    c.Dispose();
                }
            }

            return changedWorks;


        }
    }
   
    public class WorkSaveManager
    {
        private string _accName;
        private BaseContext? _c = null;
        public WorkSaveManager(string accName="")
        {
            _accName = accName;
        }

        public WorkSaveManager(BaseContext? c)
        {
            _c = c;
            if (_c != null)
            {
                this._accName = _c.accName;
            }
            else
            {
                _accName = "";
            }
        }

        public List<Work> AggregateByPosts(List<Work> works)
        {
            return works.AggregateWorksByPosts();
        }
        public List<Work> SaveWorks(List<Work> works)
        {
            List<Work> result = new List<Work>();
            BaseContext c = _c ?? new BaseContext(_accName);
            {
                foreach (var work in works)
                {
                    
                    if (work.Id is 0 or default(long))
                    {
                        //new work;
                        Work clone = (Work) work.Clone();
                        c.Works.Add(clone);
                        result.Add(clone);
                    }
                    else
                    {
                        var exist = c.Works.FirstOrDefault(x => x.Id == work.Id);
                        if (exist != null)
                        {
                            exist.TakeNewInfo(work);
                            result.Add(exist);
                        }
                        else
                        {
                            throw new Exception("Wrong work id");
                        }
                    }
                }

                c.SaveChanges();
            }
            if (_c == null)
            {
                c.Dispose();
            }
            return result;
        }
       /* 
        public List<Work> SaveNewWorks(List<Work> works)
        {
         //   works = works.Distinct().ToList();
            works = works.AggregateWorksByPosts();
            List<Work> result = new List<Work>();
            using (BaseContext c = new BaseContext(_accName))
            {
                var orderedWorks = works.GroupBy(x => x.OrderNumber).ToList();
                foreach (var orderWorks in orderedWorks)
                {
                    var existOrderWorks = c.Works.AsNoTracking().Where(x => x.OrderNumber == orderWorks.Key);
                    var newWorks = orderWorks.Except(existOrderWorks);
                    c.Works.AddRange(newWorks);
                    result.AddRange(newWorks);
                }

                  c.SaveChanges();
                  return result;
            }
        }
        public List<Work> SaveWorksChanges(List<Work> works)
        {
            using (BaseContext c = new BaseContext(_accName))
            {
                List<long> worksIds = works.Select(x => x.Id).ToList();
                var baseWorks = c.Works.Where(x => worksIds.Contains(x.Id));
                foreach (var work in works)
                {
                    var baseWork = baseWorks.First(x => x.Id == work.Id);
                    //baseWork.Status = work.Status;
                    baseWork.TakeNewInfo(work);
                }

                 c.SaveChanges();
                 return baseWorks.ToList();
            }
        }*/

      

        
     
    }
    
    public class WorkCreateManager
    {
        private BaseContext? _c = null;
        private List<PostCreationKey> _postCreationKeys;

        
        public WorkCreateManager(List<PostCreationKey>? post_key_cache = null, BaseContext? c=null)
        {
            _c = c;
            if (post_key_cache == null)
            {
                BaseContext bc = _c ?? new BaseContext("");
                {
                 _postCreationKeys = bc.PostKeys.AsNoTracking().ToList();
                }
                if (_c == null)
                {
                    bc.Dispose();
                }
            }
            else
            {
                _postCreationKeys = post_key_cache;
            }
        }

      

        public List<Work> CreateWorks(List<WorkCreateTemplate> templates, bool exceptExist)
        {
            List<Work> works = new List<Work>();
            foreach (var template in templates)
            {
                var work = CreateWork(template);
                works.Add(work);
            }

            if (exceptExist)
            {
                BaseContext c = _c ?? new BaseContext("");
                {
                    List<Work> filtered = new List<Work>();
                    foreach (var w in works)
                    {
                        var existWorks = c.Works.AsNoTracking().FirstOrDefault(x =>
                            x.OrderNumber == w.OrderNumber && x.Article == w.Article && x.PostId == w.PostId && x.OrderLineNumber == w.OrderLineNumber);
                        if (existWorks == null)
                        {
                            filtered.Add(w);
                        }
                    }
                    works = filtered;
                }
                if (_c == null)
                {
                    c.Dispose();
                }
            }
            var aggregated = works.AggregateWorksByPosts();

            
            return aggregated.OrderBy(x=>x.OrderNumber).ThenBy(x=>x.OrderLineNumber).ThenBy(x=>x.PostId).ToList();
        }

     
        private Work CreateWork(Work work)
        {
            Work w = (Work)work.Clone();
            return w;

        }
        private Work CreateWork(WorkCreateTemplate template)
        {
            Work w = new Work();
            w.OrderLineNumber = template.OrderLineNumber;
            w.Article = template.Article;
            w.Count = template.Count;
            w.OrderNumber = template.OrderNumber;
            w.SingleCost = template.SingleCost;
            w.Status = WorkStatus.hidden;
            w.Description = template.Description;
            w.ProductLineId = template.ProductLine;
            w.CreatedStamp = DateTime.Now;
            w.Comments = template.Comment.Split('\r',StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim()).Where(x=>x.Length>0).ToList();
            w.MovedFrom = "";
            w.MovedTo = "";
            w.DeadLine = template.DeadLine;

            var suggestablePost = this._postCreationKeys.FirstOrDefault(x =>
                x.Key.Equals(template.PostKey, StringComparison.InvariantCultureIgnoreCase));
            if (suggestablePost != null)
            {
                w.PostId = suggestablePost.PostId;
                //throw new ArgumentException($"PostKey: {template.PostKey} not addicted to any post");
            }
            else
            {
                w.PostId = "";
            }

           

            return w;
        }


        public Work CreateAdditionalWork(string postId,string prodLine, string accName, List<string> cooment, string description="", string article="" )
        {
            Work w = new Work();
            if (string.IsNullOrEmpty(description))
            {
                w.Description = $"Доп. работа {postId}";
            }
            else
            {
                w.Description = description;
            }

            if (string.IsNullOrEmpty(article))
            {
                w.Article = $"Работа {prodLine}";    
            }
            else
            {
                w.Article = article;
            }
            
            w.Count = 1;
            w.MovedFrom = "";
            w.MovedTo = "";
            w.DeadLine = DateTime.Today;
            w.PostId = postId;
            w.Status = WorkStatus.waiting;
            w.Comments = cooment;
            w.CreatedStamp = DateTime.Now;
            w.Priority = 10;
            w.ProductLineId = prodLine;
            w.OrderNumber = 100;
            WorkSaveManager wsm = new WorkSaveManager(accName);
            var work = wsm.SaveWorks(new List<Work>() { w })[0];
            return work;
        }
    }

    public static class WorkAggregator
    {
        public static List<Work> AggregateWorksByPosts(this List<Work> works)
        {
            List<Work> Aggregated = new List<Work>();
            var order = works.GroupBy(x => x.OrderNumber);
            foreach (var orderGroup in order)
            {
                var article = orderGroup.GroupBy(x => x.Article);
                foreach (var articleGroup in article)
                {
                    var post = articleGroup.GroupBy(x => x.PostId);
                    foreach (var postGroup in post)
                    {
                        var lineGroup = postGroup.GroupBy(x => x.OrderLineNumber);
                        foreach (var line in lineGroup)
                        {
                            Work w = (Work)line.First().Clone();
                            w.SingleCost = postGroup.Sum(x => x.SingleCost);
                            w.Comments = postGroup.SelectMany(x=>x.Comments).ToList();
                            Aggregated.Add(w);
                        }
                      
                        /*
                        Work w = new Work();
                        w.OrderNumber = orderGroup.Key;
                        w.Article = articleGroup.Key;
                        w.PostId = postGroup.Key;
                        w.SingleCost = postGroup.Sum(x => x.SingleCost);
                        w.Count = postGroup.First().Count;
                        w.Status = WorkStatus.hidden;
                        Aggregated.Add(w); /*/
                    }
                }
            }

            return Aggregated;
        }
        public static List<Work> AggregateWorksByArticle(this List<Work> works)
        {
            List<Work> Aggregated = new List<Work>();
            var order = works.GroupBy(x => x.OrderNumber);
            foreach (var orderGroup in order)
            {
                var article = orderGroup.GroupBy(x => x.Article);
                foreach (var articleGroup in article)
                {
                  
                        Work w = (Work)articleGroup.First().Clone();
                        w.PostId = "Все посты";
                        w.Post = null;
                        w.SingleCost = articleGroup.Sum(x => x.SingleCost);
                        w.Comments = articleGroup.SelectMany(x=>x.Comments).ToList();
                        Aggregated.Add(w);
                      
                    
                }
            }

            return Aggregated;
        }
        public static List<Work> AggregateWorksByOrder(this List<Work> works)
        {
            List<Work> Aggregated = new List<Work>();
            var order = works.GroupBy(x => x.OrderNumber);
            foreach (var orderGroup in order)
            {
                Work w = (Work)orderGroup.First().Clone();
                w.Article = "Все артикулы";
                w.PostId = "Все посты";
                w.SingleCost = orderGroup.Sum(x => x.SingleCost);
                w.Comments = orderGroup.SelectMany(x => x.Comments).ToList();
                Aggregated.Add(w);
            }

            return Aggregated;
        }
    }


    
    public class WorkCreateTemplate
    {
        public string Article { get; set; }
        public long OrderNumber { get; set; }
        public int OrderLineNumber { get; set; }
        public decimal SingleCost { get; set; }
        public int Count { get; set; }
        public string PostKey { get; set; }
        public string Description { get; set; } = "";
        public string ProductLine { get; set; } = "";
        public string Comment { get; set; } = "";
        public DateTime DeadLine { get; set; } = DateTime.Today;

    }
}