using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    
    public string Values { get; set; }
    [NotMapped]
    public List<string> ValueList {
        get
        {
            if (!string.IsNullOrEmpty(this.Values) && this.Values.Contains(";"))
            {
                return this.Values.Split(';').ToList();    
            }
            else
            {
                return new List<string>();
            }
            
        }
        set
        {
            this.Values = string.Join(':', value);
        }
    } 
}