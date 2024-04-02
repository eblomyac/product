using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class PostStatisticManager
    {
        private PostStatistic _getPostStat (long order, string postId, DateTime stamp, IEnumerable<Work> works)
        {
            PostStatistic ps = new PostStatistic();
            ps.Stamp = stamp;
            ps.ActualEvents = works.Sum(x => x.Issues.Count(z=>z.Resolved==null));
            ps.IncomeCost = works.Where(x => x.Status == WorkStatus.income).Sum(x=>x.TotalCost);
            ps.PredictCost = works.Where(x => x.Status == WorkStatus.hidden).Sum(x=>x.TotalCost);
            ps.WaitingCost = works.Where(x => x.Status == WorkStatus.waiting).Sum(x=>x.TotalCost);
            ps.RunningCost = works.Where(x => x.Status == WorkStatus.running).Sum(x=>x.TotalCost);
            ps.SendedCost = works.Where(x => x.Status == WorkStatus.sended).Sum(x=>x.TotalCost);

            ps.OrderNumber = order;
            ps.PostId = postId;
            return ps;
        }
        public List<PostStatistic> GetStat()
        {
            using (BaseContext c = new BaseContext("system"))
            {
                DateTime stamp = DateTime.Now;
                var lastStamp = c.PostStatistics.OrderBy(x=>x.Stamp).OrderBy(x=>x.Stamp).LastOrDefault()?.Stamp;
                if (lastStamp.HasValue)
                {
                    if ((stamp - lastStamp.Value).TotalMinutes < 15)
                    {
                        return new List<PostStatistic>();
                    }    
                }
                List<PostStatistic> result = new List<PostStatistic>();
                
                var works = c.Works.AsNoTracking()
                    .Include(x=>x.Issues)
                    .Where(x => x.Status != WorkStatus.ended && x.Status!= WorkStatus.unkown)
                    .ToList();
                var orders = works.Select(x => x.OrderNumber).Distinct().ToList();
                var posts = c.Posts.Select(x=>x.Name).ToList();
                foreach (var post in posts)
                {
                    foreach (var order in orders)
                    {
                         result.Add(_getPostStat(order,post,stamp,works.Where(x => x.OrderNumber == order && x.PostId == post)));
                    }    
                }
                return result;
            }
        }

        public void SaveStat(List<PostStatistic> sts)
        {
            if (sts.Count==0)
            {
             return;
             
            }
            using (BaseContext c = new BaseContext("system"))
            {
                c.PostStatistics.AddRange(sts);
                c.SaveChanges();
            }
        }
    }
}