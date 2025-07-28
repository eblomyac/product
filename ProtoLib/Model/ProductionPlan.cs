using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtoLib.Model;

public class ProductionPlan
{
    [Key]
    public long Id { get; set; }
    [MaxLength(64)]
    public string CrpCenter { get; set; }
    [MaxLength(128)]
    public string CrpCenterDescription { get; set; }
    
    public int Year { get; set; }
    public int Month { get; set; }
    
    public decimal TargetMinutes { get; set; }
    
    public decimal EffectiveRatio { get; set; }
    public decimal AdditionalRatio { get; set; }
    public decimal DirectorRatio { get; set; }
    
    public string PostId { get; set; }
    
    [NotMapped]
    public decimal RatioMinutes {
        get
        {
            return TargetMinutes * EffectiveRatio * AdditionalRatio * DirectorRatio;
        }}
}