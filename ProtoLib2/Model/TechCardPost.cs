using System.ComponentModel.DataAnnotations;

namespace ProtoLib2.Model;

public class TechCardPost
{
    [Key] public Guid Id { get; set; }

    public Guid TechCardId { get; set; }
    // public TechCard TechCard { get; set; }


    [MaxLength(32)] public string PostId { get; set; }

    public Post Post { get; set; }

    public Guid? ImageSetId { get; set; }
    public ImageSet? ImageSet { get; set; }

    public List<TechCardLine> Lines { get; set; }
}