using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Models
{
    public class Province
    {
        public Province()
        {
            Cities = new HashSet<City>();
        }

        public int ID { get; set; }

        [Display(Name = "Two Letter Province Code")]
        [Required(ErrorMessage = "You cannot leave the province code blank.")]
        [StringLength(2, ErrorMessage = "Province Code can only be two capital letters.")]
        [RegularExpression("^\\p{Lu}{2}$", ErrorMessage = "Please enter two capital letters for the province code.")]
        public string Code { get; set; }

        [Display(Name = "Province Name")]
        [Required(ErrorMessage = "You cannot leave the name of the province blank.")]
        [StringLength(50, ErrorMessage = "Province name can only be 50 characters long.")]
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }
    }
}
