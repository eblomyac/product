using System.ComponentModel.DataAnnotations;

namespace ProtoLib.Model;

public class ProductionLine
{
    [MaxLength(32)]
    [Key]
    public string Id { get; set; }
}