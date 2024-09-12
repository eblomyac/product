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
        private BaseContext? _c = null;

        public WorkManagerFacade(string accName)
        {
            _accName = accName;
            this._createManager = new WorkCreateManager();
            this._saveManager = new WorkSaveManager(accName);
            this._splitter = new WorkSplitter();
            this._workStarter = new WorkStarter(accName);
            _statusChanger = new WorkStatusChanger();
        }

        public bool CanBeEnd(long workId, string accName)
        {
            using (BaseContext c = new BaseContext(accName))
            {
                var u = c.Users.AsNoTracking().Include(x => x.Roles).FirstOrDefault(x => x.AccName == accName);
                if (!u.IsMaster)
                {
                    return false;
                }
                
                var work = c.Works.AsNoTracking().Include(x=>x.Post).FirstOrDefault(x => x.Id == workId);
                if (work.OrderNumber==100) return true;
                if (work.Post.CanEnd == false)
                {
                    return false;
                }

                return true;
            }
        }
        public WorkManagerFacade(BaseContext? c)
        {
            _c = c;
            if (c != null)
            {
                _accName = c.accName;
            }
            else
            {
                _accName = "";
            }
            
            this._createManager = new WorkCreateManager(_c);
            this._saveManager = new WorkSaveManager(_c);
            this._splitter = new WorkSplitter(_c);
            this._workStarter = new WorkStarter(_accName);
            _statusChanger = new WorkStatusChanger();
        }
        
        public List<Work> PrepareWorks(List<WorkCreateTemplate> workCreateTemplates, bool removeExist)
        {
            return _createManager.CreateWorks(workCreateTemplates ,removeExist);
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

            BaseContext c = _c ?? new BaseContext(_accName);
            {
                var userPost = c.Roles.AsNoTracking().FirstOrDefault(x => x.UserAccName == this._accName && x.Type == RoleType.PostMaster);
                if (userPost != null )
                {
                    var post = userPost.MasterPosts.FirstOrDefault(x => x == source.PostId);
                    return result.Where(x => x.PostId == post).ToList();
                }
            }
            if (_c == null)
            {
                c.Dispose();
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
            BaseContext c = _c ?? new BaseContext(_accName);
            {
                w = c.Works.AsNoTracking().FirstOrDefault(x=>x.Id == id);
            }
            if (_c == null)
            {
                c.Dispose();
            }

            if (w == null)
            {
                return new List<Work>();
            }
            return SplitWork(w, splitCount);

        }

        public Work View(long id)
        {
            Work result = null;
            BaseContext c = _c ?? new BaseContext(_accName);
            {
                result= c.Works.AsNoTracking().Include(x=>x.AdditionalCosts).Include(x=>x.Issues).FirstOrDefault(x=>x.Id == id);
            }
            if (_c == null)
            {
                c.Dispose();
            }

            return result;
        }

        public bool MoveToPostRequest(long workId, string toPostId, List<string> alsoStartOnPosts, string returnComment )
        {
            
            BaseContext c = _c ?? new BaseContext(_accName);
            try
            {

                var work = c.Works.AsNoTracking().FirstOrDefault(x => x.Id == workId);

                if (work == null)
                {
                    return false;
                }

                bool isForward = (int)work.Status > 30;

                if (isForward)
                {

                    string currentPost = "";
                    if (!string.IsNullOrEmpty(work.MovedTo))
                    {
                        currentPost = work.MovedTo;
                    }

                    work.MovedTo = toPostId;
                    _saveManager.SaveWorks(new List<Work>() { work });
                    var suggest = new WorkAnalytic.WorkStartSuggestion()
                    {
                        Article = work.Article,
                        OrderNumber = work.OrderNumber,
                        AvailablePosts = new List<string>(),
                        SelectedPosts = new List<string>() { toPostId }
                    };
                    return _workStarter.MoveWorkMaster(workId, toPostId, _accName, alsoStartOnPosts, currentPost);

                }
                else
                {
                    var prevWork = c.Works.FirstOrDefault(x =>
                        x.Article == work.Article && x.OrderLineNumber == work.OrderLineNumber &&
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
                    work.MovedFrom = "";
                    IssueManager im = new IssueManager();
                    im.RegisterIssue(prevWork.Id, 0, returnComment, _accName, "", work.PostId);
                    _saveManager.SaveWorks(new List<Work>() { work, prevWork });
                }

                //todo backward

                return false;




            }
            catch (Exception exc)
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
        
    }
}