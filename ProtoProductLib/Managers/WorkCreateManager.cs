using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProtoProductLib.BaseStruct;

namespace ProductLibPrototype.Managers
{
   
    
    public class WorkCreateManager
    {
        private List<PostCreationKey> _postCreationKeys;

        public WorkCreateManager(List<PostCreationKey> post_key_cache = null)
        {
            if (post_key_cache == null)
            {
                using (ProductContext c = new ProductContext())
                {
                  //  _postCreationKeys = c.PostKeys.AsNoTracking().ToList();
                }
            }
            else
            {
                _postCreationKeys = post_key_cache;
            }
        }

        public List<Work> CreateWorks(List<WorkCreateTemplate> templates)
        {
            List<Work> works = new List<Work>();
            foreach (var template in templates)
            {
                var work = CreateWork(template);
                works.Add(work);
            }

            return works;
        }

        private Work CreateWork(WorkCreateTemplate template)
        {
            Work w = new Work();
            w.Article = template.Article;
            w.Count = template.Count;
            w.OrderNumber = template.OrderNumber;
            w.SingleCost = template.SingleCost;
            w.Status = WorkStatus.hidden;

            var suggestablePost = this._postCreationKeys.FirstOrDefault(x =>
                x.Key.Equals(template.PostKey, StringComparison.InvariantCultureIgnoreCase));
            if (suggestablePost == null)
            {
                throw new ArgumentException($"PostKey: {template.PostKey} not addicted to any post");
            }

            w.PostId = suggestablePost.PostId;

            return w;
        }
    }

    public class WorkCreateTemplate
    {
        public string Article { get; set; }
        public long OrderNumber { get; set; }
        public float SingleCost { get; set; }
        public int Count { get; set; }
        public string PostKey { get; set; }
        
    }
}