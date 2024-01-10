using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProductLib.UserRole
{
    public abstract class Role
    {
        public long Id { get; set; }
        public Person.Person Owner { get; set; }
    }
    public class AdminRole:Role
    {
        public AdminRole()
        {
            
        }
    }

    public class OperatorRole:Role
    {
        
        
    }

    public class PostMasterRole:Role
    {
        public string Post { get; private set; }
    }

    public class RoleCollection
    {
        public long Id { get; set; }
        public ICollection<Role> Roles { get; set; }
 
    }

    public static class RoleCollectionManager
    {
        internal static T? Role<T>(this RoleCollection roles) where T: Role
        {
            if (roles != null)
            {
                foreach (var r in roles.Roles)
                {
                    if (typeof(T) == r.GetType())
                    {
                        return r as T;
                    }
                }
            }

            return null;
        }

        internal static ICollection<T> Roles<T>(this RoleCollection roles) where T : Role
        {
            if (roles != null)
            {
                Collection<T> result = new Collection<T>();
                foreach (var r in roles.Roles)
                {
                    if (typeof(T) == r.GetType())
                    {
                        result.Add(r as T);
                    }
                }

                return result;
            }
            return new Collection<T>();
        }
        public static bool CanAdmin(this RoleCollection roles)
        {
            return roles.Role<AdminRole>() != null;
        }
        public static bool CanOperate(this RoleCollection roles)
        {
            return roles.Role<OperatorRole>() != null;
        }
        public static bool IsMasterOnPost(this RoleCollection roles,string post)
        {
            return roles.Roles<PostMasterRole>()?.FirstOrDefault(x => x.Post == post) != null;
        }
    }
}