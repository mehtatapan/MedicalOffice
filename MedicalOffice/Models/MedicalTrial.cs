using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Models
{
    public class MedicalTrial
    {
        public MedicalTrial()
        {
            Patients = new HashSet<Patient>();
        }

        public int ID { get; set; }

        [Display(Name = "Trial Name")]
        [Required(ErrorMessage = "You cannot leave the name of the trial blank.")]
        [StringLength(100, ErrorMessage = "Trial name cannot be more than 100 characters long.")]
        [DataType(DataType.MultilineText)]
        [DisplayFormat(NullDisplayText = "None")]
        public string TrialName { get; set; }

        public virtual ICollection<Patient> Patients { get; set; }
    }
}
