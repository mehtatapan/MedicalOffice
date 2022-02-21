using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Models
{
    public class Appointment
    {
        public Appointment()
        {
            //Example of setting defaults
            AppointmentDate = DateTime.Today;
            ExtraFee = 20d;
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "You cannot leave the notes blank.")]
        [StringLength(2000, ErrorMessage = "Only 2000 characters for notes.")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Required(ErrorMessage = "You cannot leave the date for the appointment blank.")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "You must enter an amount for the extra fee.")]
        [Display(Name = "Extra Fee")]
        [DataType(DataType.Currency)]
        public double ExtraFee { get; set; }

        [Required(ErrorMessage = "You must select a Patient.")]
        [Display(Name = "Patient")]
        public int PatientID { get; set; }

        public Patient Patient { get; set; }

        //Note: Reason is not required
        [Display(Name = "Reason for Appointment")]
        public int? AppointmentReasonID { get; set; }

        [Display(Name = "Reason for Appointment")]
        public AppointmentReason AppointmentReason { get; set; }
    }
}
