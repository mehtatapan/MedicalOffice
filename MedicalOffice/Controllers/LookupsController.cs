using MedicalOffice.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Controllers
{
    [Authorize(Roles = "Admin,Supervisor")]
    public class LookupsController : Controller
    {
        private readonly MedicalOfficeContext _context;

        public LookupsController(MedicalOfficeContext context)
        {
            _context = context;
        }

        public IActionResult Index(string Tab)
        {
            ///Note: select the tab you want to load by passing in
            ///the ID of the tab such as MedicalTrialsTab, ConditionsTab
            ///or AppointmentReasonsTab
            ViewData["Tab"] = Tab;
            return View();
        }

        public PartialViewResult Conditions()
        {
            ViewData["ConditionsID"] = new
                SelectList(_context.Conditions
                .OrderBy(a => a.ConditionName), "ID", "ConditionName");
            return PartialView("_Conditions");
        }
        public PartialViewResult Specialties()
        {
            ViewData["SpecialtiesID"] = new
                SelectList(_context.Specialties
                .OrderBy(a => a.SpecialtyName), "ID", "SpecialtyName");
            return PartialView("_Specialties");
        }
        public PartialViewResult AppointmentReasons()
        {
            ViewData["AppointmentReasonsID"] = new
                SelectList(_context.AppointmentReasons
                .OrderBy(a => a.ReasonName), "ID", "ReasonName");
            return PartialView("_AppointmentReasons");
        }
        public PartialViewResult MedicalTrials()
        {
            ViewData["MedicalTrialsID"] = new
                SelectList(_context.MedicalTrials
                .OrderBy(a => a.TrialName), "ID", "TrialName");
            return PartialView("_MedicalTrials");
        }
    }
}
