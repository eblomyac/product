using System.Dynamic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class OperatorManager
{
    public class OperatorCountChangeInstruction
    {
        [JsonPropertyName("workId")]
        public long WorkId { get; set; }
        [JsonPropertyName("newCount")]
        public int NewCount { get; set; }
        [JsonPropertyName("oldCount")]
        public int OldCount { get; set; }
        [JsonPropertyName("result")]
        public string Result { get; set; }
    }
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

    public async Task<dynamic> WorkListForCountChange(long orderNumber,int orderLine)
    {
        dynamic result = new ExpandoObject();
        MaconomyOrderMaxCountManager maxCountManager = new MaconomyOrderMaxCountManager(orderNumber.ToString());
        decimal currentCount =await maxCountManager.GetCount(orderNumber, orderLine);
        using (BaseContext c = new BaseContext())
        {
            result.Posts = new List<object>();
            var works = c.Works.Include(x=>x.Post)
                .Where(x => x.OrderNumber == orderNumber && x.OrderLineNumber == orderLine).OrderBy(x=>x.Post.ProductOrder).ToList();
            var postWorks = works.GroupBy(x => x.PostId);
            foreach (var workGroup in postWorks)
            {
                dynamic post = new ExpandoObject();
                post.PostName = workGroup.Key;
                post.Works = workGroup.ToList();
                post.TotalCount = workGroup.Sum(x => x.Count);
                result.Posts.Add(post);
            }

            if (works.Count > 0)
            {
                result.Article = works[0].Article;    
            }
            else
            {
                result.Article = "Не опознано";
            }
            
            result.Works = works;
            result.CurrentCount = (int)currentCount;
            //result.IsAllSame = works.All(x => x.Count == (int)currentCount);
        }
        return result;
    }

    public List<OperatorCountChangeInstruction> ChangeCount(string accName, List<OperatorCountChangeInstruction> changeInstructions )
    {
        

        using (BaseContext c = new ())
        {
            List<long> workIds = changeInstructions.Select(x => x.WorkId).ToList();
            var works = c.Works.Where(z => workIds.Contains(z.Id)).ToList();
            var workLogs = c.WorkStatusLogs.Where(z => workIds.Contains(z.Id)).ToList();

            foreach (var ci in changeInstructions)
            {
                var work = works.FirstOrDefault(x => x.Id == ci.WorkId);

                if (work == null)
                {
                    //wtf ??? 
                    ci.Result = "Не удалось найти работу";
                    continue;
                }

                using (var transaction = c.Database.BeginTransaction())
                {
                    OperatorCountChangeRecord ocr = new();
                    ocr.Stamp = DateTime.Now;
                    ocr.Article = work.Article;
                    ocr.EditBy = accName;
                    ocr.WorkId = ci.WorkId;
                    ocr.OrderNumber = work.OrderNumber;
                    ocr.LineNumber = work.OrderLineNumber;
                    ocr.NewCount = ci.NewCount;
                    ocr.OldCount = work.Count;
                    ocr.StatusWhenChanged = work.Status;

                    work.Count = ci.NewCount;

                    var logs = workLogs.Where(x => x.WorkId == work.Id);
                    foreach (var log in logs)
                    {
                        log.Count = ci.NewCount;
                    }

                    c.OperatorCountChangeRecords.Add(ocr);
                    try
                    {
                        bool r = c.SaveChanges()>0;
                        transaction.Commit();
                        ci.Result = r?"Успешно":"Ничего не изменилось";
                    }
                    catch (Exception exc)
                    {
                        ci.Result = $"Ошибка: {exc.Message}";
                        transaction.Rollback();
                    }
                    
                    
                }
 
                

            }

            return changeInstructions;
        }
        
    }
}