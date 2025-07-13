using System.ComponentModel.DataAnnotations;

namespace ProtoLib.Model;

public class OTKTargetValue
{
    [Key]
    public long Id { get; set; }
    [MaxLength(128)]
    public string DisplayName { get; set; }
    [MaxLength(64)]
    public string Source { get; set; }
    [MaxLength(128)]
    public string Target { get; set; }
    
}