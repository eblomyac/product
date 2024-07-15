using System.ComponentModel.DataAnnotations;

namespace ProtoLib2.Model;

public class ProductionLine
{
    [MaxLength(32)] [Key] public string Id { get; set; }
}