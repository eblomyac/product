using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace ProtoLib.Model
{
    public class Post
    {
        [Key]
        [MaxLength(32)]
        public string Name { get; set; }
        public string TableName { get; set; }
        
        public bool Disabled { get; set; }
        public int ProductOrder { get; set; }
        public bool IsShared { get; set; }
        public bool CanEnd { get; set; }
        
        public virtual ICollection<PostCreationKey> PostCreationKeys { get; set; }

        [NotMapped]
        public string Keys
        {
            get
            {
                if (this.PostCreationKeys == null || this.PostCreationKeys.Count == 0)
                {
                    return "";
                }
                else
                {
                    return string.Join(", ", this.PostCreationKeys.Select(x => x.Key));
                }
            }
            set
            {
                if (this.PostCreationKeys == null)
                {
                    this.PostCreationKeys = new List<PostCreationKey>();
                }

                string[] splitted = value.Split(new[] {',', ' ',';'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in splitted.Distinct())
                {
                    var exist = this.PostCreationKeys.FirstOrDefault(x => x.Key == s);
                    if (exist == null)
                    {
                        exist = new PostCreationKey();
                        exist.Key = s;
                        exist.PostId = this.Name;
                        this.PostCreationKeys.Add(exist);
                    }
                }

                List<string> keys = this.PostCreationKeys.Select(x => x.Key).ToList();
                foreach (var key in keys)
                {
                    if (!value.Contains(key))
                    {
                        this.PostCreationKeys.Remove(this.PostCreationKeys.First(x => x.Key == key));
                    }
                }
            }
        }
    }
}