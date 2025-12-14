using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YClimb.Entities
{
    public class TrainingGroup
    {
        public TrainingGroup() { }

        public TrainingGroup(string name, string description = "-")
        {
            Name = name;
            Description = description;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
    }
}
