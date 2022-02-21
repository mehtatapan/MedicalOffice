using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Models
{
    public class Specialty
    {
        public Specialty()
        {
            DoctorSpecialties = new HashSet<DoctorSpecialty>();
        }
        public int ID { get; set; }

        [Display(Name = "Medical Specialty")]
        [Required(ErrorMessage = "You cannot leave the name of the Specialty blank.")]
        [StringLength(100, ErrorMessage = "Too Big!")]
        public string SpecialtyName { get; set; }

        public ICollection<DoctorSpecialty> DoctorSpecialties { get; set; }
    }
}
