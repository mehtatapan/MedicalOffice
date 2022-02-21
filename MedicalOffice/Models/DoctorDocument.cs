using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Models
{
    public class DoctorDocument : UploadedFile
    {
        [Display(Name = "Doctor")]
        public int DoctorID { get; set; }

        public Doctor Doctor { get; set; }
    }
}
