using System.ComponentModel.DataAnnotations;

namespace ProtoLib.Model;

public class OTKAvailableOperation
{
    [Key]
    public long Id { get; set; }
    [MaxLength(256)]
    public string ShortName { get; set; }
    [MaxLength(1024)]
    public string FullName { get; set; }
    
    [MaxLength(256)]
    public string TargetValue { get; set; }
    
    [MaxLength(128)]
    public string ProductLine { get; set; }
    
}

public class OTKWorker
{
    [Key]
    public long Id { get; set; }
    [MaxLength(128)]
    public string Name { get; set; }
}