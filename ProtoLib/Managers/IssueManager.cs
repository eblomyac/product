using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class IssueManager
    {
        private BaseContext? _c;
        public IssueManager(BaseContext? c = null)
        {
            _c = c;
        }
        public bool ResolveIssue(long issueId,string accName)
        {


            BaseContext c = _c ?? new BaseContext(accName);
            try
            {
                var exist = c.Issues.FirstOrDefault(x => x.Id == issueId);

                if (exist != null)
                {
                    var log = c.WorkIssueLogs.First(x => x.SourceIssueId == issueId);
                    log.End = DateTime.Now;
                    exist.Resolved = DateTime.Now;
                    return c.SaveChanges() > 0;
                }

                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (_c == null)
                {
                    c.Dispose();
                }
            }
        }
        
        public WorkIssue? RegisterIssue(long workId, long templateId, string description,string accName, string returnPostId, string returnedFromPostId="")
        {
            BaseContext c = _c ?? new BaseContext(accName);
            try
            {
                if (templateId == 0)
                {
                    var unvisible = c.IssueTemplates.FirstOrDefault(x => x.IsVisible == false);
                    if (unvisible == null)
                    {
                        WorkIssueTemplate tempalte = new WorkIssueTemplate();
                        tempalte.IsVisible = false;
                        tempalte.Name = "Возврат";
                        c.IssueTemplates.Add(tempalte);
                        c.SaveChanges();
                        templateId = tempalte.Id;
                    }
                    else
                    {
                        templateId = unvisible.Id;
                    }
                }

                var exist = c.Issues.Include(x => x.Template)
                    .FirstOrDefault(x => x.WorkId == workId && x.TemplateId == templateId);


                if (exist != null && exist.Resolved == null)
                {

                    exist.Description = $"{exist.Template.Name} {description}";
                    var log = c.WorkIssueLogs.First(x => x.SourceIssueId == exist.Id);
                    log.Description = description;
                    // exist.Created =DateTime.Now;

                    c.SaveChanges();
                    return exist;
                }
                else
                {
                    var template = c.IssueTemplates.FirstOrDefault(x => x.Id == templateId);
                    var work = c.Works.FirstOrDefault(x => x.Id == workId);
                    WorkIssue wi = new WorkIssue();
                    wi.Created = DateTime.Now;
                    wi.Description = $"{template.Name} {description}";
                    wi.TemplateId = templateId;
                    wi.WorkId = workId;
                    if (string.IsNullOrEmpty(returnedFromPostId))
                    {
                        wi.ReturnedFromPostId = "";
                    }
                    else
                    {
                        wi.ReturnedFromPostId = returnedFromPostId;
                    }

                    wi.ReturnBackPostId = returnPostId;
                    if (wi.ReturnBackPostId == null)
                    {
                        wi.ReturnBackPostId = "";
                    }

                    WorkIssueLog wil = new WorkIssueLog();
                    wil.ReturnedToPost = wi.ReturnBackPostId;
                    wil.Start = DateTime.Now;
                    wil.Article = work.Article;
                    wil.OrderNumber = work.OrderNumber;
                    wil.Description = description;
                    wil.Type = template.Name;
                    wil.PostId = work.PostId;
                    wil.End = null;
                    
                    WorkStatusChanger wss = new WorkStatusChanger();
                    wss.ChangeStatus(workId, WorkStatus.waiting, accName);

                    if (!string.IsNullOrWhiteSpace(wi.ReturnBackPostId))
                    {
                        /*
                        var previousWork = c.Works.AsNoTracking().FirstOrDefault(x =>
                            x.PostId == wi.ReturnBackPostId && x.Article == work.Article &&
                            x.OrderLineNumber == work.OrderLineNumber &&
                            x.OrderNumber == work.OrderNumber &&
                            (x.Status == WorkStatus.ended || x.Status == WorkStatus.sended));
                        if (previousWork != null)
                        {
                        
                            WorkStatusChanger wss = new WorkStatusChanger();
                            wss.ChangeStatus(previousWork, WorkStatus.waiting, accName);
                            WorkSaveManager workSaveManager = new WorkSaveManager(accName);
                            previousWork.MovedTo = "";
                            workSaveManager.SaveWorks(new List<Work> { previousWork });

                            RegisterIssue(previousWork.Id, templateId, description, accName, "", work.PostId);
                        }*/
                        var previousWorks = c.Works.AsNoTracking().Where(x =>
                            x.PostId == wi.ReturnBackPostId && x.Article == work.Article &&
                            x.OrderLineNumber == work.OrderLineNumber &&
                            x.OrderNumber == work.OrderNumber &&
                            (x.Status == WorkStatus.ended || x.Status == WorkStatus.sended)).ToList();
                        if (previousWorks.Count == 0)
                        {
                            //создать работу?
                        }
                        else if(previousWorks.Count==1)
                        {
                            var prevWork = previousWorks[0];
                            if (prevWork.Count != work.Count)
                            {
                                WorkManagerFacade wmf = new WorkManagerFacade(accName);
                                var splittedWorks = wmf.SplitWork(prevWork.Id, work.Count);
                                if (splittedWorks.Count > 1)
                                {
                                    prevWork = splittedWorks[1];
                                }
                            }
                       //     WorkStatusChanger wss = new WorkStatusChanger();
                            wss.ChangeStatus(prevWork, WorkStatus.waiting, accName);
                            WorkSaveManager workSaveManager = new WorkSaveManager(accName);
                            prevWork.MovedTo = "";
                            workSaveManager.SaveWorks(new List<Work> { prevWork });

                            RegisterIssue(prevWork.Id, templateId, description, accName, "", work.PostId);
                            
                        }
                    }

                    c.Issues.Add(wi);
                    c.SaveChanges();
                    wil.SourceIssueId = wi.Id;
                    c.WorkIssueLogs.Add(wil);
                    c.SaveChanges();
                    return wi;
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                if (_c == null)
                {
                    c.Dispose();
                }
            }
        }
        
        public List<WorkIssueTemplate> List()
        {
            List<WorkIssueTemplate> result;
            BaseContext c = _c ?? new BaseContext("");
            {
                 result = c.IssueTemplates.Where(x=>x.IsVisible).AsNoTracking().ToList();
            }
            if (_c == null)
            {
                c.Dispose();
            }
            return result;
        }

        public List<WorkIssueTemplate> Update(List<WorkIssueTemplate> templates,string accName)
        {
            List<WorkIssueTemplate> result;
            BaseContext c = new BaseContext(accName);
            {
                foreach (var template in templates)
                {
                    var exist = c.IssueTemplates.FirstOrDefault(x => x.Name == template.Name);
                    if (exist == null)
                    {
                        exist = new WorkIssueTemplate();
                        exist.Name = template.Name;
                        c.IssueTemplates.Add(exist);
                    }
                    
                }

                c.SaveChanges();
                var dbTemplates = c.IssueTemplates.ToList();
                var toDeleteTemplates = new List<WorkIssueTemplate>();
                foreach (var template in dbTemplates)
                {
                    var inList = templates.FirstOrDefault(x => x.Name == template.Name);
                    if (inList == null)
                    {
                        toDeleteTemplates.Add(template);
                    }
                }
                if(toDeleteTemplates.Count>0){c.IssueTemplates.RemoveRange(toDeleteTemplates);
                    c.SaveChanges();
                }

                result =  dbTemplates;
            }
            if (_c == null)
            {
                c.Dispose();
            }
            return result;
        }

        public List<WorkIssue> WorkIssues(long workId, bool onlyActual = true)
        {
            List<WorkIssue> result;
            BaseContext c = _c ?? new BaseContext("");
            {
                result =  c.Issues.AsNoTracking().Where(x => x.WorkId == workId && x.Resolved==null).ToList();
            }
            if (_c == null)
            {
                c.Dispose();
            }
            return result;
        }
    }
}