using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtoLib.Model;

public class DailySource
{
    [Key]
    public long Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public string FilledBy { get; set; }
    public decimal Value { get; set; }
    public string PostId { get; set; }
    public Post Post { get; set; }
    
   
    [MaxLength(32)]
    public string ProductLineId { get; set; }
    public ProductionLine ProductLine { get; set; }
}