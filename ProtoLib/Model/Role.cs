using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtoLib.Model
{
    public enum RoleType
    {
        Admin,
        PostMaster,
        Operator,
        Personnel
    }
    public class Role:IEquatable<Role>
    {
        public long Id { get; set; }
        public RoleType Type { get; set; }
        [MaxLength(32)]
        public string UserAccName { get; set; }
      //  public string? PostId { get; set; }
        
        public string? MasterPostMap { get; set; }
        [NotMapped]
        public List<string> MasterPosts{
            get
            {
                if (string.IsNullOrEmpty(this.MasterPostMap))
                {
                    return new List<string>();
                }
                else
                {
                    return this.MasterPostMap.Split(';').ToList();
                }
            }
            set
            {
                if (value.Contains("Нет") || value.Contains(null) || value.Count==0)
                {
                    this.MasterPostMap = "";
                }
                else
                {
                    this.MasterPostMap = string.Join(';', value);
                }
                
            }
            
        } 

        public bool Equals(Role? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && UserAccName == other.UserAccName;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Role) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int) Type, UserAccName);
        }
    }
}