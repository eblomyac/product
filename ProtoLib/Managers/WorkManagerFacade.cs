using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
   public class WorkManagerFacade
    {
        private WorkSaveManager _saveManager;
        private WorkCreateManager _createManager;
        private WorkStarter _workStarter;
        private WorkSplitter _splitter;
        private WorkStatusChanger _statusChanger;
        private string _accName="";

        public WorkManagerFacade(string accName)
        {
            _accName = accName;
            this._createManager = new WorkCreateManager();
            this._saveManager = new WorkSaveManager(accName);
            this._splitter = new WorkSplitter();
            this._workStarter = new WorkStarter(accName);
            _statusChanger = new WorkStatusChanger();
        }
        
        public List<Work> PrepareWorks(List<WorkCreateTemplate> workCreateTemplates)
        {
            return _createManager.CreateWorks(workCreateTemplates ,true);
        }

        public List<Work> CreateWorks(List<Work> works)
        {
            works = _saveManager.AggregateByPosts(works);
            return    this._saveManager.SaveWorks(works);
        }

        public List<Work> SplitWork(Work source, int splitCount)
        {
            var works =_splitter.SplitWork(source, splitCount,_accName);
            var result= this._saveManager.SaveWorks(works);

            using (BaseContext c = new BaseContext(_accName))
            {
                var userPost = c.Roles.AsNoTracking().FirstOrDefault(x => x.UserAccName == this._accName && x.Type == RoleType.PostMaster);
                if (userPost != null )
                {
                    var post = userPost.MasterPosts.FirstOrDefault(x => x == source.PostId);
                    return result.Where(x => x.PostId == post).ToList();
                }
            }

            return result;

            // var newWorks = this._createManager.SplitWork(source, splitCount);
            //   var savedSource =this._saveManager.SaveWorksChanges(new List<Work>(){source});
            // var toSaveNew = this._createManager.CreateWorks(new List<Work>() {newWork});
            //var savedNew=this._saveManager.SaveNewWorksIgnoreCheck(toSaveNew);
            //   List<Work> result = new List<Work>();
            //   result.AddRange(savedSource);
            //    result.AddRange(savedNew);
            //  return result;
            //return new List<Work>();
        }

        public List<Work> SplitWork(long id, int splitCount)
        {
            Work w;
            using (BaseContext c = new BaseContext(_accName))
            {
                w = c.Works.AsNoTracking().FirstOrDefault(x=>x.Id == id);
            }

            if (w == null)
            {
                return new List<Work>();
            }
            return SplitWork(w, splitCount);

        }

        public Work View(long id)
        {
            using (BaseContext c = new BaseContext(_accName))
            {
                return c.Works.AsNoTracking().Include(x=>x.Issues).FirstOrDefault(x=>x.Id == id);
            }
        }

        public bool MoveToPostRequest(long workId, string toPostId, List<string> alsoStartOnPosts, string returnComment )
        {
            using (BaseContext c = new BaseContext(_accName))
            {
                
                var work = c.Works.AsNoTracking().FirstOrDefault(x => x.Id == workId);
                
                if(work==null){return false;}
                
                bool isForward = (int)work.Status > 30;

                if (isForward)
                {
                    
                    string currentPost = "";
                    if (!string.IsNullOrEmpty(work.MovedTo))
                    {
                        currentPost = work.MovedTo;
                    }    
                    work.MovedTo = toPostId;
                    _saveManager.SaveWorks(new List<Work>() {work});
                    var suggest = new WorkAnalytic.WorkStartSuggestion()
                    {
                        Article = work.Article,
                        OrderNumber = work.OrderNumber,
                        AvailablePosts = new List<string>(),
                        SelectedPosts = new List<string>() {toPostId}
                    };
                    return _workStarter.MoveWorkMaster(workId, toPostId,_accName,alsoStartOnPosts,currentPost);

                }
                else
                {
                    var prevWork = c.Works.FirstOrDefault(x =>
                        x.Article == work.Article && 
                        x.OrderNumber == work.OrderNumber && 
                        x.MovedTo == work.PostId && 
                        x.Count == work.Count && 
                        x.Status == WorkStatus.sended);
                    if (prevWork == null)
                    {
                        throw new Exception("Prevo work not found");
                    }

                    _statusChanger.ChangeStatus(prevWork, WorkStatus.waiting, _accName);
                    prevWork.MovedTo = "";
                    work.Status = WorkStatus.hidden;
                    IssueManager im = new IssueManager();
                    im.RegisterIssue(prevWork.Id, 0, returnComment, _accName, "", work.PostId);
                    _saveManager.SaveWorks(new List<Work>() {work, prevWork});
                }

                //todo backward
                
                return false;




            }
        }
        
    }
}