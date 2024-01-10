using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProtoProductLib.BaseStruct;

namespace ProductLibPrototype.Managers
{
    public class PostCreator
    {
        public Post CreatePost(string name, params string[] keys)
        {
            using (ProductContext c = new ProductContext())
            {
                Post existPost = null;//c.Posts.Include(x=>x.PostCreationKeys).FirstOrDefault(x => x.Name == name);
                if (existPost == null)
                {
                    existPost.Name = name;
                    existPost.PostCreationKeys = new List<PostCreationKey>();

                    foreach (string key in keys)
                    {
                        PostCreationKey pck = new PostCreationKey();
                        pck.Key = key;
                        existPost.PostCreationKeys.Add(pck);
                    }

              //      c.Posts.Add(existPost);
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
    }
}