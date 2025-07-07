using KSK_LIB.Maconomy;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class OTKManager
{
   
   private BaseContext bc;
   public OTKManager(string user,BaseContext c=null)
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

   public OTKCheck Process(OTKCheck otkCheck, string user)
   {
      WorkManagerFacade wmf = new WorkManagerFacade(bc);
      if (otkCheck.ProductCount != otkCheck.CheckedCount)
      {
         var newWork = wmf.SplitWork(otkCheck.WorkId,(int)(otkCheck.ProductCount - otkCheck.CheckedCount));
      }

      if (otkCheck.Lines.All(x => x.Value != "Брак есть"))
      {
         WorkStatusChanger wsc = new WorkStatusChanger(bc);
         wsc.ChangeStatus(otkCheck.WorkId, WorkStatus.ended, user);
         wmf.MoveToPostRequest(otkCheck.WorkId, Constants.Work.EndPosts.TotalEnd, new List<string>(), "", out var error);
         return SaveCheck(otkCheck);   
      }
      else
      {
         IssueManager im = new IssueManager(bc);
         string from = bc.Works.AsNoTracking().First(x => x.Id == otkCheck.WorkId).MovedFrom;
         var otkIssue = bc.IssueTemplates.FirstOrDefault(x => x.Name.Contains("ОТК"));
         if (otkIssue == null)
         {
            otkIssue = new WorkIssueTemplate();
            otkIssue.IsVisible = true;
            otkIssue.Name = "Блокировка ОТК";
            bc.IssueTemplates.Add(otkIssue);
            bc.SaveChanges();
         }
         
         string desc = string.Join(",",otkCheck.Lines.Where(x=>x.Value=="Брак есть").Select(x=>$"{x.ShortName} {x.Description}"));
         im.RegisterIssue(otkCheck.WorkId, otkIssue.Id, desc, user, from, "");
      }

      return otkCheck;

   }
   
   public OTKCheck SaveCheck(OTKCheck otkCheck)
   {
      OTKCheck toSave = new OTKCheck();
      toSave.Iteration = otkCheck.Iteration;
      toSave.OrderNumber = otkCheck.OrderNumber;
      toSave.ProductLine = otkCheck.ProductLine;
      toSave.Article = otkCheck.Article;
      toSave.CheckedCount = otkCheck.CheckedCount;
      toSave.OrderLineNumber = otkCheck.OrderLineNumber;
      toSave.ProductCount = otkCheck.ProductCount;
      toSave.WorkId = otkCheck.WorkId;
      toSave.Stamp = otkCheck.Stamp;
      toSave.Worker = otkCheck.Worker;

      toSave.Lines = new List<OTKCheckLine>();
      foreach (var line in toSave.Lines)
      {
         var toSaveLine = new OTKCheckLine();
         toSaveLine.FullName = line.FullName;
         toSaveLine.ShortName = line.ShortName;
         toSaveLine.Description = line.Description;
         toSaveLine.Value = line.Value;
         
         toSave.Lines.Add(toSaveLine);
      }
      bc.OTKChecks.Add(toSave);
      bc.SaveChanges();
      return toSave;
   }
   
  
   
   public OTKCheck Template(long workId)
   {
      var work = bc.Works.FirstOrDefault(x => x.Id == workId);
      if (work == null)
      {
         return null;
      }
      OTKCheck otkCheck = new OTKCheck();
      otkCheck.Article = work.Article;
      otkCheck.OrderNumber = work.OrderNumber;
      otkCheck.OrderLineNumber = work.OrderLineNumber;
      otkCheck.ProductCount = work.Count;
      otkCheck.Stamp = DateTime.Now;
      otkCheck.ProductLine = work.ProductLineId;
      otkCheck.CheckedCount = work.Count;
      otkCheck.WorkId = workId;

   

      otkCheck.Iteration = bc.OTKChecks.Count(x => x.OrderNumber ==  work.OrderNumber && x.OrderLineNumber == work.OrderLineNumber)+1;
      otkCheck.Lines = new List<OTKCheckLine>();

      var operations = bc.OTKAvailableOperations.Where(x=>x.ProductLine.Contains(work.ProductLineId)).ToList();
      foreach (var op in operations)
      {
         OTKCheckLine otkCheckLine = new OTKCheckLine();
         otkCheckLine.ShortName = op.ShortName;
         otkCheckLine.FullName = op.FullName;
         otkCheckLine.Description = "";
         otkCheckLine.Value = "";

         if (otkCheckLine.ShortName.Contains("[Габариты изделия]") || otkCheckLine.FullName.Contains("[Габариты изделия]"))
         {
            using (MaconomyBase mb = new MaconomyBase())
            {
               var t = mb.getTableFromDB(
                  $"SELECT SUPPLEMENTARYTEXT9 from iteminformation where iteminformation.itemnumber = '{otkCheck.Article}'");
               if (t.Rows.Count > 0)
               {
                  string s = MaconomyBase.makeStringRu(t.Rows[0][0].ToString());
                  otkCheckLine.FullName = otkCheckLine.FullName.Replace("[Габариты изделия]", s);
                  otkCheckLine.ShortName = otkCheckLine.ShortName.Replace("[Габариты изделия]", s);
               }
            }
         }
         
         otkCheck.Lines.Add(otkCheckLine);
      }

      return otkCheck;
   }

   public List<OTKWorker> Workers()
   {
      return bc.OTKWorkers.AsNoTracking().ToList();
   }

   public List<OTKWorker> SaveWorkers(List<OTKWorker> workers)
   {
      var existList = bc.OTKWorkers.ToList();
      foreach (var worker in workers)
      {
         var exist = existList.FirstOrDefault(x => x.Id == worker.Id || x.Name == worker.Name);
         if (exist == null)
         {
            exist = new OTKWorker();
            bc.OTKWorkers.Add(exist);
         }
         exist.Name = worker.Name;
         bc.SaveChanges();
      }
      existList = bc.OTKWorkers.ToList();
      if (existList.Count > workers.Count)
      {
         List<OTKWorker> removeWorkers = new List<OTKWorker>();
         foreach (var exist in existList)
         {
            var inList = workers.FirstOrDefault(x=>x.Id==exist.Id);
            if (inList == null)
            {
               removeWorkers.Add(exist);
            }
         }
         bc.OTKWorkers.RemoveRange(removeWorkers);
         bc.SaveChanges();
      }

      return bc.OTKWorkers.ToList();
   }

   public List<OTKAvailableOperation> Operations()
   {
      return bc.OTKAvailableOperations.AsNoTracking().ToList();
   }

   public List<OTKAvailableOperation> SaveOperations(List<OTKAvailableOperation> operations)
   {
      var existList = bc.OTKAvailableOperations.ToList();
      foreach (var toSave in operations)
      {
         var exist = existList.FirstOrDefault(x => x.Id == toSave.Id);
         if (exist==null)
         {
            //new item;
            exist = new OTKAvailableOperation();
            bc.OTKAvailableOperations.Add(exist);
         }
         exist.FullName = toSave.FullName;
         exist.ShortName = toSave.ShortName;
         exist.ProductLine = toSave.ProductLine;

         bc.SaveChanges();
      }
      existList = bc.OTKAvailableOperations.ToList();
      if (existList.Count > operations.Count)
      {
         List<OTKAvailableOperation> removeOperations = new List<OTKAvailableOperation>();
         foreach (var exist in existList)
         {
            var inList = operations.FirstOrDefault(x=>x.Id==exist.Id);
            if (inList == null)
            {
               removeOperations.Add(exist);
            }
         }
         bc.OTKAvailableOperations.RemoveRange(removeOperations);
         bc.SaveChanges();
      }
      return bc.OTKAvailableOperations.ToList();
   }
}