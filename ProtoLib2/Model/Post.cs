using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtoLib2.Model;

public class Post
{
    [Key] [MaxLength(32)] public string Name { get; set; }

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
            if (PostCreationKeys == null || PostCreationKeys.Count == 0)
                return "";
            return string.Join(", ", PostCreationKeys.Select(x => x.Key));
        }
        set
        {
            if (PostCreationKeys == null) PostCreationKeys = new List<PostCreationKey>();

            var splitted = value.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in splitted.Distinct())
            {
                var exist = PostCreationKeys.FirstOrDefault(x => x.Key == s);
                if (exist == null)
                {
                    exist = new PostCreationKey();
                    exist.Key = s;
                    exist.PostId = Name;
                    PostCreationKeys.Add(exist);
                }
            }

            var keys = PostCreationKeys.Select(x => x.Key).ToList();
            foreach (var key in keys)
                if (!value.Contains(key))
                    PostCreationKeys.Remove(PostCreationKeys.First(x => x.Key == key));
        }
    }
}