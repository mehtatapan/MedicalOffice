using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Models
{
    public class DoctorSpecialty
    {
        public int DoctorID { get; set; }
        public Doctor Doctor { get; set; }

        public int SpecialtyID { get; set; }
        public Specialty Specialty { get; set; }
    }
}
