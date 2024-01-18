using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class PostManager
    {
        public Post CreatePost(string name,string accName, params string[] keys)
        {
            using (BaseContext c = new BaseContext(accName))
            {
                Post existPost =c.Posts.Include(x=>x.PostCreationKeys).FirstOrDefault(x => x.Name == name);
                if (existPost == null)
                {
                    existPost = new Post();
                    existPost.Name = name;
                    existPost.PostCreationKeys = new List<PostCreationKey>();

                    foreach (string key in keys)
                    {
                        PostCreationKey pck = new PostCreationKey();
                        pck.Key = key;
                        existPost.PostCreationKeys.Add(pck);
                    }

                    c.Posts.Add(existPost);
                    c.SaveChanges();

                }
                else
                {
                    foreach (string key in keys)
                    {
                        var existKey = existPost.PostCreationKeys.FirstOrDefault(x => x.Key == key);
                        if (existKey == null)
                        {
                            PostCreationKey pck = new PostCreationKey();
                            pck.Key = key;
                            existPost.PostCreationKeys.Add(pck);
                        }
                    }

                    c.SaveChanges();
                }

                return existPost;
            }
        }

        public List<Post> List()
        {
            using ( BaseContext c = new BaseContext(""))
            {
                return c.Posts
                    .Include(x => x.PostCreationKeys)
                    .ToList();
            }
        }

        public void Update(List<Post> posts, string accName)
        {
            using (BaseContext c = new BaseContext(accName))
            {
                var user = c.Users.Include(x=>x.Roles).FirstOrDefault(x => x.AccName == accName);
                if (user.IsAdmin)
                {
                    foreach (var post in posts)
                    {
                        var dbPost = c.Posts.Include(x=>x.PostCreationKeys).FirstOrDefault(x => x.Name == post.Name);
                        if (dbPost == null)
                        {
                            dbPost = new Post();
                            dbPost.Name = post.Name;
                            c.Posts.Add(dbPost);
                        }

                        dbPost.IsShared = post.IsShared;
                        dbPost.ProductOrder = post.ProductOrder;
                        dbPost.Disabled = post.Disabled;
                        dbPost.Keys = post.Keys;
                    }

                    c.SaveChanges();
                }
            }
        }

        public void PostStatus(string accName)
        {
            using (BaseContext c = new BaseContext(accName))
            {
                
            }
        }

        public void PostActualStatistic(string accName, string postId)
        {
            using (BaseContext c = new BaseContext(accName))
            {
                List<WorkStatus> selectStatus = new List<WorkStatus>();
                selectStatus.Add(WorkStatus.income);
                selectStatus.Add(WorkStatus.running);
                selectStatus.Add(WorkStatus.waiting);
                selectStatus.Add(WorkStatus.sended);
                var works = c.Works.Where(x => x.PostId == postId && selectStatus.Contains(x.Status)).ToList();

                dynamic result = new ExpandoObject();

                var status = works.Select(x => x.Status).Distinct();
                
                
            }
        }
    }
}