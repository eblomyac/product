using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class UserManager
    {
        private string _accName;
        private User _user;
        
        public UserManager(string accName)
        {
            this._accName = accName;
        }
        public User? Login(string accName, string mail,string name)
        {
            using (BaseContext c = new BaseContext())
            {
               return c.Users.AsNoTracking().Include(x=>x.Roles).FirstOrDefault(x => x.AccName == accName);
               
            }
        }

        public User Add(string accName, string mail, string name)
        {
            using (BaseContext c = new BaseContext(_accName))
            {
                User u = new User();
                u.Mail = mail;
                u.Name = name;
                u.AccName = accName;
                c.Users.Add(u);
                c.SaveChanges();
                return u;
            }
        }

        public void Update(List<User> users)
        {
            using (BaseContext c = new BaseContext(_accName))
            {
                if (this._user == null)
                {
                    this._user = c.Users.AsNoTracking().Include(x => x.Roles)
                        .FirstOrDefault(x => x.AccName == _accName);
                }

                if (this._user.IsAdmin)
                {
                    foreach (var user in users)
                    {
                        var dbUser = c.Users.Include(x=>x.Roles).FirstOrDefault(x => x.AccName == user.AccName);
                        dbUser.IsAdmin = user.IsAdmin;
                        dbUser.IsOperator = user.IsOperator;
                        dbUser.IsMaster = user.IsMaster;
                        dbUser.PostIdMaster = user.PostIdMaster;
                        
                    }
                    c.SaveChanges();
                }
                else
                {
                    
                }
            }
        }
        public List<User> List()
        {
            using (BaseContext c = new BaseContext(_accName))
            {
                if (this._user == null)
                {
                    this._user = c.Users.AsNoTracking().Include(x => x.Roles).FirstOrDefault(x => x.AccName == _accName);
                }

                if (this._user.IsAdmin)
                {
                    return c.Users.Include(x => x.Roles).ToList();    
                }
                else
                {
                    return new List<User>();
                }
                
            }
        }
    }
}