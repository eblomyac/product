using System.ComponentModel.DataAnnotations;

namespace ProtoLib2.Model;

public class AdditionalCost
{
    public long Id { get; set; }

    [MaxLength(128)] public string Description { get; set; }

    [MaxLength(128)] public string Comment { get; set; }

    public decimal Cost { get; set; }

    public long WorkId { get; set; }
    public Work Work { get; set; }


    public long AdditionalCostTemplateId { get; set; }
    public AdditionalCostTemplate AdditionalCostTemplate { get; set; }
}