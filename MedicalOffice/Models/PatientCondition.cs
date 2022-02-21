using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Models
{
    public class PatientCondition
    {
        public int ConditionID { get; set; }
        public Condition Condition { get; set; }

        public int PatientID { get; set; }
        public Patient Patient { get; set; }
    }
}
