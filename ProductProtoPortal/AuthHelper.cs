using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace ProductProtoPortal
{
     public static class AuthHelper
    {
        public struct ADUser
        {
            public string Name;
            public string LastName;
            public string Mail;
            public string SAM;
            public string FullName;
        }
        public static ICollection<string> GetGroups(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            
            Collection<string> result = new Collection<string>();
#if DEBUG
            result.Add("b2boperator");
#endif
            var groupList = httpContext.User.Claims.Where(x => x.Type == System.Security.Claims.ClaimTypes.GroupSid);
            foreach (var item in groupList)
            {
                var name = new System.Security.Principal.SecurityIdentifier(item.Value).Translate(typeof(System.Security.Principal.NTAccount)).ToString();
               result.Add(name);
            }

            return result;
        }
        public static bool IsUserInApiGroup(string group, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {


            var groupList = httpContext.User.Claims.Where(x => x.Type == System.Security.Claims.ClaimTypes.GroupSid);
            foreach (var item in groupList)
            {
                var name = new System.Security.Principal.SecurityIdentifier(item.Value).Translate(typeof(System.Security.Principal.NTAccount)).ToString();
                if (name.ToLower().Contains(group.ToLower()))
                {
                    return true;
                }
            }
            #if (DEBUG)
              return true;
            #endif
            return false;
        }

        public static ADUser GetADUser(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {   try
            {
#if DEBUG
                return new ADUser()
                {
                    LastName = "Олейник",
                    Mail = "Pavel.Olejnik@ksk.ru",
                    Name = "Павел",
                    FullName =  "Олейник Павел",
                    SAM = "polejnik"
                };
#endif
            var groupList = httpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Name);
            if (groupList != null)
            {
             
                    string name = groupList.Value.Split('\\')[1];
                    System.DirectoryServices.DirectorySearcher ds = new System.DirectoryServices.DirectorySearcher();
                    ds.Filter = "(SAMAccountName=" + name + ")";
                    System.DirectoryServices.SearchResult sr = ds.FindOne();
                    ADUser user = new ADUser();
                    user.Mail = sr.Properties["mail"][0].ToString();
                    user.Name = sr.Properties["givenName"][0].ToString();
                    user.LastName = sr.Properties["sn"][0].ToString();
                    user.FullName = user.FullName + " " + user.Name;
                    user.SAM = name;
                    return user;
                }
           
               
            }catch(Exception exc)
            {
                throw new Exception("auth error",exc);
            }
            return new ADUser();
        }
        public static string GetAccountName(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            var groupList = httpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Name);
            #if DEBUG
            return "polejnik";
            #endif
            if (groupList != null)
            {
                try
                {
                    string name = groupList.Value.Split('\\')[1];
                    System.DirectoryServices.DirectorySearcher ds = new System.DirectoryServices.DirectorySearcher();
                    ds.Filter = "(SAMAccountName=" + name + ")";
                    System.DirectoryServices.SearchResult sr = ds.FindOne();
                    //return sr.Properties["displayname"][0] + " (" + name + ")";
                    return name;
                }
                catch { }
            }
            return "";

        }

        public static long GetStock(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            StringValues h = "";
            if (httpContext.Request.Headers.TryGetValue("StockId", out h))
            {
                return long.Parse(h);
            }

            return 0;
        }
        public static string GetToken(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            StringValues h = "";
            if (httpContext.Request.Headers.TryGetValue("Token", out h))
            {
                return h;
            }

            return "";
        }
    }
    }
