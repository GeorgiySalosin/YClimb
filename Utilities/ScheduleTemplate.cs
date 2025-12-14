using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YClimb.Entities
{
    public class ScheduleTemplate
    {
        public ScheduleTemplate() { }

        public ScheduleTemplate(DayOfWeek dayOfWeek, int trainingGroupId, int trainerId, TimeSpan startTime, TimeSpan endTime)
        {
            DayOfWeek = dayOfWeek;
            TrainingGroupId = trainingGroupId;
            TrainerId = trainerId;
            StartTime = startTime;
            EndTime = endTime;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        [ForeignKey("TrainingGroup")]
        public int TrainingGroupId { get; set; }
        public virtual TrainingGroup TrainingGroup { get; set; }

        [ForeignKey("Trainer")]
        public int TrainerId { get; set; }
        public virtual Trainer Trainer { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}