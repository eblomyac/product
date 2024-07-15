using System.ComponentModel.DataAnnotations;

namespace ProtoLib2.Model;

public class TechCard
{
    [Key] public Guid Id { get; set; }

    [MaxLength(255)] public string Article { get; set; }

    [MaxLength(255)]
    public string ArticleLow
    {
        get
        {
            if (!string.IsNullOrEmpty(Identity))
                return Article.ToLower();
            return "";
        }
    }

    [MaxLength(32)] public string Identity { get; set; }

    [MaxLength(32)]
    public string IdentityLow
    {
        get
        {
            if (!string.IsNullOrEmpty(Identity)) return Identity.ToLower();

            return "";
        }
    }

    public Guid? ImageSetId { get; set; }
    public ImageSet? ImageSet { get; set; }
    public string Description { get; set; }

    public List<TechCardPost> PostParts { get; set; }
}