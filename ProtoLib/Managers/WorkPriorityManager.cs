using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public static class WorkPriorityData{

    public static int GetWorkPrioritu(this List<WorkPriority> data, string article, long order)
    {
        int result = 10;
        var orderPriority =
            data.FirstOrDefault(x => x.OrderNumber == order && x.Article.Length < 1);
        var articlePriority =
            data.FirstOrDefault(x => x.OrderNumber == order && x.Article == article);
        if (orderPriority == null && articlePriority == null)
        {
            result=10;
        }
        if (orderPriority != null)
        {
            result= orderPriority.Priority;
        }
        if (articlePriority != null)
        {
           result = articlePriority.Priority;
        }

        return result;
    }
}
public class WorkPriorityManager
{

    public class OrderPriority
    {
        public long OrderNumber { get; set; }
        public int Priority { get; set; }
        public List<ArticlePriority> Articles { get; set; }
    }

    public class ArticlePriority
    {
        public long OrderNumber { get; set; }
        public string Article { get; set; }
        public int Priority { get; set; }
    }

    public List<WorkPriority> WorkPriorityList(List<long> orders)
    {
        using (BaseContext c = new BaseContext(""))
        {
            return c.WorkPriorities.AsNoTracking().Where(x => orders.Contains(x.OrderNumber)).ToList();
        }
    }
    public List<OrderPriority> PriorityList()
    {
        using (BaseContext c = new BaseContext(""))
        {
            var actualWorks = c.Works.AsNoTracking().Where(x => x.Status != WorkStatus.ended).ToList();
            var orderGroup = actualWorks.GroupBy(x => x.OrderNumber);
            var actualOrderList = actualWorks.Select(x => x.OrderNumber).Distinct().ToList();
            var savedPriority = c.WorkPriorities.AsNoTracking().Where(x => actualOrderList.Contains(x.OrderNumber)).ToList();

            List<OrderPriority> result = new List<OrderPriority>();
            foreach (var order in orderGroup)
            {
                OrderPriority d = new OrderPriority();
                d.OrderNumber = order.Key;
                d.Priority = 10;
                result.Add(d);
                var existOrderPriority =
                    savedPriority.FirstOrDefault(x => x.OrderNumber == order.Key && x.Article == "");
                if (existOrderPriority != null)
                {
                    d.Priority = existOrderPriority.Priority;
                }
                
                d.Articles = new List<ArticlePriority>();
                var articles = order.Select(x => x.Article).Distinct().ToList();

                foreach (var article in articles)
                {
                    ArticlePriority a = new ArticlePriority();
                    a.Article = article;
                    a.Priority = 10;
                    a.OrderNumber = order.Key;
                    var existArticlePriority =
                        savedPriority.FirstOrDefault(x => x.Article == article && x.OrderNumber == order.Key);
                    if (existArticlePriority != null)
                    {
                        a.Priority = existArticlePriority.Priority;
                    }
                    d.Articles.Add(a);
                }
            }

            return result;

        }
    }

    public string SaveList(List<OrderPriority> orderPriorities)
    {
        using (BaseContext c = new BaseContext(""))
        {
            List<long> orders = orderPriorities.Select(x => x.OrderNumber).ToList();
            var existPriorities = c.WorkPriorities.Where(x => orders.Contains(x.OrderNumber)).ToList();
            foreach (var orderPriority in orderPriorities)
            {
                var exist = existPriorities.FirstOrDefault(x => x.OrderNumber == orderPriority.OrderNumber && x.Article.Length<1);
                if (exist == null && orderPriority.Priority != 10)
                {
                    //should be added
                    WorkPriority wp = new WorkPriority();
                    wp.OrderNumber = orderPriority.OrderNumber;
                    wp.Priority = orderPriority.Priority;
                    wp.Article = "";
                    wp.DateChange=DateTime.Now;
                    c.WorkPriorities.Add(wp);
                }else if (exist != null)
                {
                    exist.Priority = orderPriority.Priority;
                    exist.DateChange = DateTime.Now;
                }

                foreach (var articlePriority in orderPriority.Articles)
                {
                    var existArticle = existPriorities.FirstOrDefault(x =>
                        x.OrderNumber == orderPriority.OrderNumber && x.Article == articlePriority.Article);
                    if (existArticle == null && (articlePriority.Priority != 10 || orderPriority.Priority != 10))
                    {
                        //should be added
                        WorkPriority wp = new WorkPriority();
                        wp.OrderNumber = orderPriority.OrderNumber;
                        wp.Article = articlePriority.Article;
                        wp.Priority = articlePriority.Priority;
                        wp.DateChange = DateTime.Now;
                        c.WorkPriorities.Add(wp);
                    }else if (existArticle != null)
                    {
                        existArticle.Priority = articlePriority.Priority;
                        existArticle.DateChange = DateTime.Now;
                    }
                }
            }

            c.SaveChanges();
            return "";
        }
    }
}