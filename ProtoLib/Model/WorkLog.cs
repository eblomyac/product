using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtoLib.Model
{
    public class WorkStatusLog
    {
        public long Id { get; set; }
        public long WorkId { get; set; }
        public long OrderNumber { get; set; }
        [MaxLength(128)]
        public string Article { get; set; }
        [MaxLength(32)]
        public string PostId { get; set; }
      
        public WorkStatus PrevStatus { get; set; }
        public WorkStatus NewStatus { get; set; }
        public DateTime Stamp { get; set; }
        public string EditedBy { get; set; }
        
       
        

        [NotMapped]
        public string Action
        {
            get
            {
                if (PrevStatus == WorkStatus.unkown && NewStatus == WorkStatus.hidden)
                {
                    return "Создание";
                }else if (PrevStatus == NewStatus)
                {
                    return "Разделил";
                }
                else
                {
                    return $"Статус изменен с {WorkStatusMapper.Map(PrevStatus)} на {WorkStatusMapper.Map(NewStatus)}";
                }
            }
        }

    }
}