using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Models
{
    public class City
    {
        public int ID { get; set; }

        [Display(Name = "City")]
        [DisplayFormat(NullDisplayText = "No City Specified")]
        public string CityProvince
        {
            get
            {
                return Name + ", " + Province?.Code;
            }
        }

        [Display(Name = "City Name")]
        [Required(ErrorMessage = "You cannot leave the name of the city blank.")]
        [StringLength(255, ErrorMessage = "City name cannot be more than 255 characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must select the Province.")]
        [Display(Name = "Province")]
        public int ProvinceID { get; set; }
        public Province Province { get; set; }

        public ICollection<Doctor> Doctors { get; set; }

    }
}
