using System.Data;
using System.Text.RegularExpressions;
using KSK_LIB.Maconomy;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class OTKFilter
{
   public DateTime? From { get; set; }
   public DateTime? To { get; set; }
   public string? Worker { get; set; }
   public string? Result { get; set; }
   public string? Article { get; set; }
   public long? OrderNumber { get; set; }
   public int? Offset { get; set; }
}
public class OTKManager
{


   public DataTable OTKReport(DateTime from, DateTime? to)
   {
      DataTable t = new DataTable();
      t.Columns.Add("Номер акта");
      t.Columns.Add("Заказ", typeof(long));
      t.Columns.Add("Артикул");
      t.Columns.Add("Производственная линия");
      t.Columns.Add("Дата акта");
      return t;
   }
   
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

   public List<OTKCheck> GetOTKChecks(OTKFilter filter )
   {
      var q = bc.OTKChecks.Include(x => x.Lines).Where(x=>x.Id>0);
      if (filter.From.HasValue)
      {
         q = q.Where(x => x.Stamp.Date >= filter.From.Value.Date);
      }

      if (filter.To.HasValue)
      {
         q=q.Where(x => x.Stamp.Date <= filter.To.Value.Date);
      }

      if (!string.IsNullOrEmpty(filter.Worker))
      {
         q=q.Where(x => x.Worker==filter.Worker);
      }

      if (!string.IsNullOrEmpty(filter.Article))
      {
         q=q.Where(x => x.Article.Contains(filter.Article));
      }

      if (filter.OrderNumber.HasValue)
      {
         q = q.Where(x=>x.OrderNumber==filter.OrderNumber);
      }

      if (!string.IsNullOrEmpty(filter.Result))
      {
         if (filter.Result == "Брак есть")
         {
            q = q.Where(x => x.Lines.Count(z => z.Value == filter.Result) > 0);
         }

         else if (filter.Result == "Брака нет")
         {
            q = q.Where(x => x.Lines.All(z => z.Value == filter.Result || z.Value=="Не применяется"));
         }
      }

      if (filter.Offset.HasValue)
      {
         return q.ToList().OrderByDescending(x=>x.Stamp).Skip(filter.Offset.Value).Take(10).ToList();
      }
      else
      {
         return q.ToList().OrderByDescending(x=>x.Stamp).Take(10).ToList();
      }
     
      
   }

   public List<OTKTargetValue> Targets()
   {
      return bc.OTKTargetValues.ToList();
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

   private OTKCheck SaveCheck(OTKCheck otkCheck)
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
      foreach (var line in otkCheck.Lines)
      {
         var toSaveLine = new OTKCheckLine();
         toSaveLine.FullName = line.FullName;
         toSaveLine.ShortName = line.ShortName;
         toSaveLine.Description = line.Description;
         toSaveLine.Value = line.Value;
         toSaveLine.TargetValue = line.TargetValue;
         toSaveLine.MeasuredValue = line.MeasuredValue;
         
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
      

         // \[(.*?)\]
         
         

      otkCheck.Iteration = bc.OTKChecks.Count(x => x.OrderNumber ==  work.OrderNumber && x.OrderLineNumber == work.OrderLineNumber)+1;
      otkCheck.Lines = new List<OTKCheckLine>();

      var operations = bc.OTKAvailableOperations.Where(x=>x.ProductLine.Contains(work.ProductLineId)).ToList();
      
      Regex targetRegex = new Regex(@"\[(.*?)\]");
      List<string> macProps = new List<string>();
      List<string> macExtendedProps = new List<string>();
      List<string> crpProps = new List<string>();
      
      foreach (var op in operations)
      {
         OTKCheckLine otkCheckLine = new OTKCheckLine();
         otkCheckLine.ShortName = op.ShortName;
         otkCheckLine.FullName = op.FullName;
         otkCheckLine.Description = "";
         otkCheckLine.Value = "";
         otkCheckLine.TargetValue = op.TargetValue;
         otkCheckLine.MeasuredValue = "";

         var matches = targetRegex.Matches(otkCheckLine.TargetValue);
       
         foreach (Match m in matches)
         {
            if (m.Groups.Count == 2)
            {
               string s = m.Groups[1].Value;
               if (s.Contains("(m)"))
               {
                  macProps.Add(Regex.Replace(s, @"^\([^)]*\)", ""));
               }else if (s.Contains("(me)"))
               {
                  macExtendedProps.Add(Regex.Replace(s, @"^\([^)]*\)", ""));
               }else if (s.Contains("(crp)"))
               {
                  crpProps.Add(  Regex.Replace(s, @"^\([^)]*\)", ""));
               }
            }
         }
         otkCheck.Lines.Add(otkCheckLine);
      }

      MaconomyBase mb = new MaconomyBase();
      
      Dictionary<string, string> toReplace = new Dictionary<string, string>();
      
      if (macProps.Count > 0)
      {
         string macPropsQuery = string.Join(",", macProps);
         var t = mb.getTableFromDB($"SELECT {macPropsQuery} FROM ITEMINFORMATION WHERE ITEMNUMBER='{otkCheck.Article}'");
         if (t.Rows.Count > 0)
         {
            foreach (DataColumn column in t.Columns )
            {
                  toReplace.Add($"[(m){column.ColumnName.ToUpper()}]",MaconomyBase.makeStringRu(t.Rows[0][column].ToString()));
            }   
         }
         
      }

      if (macExtendedProps.Count > 0)
      {
         string macExtendedPropsQuery = string.Join(",", macExtendedProps.Select(x=>$"'{x}'"));
         var exT = mb.getTableFromDB($"SELECT PROPERTYNAME, REMARK1 from itempropertyline where ITEMNUMBER='{otkCheck.Article}' and propertyname in ({macExtendedPropsQuery})))");

         foreach (DataRow r in exT.Rows)
         {
            toReplace.Add($"[(me){r[0].ToString()}]",MaconomyBase.makeStringRu(r[1].ToString()));
         }
      }

      foreach (var line in otkCheck.Lines)
      {
         foreach (var r in toReplace)
         {
            if (line.TargetValue.Contains(r.Key))
            {
               line.TargetValue = line.TargetValue.Replace(r.Key, r.Value);
            }
         }
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
         exist.TargetValue = toSave.TargetValue;

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