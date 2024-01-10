using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductLibPrototype.DataBase
{
    public class Post
    {
        [Key]
        [MaxLength(32)]
        public string Name { get; set; }
        
        public virtual ICollection<PostCreationKey> PostCreationKeys { get; set; }
    }
}