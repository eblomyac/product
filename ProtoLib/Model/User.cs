using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ProtoLib.Model
{
    public class User
    {
        [Key]
        [MaxLength(32)]
        public string AccName { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(64)]
        public string Mail { get; set; }
        
        public ICollection<Role> Roles { get; set; }
        
        [NotMapped]
        public bool IsAdmin {
            get
            {
                if (Roles == null || Roles.Count == 0)
                {
                    return false;
                }
                else
                {
                    return Roles.FirstOrDefault(x => x.Type == RoleType.Admin) != null;
                }
            }
            set
            {
                if (value)
                {
                    if (this.Roles == null)
                    {
                        this.Roles = new List<Role>();
                    }

                    var exist = this.Roles.FirstOrDefault(x => x.Type == RoleType.Admin);
                    if (exist == null)
                    {
                        exist = new Role();
                        exist.Type = RoleType.Admin;
                        exist.UserAccName = this.AccName;
                        this.Roles.Add(exist);
                    }
                }
                else
                {
                    if (Roles != null)
                    {
                        var existRole = this.Roles.FirstOrDefault(x => x.Type == RoleType.Admin);
                        if (existRole != null)
                        {
                            this.Roles.Remove(existRole);                            
                        }

                    }
                    
                }
            }
        }

        [NotMapped]
        public bool IsOperator
        {
            get
            {
                if (Roles == null || Roles.Count == 0)
                {
                    return false;
                }
                else
                {
                    return Roles.FirstOrDefault(x => x.Type == RoleType.Operator) != null;
                }
            }
            set
            {
                if (value)
                {
                    if (this.Roles == null)
                    {
                        this.Roles = new List<Role>();
                    }

                    var exist = this.Roles.FirstOrDefault(x => x.Type == RoleType.Operator);
                    if (exist == null)
                    {
                        exist = new Role();
                        exist.Type = RoleType.Operator;
                        exist.UserAccName = this.AccName;
                        this.Roles.Add(exist);
                    }
                }else
                {
                    if (Roles != null)
                    {
                        var existRole = this.Roles.FirstOrDefault(x => x.Type == RoleType.Operator);
                        if (existRole != null)
                        {
                            this.Roles.Remove(existRole);                            
                        }

                    }
                    
                }
            }
        }

        [NotMapped]
        public bool IsMaster
        {
            get
            {
                if (Roles == null || Roles.Count == 0)
                {
                    return false;
                }
                else
                {
                    return Roles.FirstOrDefault(x => x.Type == RoleType.PostMaster) != null;
                }
            }
            set
            {
                if (value)
                {
                    if (this.Roles == null)
                    {
                        this.Roles = new List<Role>();
                    }

                    var exist = this.Roles.FirstOrDefault(x => x.Type == RoleType.PostMaster);
                    if (exist == null)
                    {
                        exist = new Role();
                        exist.Type = RoleType.PostMaster;
                        exist.UserAccName = this.AccName;
                        exist.MasterPosts = new List<string>();
                        this.Roles.Add(exist);
                    }
                }else
                {
                    if (Roles != null)
                    {
                        var existRole = this.Roles.FirstOrDefault(x => x.Type == RoleType.PostMaster);
                        if (existRole != null)
                        {
                            this.Roles.Remove(existRole);                            
                        }

                    }
                    
                }
            }
        }

        [NotMapped]
        public List<string> PostIdMaster
        {
            get
            {
                if (Roles == null || Roles.Count == 0)
                {
                    return new List<string>();
                }
                else
                {
                    var masterRole = Roles.FirstOrDefault(x => x.Type == RoleType.PostMaster);
                    if (masterRole == null)
                    {
                        return new List<string>();
                    }
                    else
                    {
                        return masterRole.MasterPosts;
                    }
                }
            }
            set
            {
                if (value.Count > 0)
                {
                    if (this.Roles == null)
                    {
                        this.Roles = new List<Role>();
                    }

                    var exist = this.Roles.FirstOrDefault(x => x.Type == RoleType.PostMaster);
                    if (exist == null)
                    {
                        exist = new Role();
                        exist.Type = RoleType.PostMaster;
                        exist.UserAccName = this.AccName;
                        exist.MasterPosts = value;
                        this.Roles.Add(exist);
                    }

                    exist.MasterPosts = value;
                }
                else
                {
                    if (Roles != null)
                    {
                        var existRole = this.Roles.FirstOrDefault(x => x.Type == RoleType.PostMaster);
                        if (existRole != null)
                        {
                            this.Roles.Remove(existRole);                            
                        }

                    }
                    
                }
            }
        }
        

    }
}