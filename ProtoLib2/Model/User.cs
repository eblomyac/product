using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtoLib2.Model;

public class User
{
    [Key] [MaxLength(32)] public string AccName { get; set; }

    [MaxLength(128)] public string Name { get; set; }

    [MaxLength(64)] public string Mail { get; set; }

    public ICollection<Role> Roles { get; set; }

    [NotMapped]
    public bool IsPersonnel
    {
        get
        {
            if (Roles == null || Roles.Count == 0)
                return false;
            return Roles.FirstOrDefault(x => x.Type == RoleType.Personnel) != null;
        }
        set
        {
            if (value)
            {
                if (Roles == null) Roles = new List<Role>();

                var exist = Roles.FirstOrDefault(x => x.Type == RoleType.Personnel);
                if (exist == null)
                {
                    exist = new Role();
                    exist.Type = RoleType.Personnel;
                    exist.UserAccName = AccName;
                    Roles.Add(exist);
                }
            }
            else
            {
                if (Roles != null)
                {
                    var existRole = Roles.FirstOrDefault(x => x.Type == RoleType.Personnel);
                    if (existRole != null) Roles.Remove(existRole);
                }
            }
        }
    }

    [NotMapped]
    public bool IsAdmin
    {
        get
        {
            if (Roles == null || Roles.Count == 0)
                return false;
            return Roles.FirstOrDefault(x => x.Type == RoleType.Admin) != null;
        }
        set
        {
            if (value)
            {
                if (Roles == null) Roles = new List<Role>();

                var exist = Roles.FirstOrDefault(x => x.Type == RoleType.Admin);
                if (exist == null)
                {
                    exist = new Role();
                    exist.Type = RoleType.Admin;
                    exist.UserAccName = AccName;
                    Roles.Add(exist);
                }
            }
            else
            {
                if (Roles != null)
                {
                    var existRole = Roles.FirstOrDefault(x => x.Type == RoleType.Admin);
                    if (existRole != null) Roles.Remove(existRole);
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
                return false;
            return Roles.FirstOrDefault(x => x.Type == RoleType.Operator) != null;
        }
        set
        {
            if (value)
            {
                if (Roles == null) Roles = new List<Role>();

                var exist = Roles.FirstOrDefault(x => x.Type == RoleType.Operator);
                if (exist == null)
                {
                    exist = new Role();
                    exist.Type = RoleType.Operator;
                    exist.UserAccName = AccName;
                    Roles.Add(exist);
                }
            }
            else
            {
                if (Roles != null)
                {
                    var existRole = Roles.FirstOrDefault(x => x.Type == RoleType.Operator);
                    if (existRole != null) Roles.Remove(existRole);
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
                return false;
            return Roles.FirstOrDefault(x => x.Type == RoleType.PostMaster) != null;
        }
        set
        {
            if (value)
            {
                if (Roles == null) Roles = new List<Role>();

                var exist = Roles.FirstOrDefault(x => x.Type == RoleType.PostMaster);
                if (exist == null)
                {
                    exist = new Role();
                    exist.Type = RoleType.PostMaster;
                    exist.UserAccName = AccName;
                    exist.MasterPosts = new List<string>();
                    Roles.Add(exist);
                }
            }
            else
            {
                if (Roles != null)
                {
                    var existRole = Roles.FirstOrDefault(x => x.Type == RoleType.PostMaster);
                    if (existRole != null) Roles.Remove(existRole);
                }
            }
        }
    }

    [NotMapped]
    public List<string> PostIdMaster
    {
        get
        {
            if (Roles == null || Roles.Count == 0) return new List<string>();

            var masterRole = Roles.FirstOrDefault(x => x.Type == RoleType.PostMaster);
            if (masterRole == null)
                return new List<string>();
            return masterRole.MasterPosts;
        }
        set
        {
            if (value.Count > 0)
            {
                if (Roles == null) Roles = new List<Role>();

                var exist = Roles.FirstOrDefault(x => x.Type == RoleType.PostMaster);
                if (exist == null)
                {
                    exist = new Role();
                    exist.Type = RoleType.PostMaster;
                    exist.UserAccName = AccName;
                    exist.MasterPosts = value;
                    Roles.Add(exist);
                }

                exist.MasterPosts = value;
            }
            else
            {
                if (Roles != null)
                {
                    var existRole = Roles.FirstOrDefault(x => x.Type == RoleType.PostMaster);
                    if (existRole != null) Roles.Remove(existRole);
                }
            }
        }
    }
}