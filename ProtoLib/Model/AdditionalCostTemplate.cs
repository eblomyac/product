using System.ComponentModel.DataAnnotations;

namespace ProtoLib.Model;

public class AdditionalCostTemplate
{
    public long Id { get; set; }
    [MaxLength(128)]
    public string Name { get; set; }
    
    public bool CanItem { get; set; }
    public bool CanPost { get; set; }
}