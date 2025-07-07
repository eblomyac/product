using System.ComponentModel.DataAnnotations;

namespace ProtoLib.Model;

public class OTKCheck
{
    
    public long Id { get; set; }
    public DateTime Stamp { get; set; }
    [MaxLength(64)]
    public string ProductLine { get; set; }
    [MaxLength(128)]
    public string Article { get; set; }
    public long OrderNumber { get; set; }
    public int OrderLineNumber { get; set; }
    public int Iteration { get; set; }
    public decimal ProductCount { get; set; }
    public decimal CheckedCount { get; set; }
    
    [MaxLength(128)]
    public string Worker { get; set; }
    
    public long WorkId { get; set; }
    
    public virtual ICollection<OTKCheckLine> Lines { get; set; }
    
}

public class OTKCheckLine
{
    public long Id { get; set; }
    public long OTKCheckId { get; set; }
    public OTKCheck OTKCheck { get; set; }
    [MaxLength(256)]
    public string ShortName { get; set; }
    [MaxLength(1024)]
    public string FullName { get; set; }
    [MaxLength(64)]
    public string Value { get; set; }
    [MaxLength(256)]
    public string? Description { get; set; }
}