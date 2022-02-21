using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Models
{
    public class AppointmentReason
    {
        public AppointmentReason()
        {
            this.Appointments = new HashSet<Appointment>();
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "You cannot leave the name of the complaint blank.")]
        [Display(Name = "Reason for Apt.")]
        [StringLength(50, ErrorMessage = "Too Big!")]
        [DisplayFormat(NullDisplayText = "No Reason Given")]
        public string ReasonName { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
