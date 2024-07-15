using System.ComponentModel.DataAnnotations;

namespace ProtoLib2.Model;

public class AdditionalCostTemplate
{
    public long Id { get; set; }

    [MaxLength(128)] public string Name { get; set; }

    public bool Disabled { get; set; }
}