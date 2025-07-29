using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    
   
    public string AvailableValues { get; set; }
    public string AvailableTargetValues { get; set; }
    [NotMapped]
    public List<string> Values
    {
       
        get
        {
            if (!string.IsNullOrEmpty(this.AvailableValues) )
            {
                return this.AvailableValues.Split(';').ToList();    
            }
            else
            {
                return new List<string>();
            }
            
        }
        set
        {
            this.AvailableValues = string.Join(';', value);
        }
    }

    [NotMapped]
    public List<string> TargetValues
    {
        get
        {
            if (!string.IsNullOrEmpty(this.AvailableTargetValues) )
            {
                return this.AvailableTargetValues.Split(';').ToList();    
            }
            else
            {
                return new List<string>();
            }
            
        }
        set
        {
            this.AvailableTargetValues = string.Join(';', value);
        }
    }
    

}

public class OTKWorker
{
    [Key]
    public long Id { get; set; }
    [MaxLength(128)]
    public string Name { get; set; }
}