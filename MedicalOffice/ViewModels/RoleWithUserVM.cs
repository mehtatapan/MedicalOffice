using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.ViewModels
{
    public class RoleWithUserVM
    {
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Display(Name = "User Account")]
        public string UserName { get; set; }

        [Display(Name = "User Name")]
        public string UserFullName { get; set; }
    }
}
