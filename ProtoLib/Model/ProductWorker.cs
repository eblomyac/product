using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProtoLib.Model;

public class ProductWorker
{
    [Key]
    public long Id { get; set; }
    [MaxLength(128)]
    public string Name { get; set; }
    
    [MaxLength(128)]
    public string Title { get; set; }
    public bool IsActive { get; set; }

  
    public ICollection<ProductTarget> Targets { get; set; }
}

public class ProductTarget
{
    public long Id { get; set; }
    public long ProductWorkerId { get; set; }
    [MaxLength(64)]
    public string PostId { get; set; }
    [NotMapped]
    public string TargetName {
        get
        {
            return $"{TargetCrpCenter} {TargetCrpPost} {TargetCrpPostDescription}".Trim();
        }}
    
    [MaxLength(64)]
    public string TargetCrpCenter { get; set; }
    [MaxLength(64)]
    public string TargetCrpPost { get; set; }
    
    [MaxLength(128)]
    public string TargetCrpCenterDescription { get; set; }
    [MaxLength(128)]
    public string TargetCrpPostDescription { get; set; }
    
}

public class ProductCalendarRecord
{
    
    public long Id { get; set; }
    
    [MaxLength(64)]
    public string PostId { get; set; }
    [MaxLength(128)]
    public string ProductWorkerName { get; set; }

    [NotMapped]
    public string TargetName {
        get
        {
            return $"{TargetCrpCenter} {TargetCrpPost} {TargetCrpPostDescription}".Trim();
        }}
    
    [MaxLength(64)]
    public string TargetCrpCenter { get; set; }
    [MaxLength(64)]
    public string TargetCrpPost { get; set; }
    
    [MaxLength(128)]
    public string TargetCrpCenterDescription { get; set; }
    [MaxLength(128)]
    public string TargetCrpPostDescription { get; set; }
    
    
    public int Day {get; set;}
    public int Month { get; set; }
    public int Year { get; set; }
    
    public decimal PlanningHours { get; set; }
    [MaxLength(64)]
    public string Description { get; set; }
    
    public decimal EffectiveHours { get; set; }
    public decimal PlanToWorkConst { get; set; }
}