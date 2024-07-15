using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtoLib2.Model;

public class WorkStatusLog
{
    public long Id { get; set; }
    public long WorkId { get; set; }
    public long OrderNumber { get; set; }

    [MaxLength(128)] public string Article { get; set; }

    [MaxLength(32)] public string PostId { get; set; }

    public WorkStatus PrevStatus { get; set; }
    public WorkStatus NewStatus { get; set; }
    public DateTime Stamp { get; set; }
    public string EditedBy { get; set; }


    public decimal? SingleCost { get; set; }
    public int? OrderLineNumber { get; set; }
    public string? ProductionLineId { get; set; }
    public int? Count { get; set; }
    public string? MovedFrom { get; set; }
    public string? MovedTo { get; set; }

    [NotMapped]
    public decimal TotalCost
    {
        get
        {
            if (Count == null || SingleCost == null)
                return 0;
            return Count.Value * SingleCost.Value;
        }
    }


    [NotMapped]
    public string Action
    {
        get
        {
            if (PrevStatus == WorkStatus.unkown && NewStatus == WorkStatus.hidden)
                return "Создание";
            if (PrevStatus == NewStatus)
                return "Разделил";
            return $"Статус изменен с {WorkStatusMapper.Map(PrevStatus)} на {WorkStatusMapper.Map(NewStatus)}";
        }
    }
}