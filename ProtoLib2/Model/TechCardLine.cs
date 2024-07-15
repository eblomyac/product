using System.ComponentModel.DataAnnotations;

namespace ProtoLib2.Model;

public class TechCardLine
{
    [Key] public Guid Id { get; set; }

    public Guid TechCardPostId { get; set; }
    //  public TechCardPost TechCardPost { get; set; }

    public bool IsCustom { get; set; }

    public int ProcessionOrder { get; set; }

    public string Operation { get; set; }
    public string Device { get; set; }
    public string Equipment { get; set; }
    public string EquipmentInfo { get; set; }

    public Guid? ImageSetId { get; set; }
    public decimal Cost { get; set; }
}