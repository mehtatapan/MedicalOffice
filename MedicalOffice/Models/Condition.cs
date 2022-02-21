using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Models
{
    public class Condition
    {
        public Condition()
        {
            PatientConditions = new HashSet<PatientCondition>();
        }

        public int ID { get; set; }

        [Display(Name = "Medical Condition")]
        [Required(ErrorMessage = "You cannot leave the name of the condition blank.")]
        [StringLength(50, ErrorMessage = "Too Big!")]
        public string ConditionName { get; set; }

        [Display(Name = "History")]
        public ICollection<PatientCondition> PatientConditions { get; set; }
    }
}
