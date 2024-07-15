using System.ComponentModel.DataAnnotations;

namespace ProtoLib2.Model;

public class PostCreationKey
{
    public long Id { get; set; }

    [MaxLength(32)] public string PostId { get; set; }

    public string Key { get; set; }
}