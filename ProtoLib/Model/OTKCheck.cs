using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

    [NotMapped]
    public string Name
    {
        get
        {
            return $"{OrderNumber}-{OrderLineNumber}-{Iteration}";
        }
    }

    [NotMapped]
    public string Result
    {
        get
        {
            if (this.Lines != null && this.Lines.Count > 0)
            {
                if (this.Lines.All(x => x.Value == "Брака нет" || x.Value=="Не применяется"))
                {
                    return "Брака нет";
                }
                else
                {
                    return "Брак есть";
                }
            }

            else
            {
                return "";
            }
        }
    }

    [MaxLength(128)]
    public string Worker { get; set; }
    
    public long WorkId { get; set; }
    
    public virtual ICollection<OTKCheckLine> Lines { get; set; }
    
}

public class OTKCheckLine
{
    [Key]
    public long Id { get; set; }
    public long OTKCheckId { get; set; }
    [JsonIgnore]
    public OTKCheck OTKCheck { get; set; }
    [MaxLength(256)]
    public string ShortName { get; set; }
    [MaxLength(1024)]
    public string FullName { get; set; }
    
    
    [MaxLength(256)]
    public string TargetValue { get; set; }
    
    [NotMapped]
    public List<string> AvailableTargetValues { get; set; }
    
    [NotMapped]
    public List<string> AvailableValues { get; set; }
    
    [MaxLength(256)]
    public string MeasuredValue { get; set; }
    
    [MaxLength(64)]
    public string Value { get; set; }
    [MaxLength(256)]
    public string? Description { get; set; }
}