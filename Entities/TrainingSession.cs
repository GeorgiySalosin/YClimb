using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YClimb.Entities
{
    public class TrainingSession
    {
        public TrainingSession() { }

        public TrainingSession(int trainingGroupId, int trainerId, DateTime startTime, DateTime endTime)
        {
            TrainingGroupId = trainingGroupId;
            TrainerId = trainerId;
            StartTime = startTime;
            EndTime = endTime;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("TrainingGroup")]
        public int TrainingGroupId { get; set; }
        public virtual TrainingGroup TrainingGroup { get; set; }

        [ForeignKey("Trainer")]
        public int TrainerId { get; set; }
        public virtual Trainer Trainer { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
