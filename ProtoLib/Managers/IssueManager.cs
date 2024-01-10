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
        public bool ResolveIssue(long issueId,string accName)
        {
            using (BaseContext c = new BaseContext(accName))
            {
                var exist = c.Issues.FirstOrDefault(x => x.Id == issueId);
                
                if (exist != null)
                {
                    var log = c.WorkIssueLogs.First(x => x.SourceIssueId == issueId);
                    log.End = DateTime.Now;
                    exist.Resolved = DateTime.Now;
                  return c.SaveChanges()>0;
                }

                return false;
            }
        }
        
        public WorkIssue RegisterIssue(long workId, long templateId, string description,string accName)
        {
            using (BaseContext c = new BaseContext(accName))
            {
                var exist = c.Issues.Include(x=>x.Template).FirstOrDefault(x => x.WorkId == workId && x.TemplateId == templateId);
                
                
                if (exist != null && exist.Resolved==null)
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
                    wi.Description =  $"{template.Name} {description}";
                    wi.TemplateId = templateId;
                    wi.WorkId = workId;

                    WorkIssueLog wil = new WorkIssueLog();
                   
                    wil.Start=DateTime.Now;
                    wil.Article = work.Article;
                    wil.OrderNumber = work.OrderNumber;
                    wil.Description = description;
                    wil.Type = template.Name;
                    wil.PostId = work.PostId;
                    wil.End = null;
                    
                    c.Issues.Add(wi);
                    c.SaveChanges();
                    wil.SourceIssueId = wi.Id;
                    c.WorkIssueLogs.Add(wil);
                    c.SaveChanges();
                    return wi;
                }
            }
        }
        
        public List<WorkIssueTemplate> List()
        {
            using (BaseContext c = new BaseContext(""))
            {
                return c.IssueTemplates.AsNoTracking().ToList();
            }
        }

        public List<WorkIssueTemplate> Update(List<WorkIssueTemplate> templates,string accName)
        {
            using (BaseContext c = new BaseContext(accName))
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

                return dbTemplates;
            }
        }

        public List<WorkIssue> WorkIssues(long workId, bool onlyActual = true)
        {
            using (BaseContext c = new BaseContext(""))
            {
                return c.Issues.AsNoTracking().Where(x => x.WorkId == workId && x.Resolved==null).ToList();
            }
        }
    }
}