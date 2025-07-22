using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class HrManager
{
    
    private BaseContext bc;
    public HrManager(string user,BaseContext c=null)
    {
        if (c == null)
        {
            bc = new BaseContext(user);
        }
        else
        {
            bc = c;
        }
    }

    public List<ProductWorker> WorkerList()
    {
        return bc.ProductWorkers.Include(x=>x.Targets).AsNoTracking().OrderBy(x=>x.Name).ToList();
    }

    public Dictionary<string, List<CrpManager.CrpTarget>> ProductTargets()
    {
        CrpManager crp = new CrpManager();
        List<PostCreationKey> PostKeys = bc.PostKeys.ToList();
        
        
        var a = crp.GetTargetHrList(PostKeys.Select(x=>x.Key).ToList());

        Dictionary<string, List<CrpManager.CrpTarget>> b = new();
        foreach (var pair in a)
        {
            string postKey = pair.Key;
            var existPostKey = PostKeys.FirstOrDefault(x => x.Key == postKey);
            if (existPostKey != null)
            {
                string postId = existPostKey.PostId;
                if (!b.ContainsKey(postId))
                {
                    b.Add(postId, new List<CrpManager.CrpTarget>());
                }
                b[postId].AddRange(pair.Value);
                
            }
        }

        return b;




    }

    public class CalendarData
    {
        public List<ProductCalendarRecord> Data { get; set; }
        public List<ProductWorker> Workers { get; set; }
    }
    public CalendarData CalendarList(int month, int year)
    { 
        CalendarData c = new CalendarData();
        
        c.Data = new();
       var existRecords = bc.ProductCalendarRecords.AsNoTracking().Where(x => x.Month == month && x.Year == year).ToList();
       c.Data.AddRange(existRecords);
       var activeWorkers = bc.ProductWorkers.Where(x=>x.IsActive).ToList();
       var existTargets = bc.ProductTargets.ToList();
       foreach (var worker in activeWorkers)
       {
           foreach (var target in existTargets.Where(x=>x.ProductWorkerId == worker.Id))
           {
               for (int loop = 1; loop <= DateTime.DaysInMonth(year, month); loop++)
               {
                   var existRecord = existRecords.FirstOrDefault(x=>x.Day == loop && x.Month == month && x.Year == year 
                                                                    && x.TargetName == target.TargetName && x.ProductWorkerName==worker.Name && x.PostId == target.PostId);
                   if (existRecord == null)
                   {
                       existRecord = new ProductCalendarRecord();
                       existRecord.Month = month;
                       existRecord.Year = year;
                       
                       existRecord.TargetCrpCenter = target.TargetCrpCenter;
                       existRecord.TargetCrpPost = target.TargetCrpPost;
                       existRecord.TargetCrpPostDescription = target.TargetCrpPostDescription;
                       existRecord.TargetCrpCenterDescription = target.TargetCrpCenterDescription;
                       
                       existRecord.ProductWorkerName = worker.Name;
                       existRecord.PostId = target.PostId;
                       existRecord.Day = loop;
                       existRecord.Id = 0;
                       existRecord.PlanningHours = 0;
                       existRecord.EffectiveHours = 0;
                       existRecord.PlanToWorkConst = 1;
                       existRecord.Description = "";
                       c.Data.Add(existRecord);
                   }
               }
           }
       }
      c.Data = c.Data.OrderBy(x=>x.PostId).ThenBy(x=>x.ProductWorkerName).ThenBy(x=>x.TargetName).ToList();
      c.Workers = new List<ProductWorker>();
      var workerNameList = c.Data.Select(x=>x.ProductWorkerName).Distinct().ToList();
      foreach (var worker in workerNameList)
      {
          ProductWorker pw = new ProductWorker();
          c.Workers.Add(pw);
          pw.Name = worker;
          pw.Targets= new List<ProductTarget>();
          var workerPosts = c.Data.Where(x=>x.ProductWorkerName == worker).Select(x=>x.PostId).Distinct().ToList();
          foreach (var post in workerPosts)
          {
              var workerPostTargets = c.Data.Where(x=>x.ProductWorkerName == worker && x.PostId==post).Distinct().ToList();
              foreach (var target in workerPostTargets)
              {
                  var exist = pw.Targets.FirstOrDefault(x => x.TargetName == target.TargetName);
                  if (exist == null)
                  {
                      pw.Targets.Add(new ProductTarget()
                      {
                          PostId = post, ProductWorkerId = 0, Id = 0,
                          TargetCrpCenter = target.TargetCrpCenter,
                          TargetCrpCenterDescription = target.TargetCrpCenterDescription,
                          TargetCrpPost = target.TargetCrpPost,
                          TargetCrpPostDescription = target.TargetCrpPostDescription,
                      });
                  }
              }
          }
      }
      return c;
    }

    public List<ProductCalendarRecord> SaveCalendarRecords(List<ProductCalendarRecord> list)
    {
        
        var existRecords = list.Where(x=>x.Id!=0).ToList();
        var existRecordIds = existRecords.Select(x=>x.Id).ToList();
        var toEdit = bc.ProductCalendarRecords.Where(x => existRecordIds.Contains(x.Id)).ToList();
        
        
        
        List<ProductCalendarRecord> result = new List<ProductCalendarRecord>();
        foreach (var existRecord in toEdit)
        {
            var newRecord= existRecords.FirstOrDefault(z => z.Id == existRecord.Id);
            if (newRecord != null)
            {
                existRecord.PlanningHours = newRecord.PlanningHours;
                existRecord.PlanToWorkConst = newRecord.PlanToWorkConst;
            }
            result.Add(existRecord);
        }
        bc.SaveChanges();
        foreach (var newRecord in list.Where(x=>x.Id==0))
        {
            var newRecordEF = new ProductCalendarRecord();
            newRecordEF.PostId = newRecord.PostId;
            newRecordEF.PlanningHours = newRecord.PlanningHours;
            newRecordEF.PlanToWorkConst = newRecord.PlanToWorkConst;
            newRecordEF.Day = newRecord.Day;
            newRecordEF.Month = newRecord.Month;
            newRecordEF.Year = newRecord.Year;
            newRecordEF.TargetCrpCenter = newRecord.TargetCrpCenter;
            newRecordEF.TargetCrpCenterDescription = newRecord.TargetCrpCenterDescription;
            newRecordEF.TargetCrpPost = newRecord.TargetCrpPost;
            newRecordEF.TargetCrpPostDescription = newRecord.TargetCrpPostDescription;
            newRecordEF.ProductWorkerName = newRecord.ProductWorkerName;
            newRecordEF.EffectiveHours = newRecord.EffectiveHours;
            newRecordEF.Description = newRecord.Description;
            bc.ProductCalendarRecords.Add(newRecordEF);
            result.Add(newRecordEF);
        }

        bc.SaveChanges();
        return result;
    }

    public List<ProductWorker> SaveTargetList(List<ProductWorker> workers)
    {
        var currentWorkers = bc.ProductWorkers.Include(x=>x.Targets).ToList();
        foreach (var worker in workers)
        {
            var currentWorker = currentWorkers.FirstOrDefault(x=>x.Id == worker.Id);
            if (currentWorker != null)
            {
                foreach (var target in worker.Targets)
                {
                    var existTarget = currentWorker.Targets.FirstOrDefault(x =>
                        x.PostId == target.PostId && x.TargetName == target.TargetName);
                    if (existTarget == null)
                    {
                        existTarget = new();
                        existTarget.PostId = target.PostId;
                        //existTarget.TargetName = target.TargetName;
                        existTarget.TargetCrpCenter = target.TargetCrpCenter;
                        existTarget.TargetCrpPost = target.TargetCrpPost;
                        existTarget.TargetCrpPostDescription = target.TargetCrpPostDescription;
                        existTarget.TargetCrpCenterDescription = target.TargetCrpCenterDescription;
                        
                        existTarget.ProductWorkerId = currentWorker.Id;
                        bc.ProductTargets.Add(existTarget);
                    }
                }
                foreach (var currentTarget in currentWorker.Targets)
                {
                    var target = worker.Targets.FirstOrDefault(x=>x.PostId == currentTarget.PostId && x.TargetName == currentTarget.TargetName);
                    if (target == null)
                    {
                        bc.ProductTargets.Remove(currentTarget);
                    }
                }

                bc.SaveChanges();
            }
        }

        return WorkerList();
    }

    public List<ProductWorker> SaveWorkerList(List<ProductWorker> workers)
    {
        var existWorkers = bc.ProductWorkers.ToList();
        var toRemove = new List<ProductWorker>();
        foreach (var productWorker in workers)
        {
            var eWorker = existWorkers.FirstOrDefault(x => x.Name == productWorker.Name);
            if ( eWorker== null)
            {
                eWorker = new ProductWorker();
                bc.ProductWorkers.Add(eWorker);
            }
            
                eWorker.IsActive = productWorker.IsActive;
                eWorker.Title = productWorker.Title;
                eWorker.Name = productWorker.Name;
                
                
            

            bc.SaveChanges();
        }

     

        bc.SaveChanges();
        return WorkerList();
    }

 
}