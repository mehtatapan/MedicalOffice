using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalOffice.Data;
using MedicalOffice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.IO;

namespace MedicalOffice.Controllers
{
    [Authorize(Roles = "Admin,Supervisor")]
    public class AppointmentReasonsController : Controller
    {
        private readonly MedicalOfficeContext _context;

        public AppointmentReasonsController(MedicalOfficeContext context)
        {
            _context = context;
        }

        // GET: AppointmentReasons
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Lookups", new { Tab = ControllerName() + "Tab" });
        }

        // GET: AppointmentReasons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AppointmentReasons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ReasonName")] AppointmentReason appointmentReason)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(appointmentReason);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Lookups", new { Tab = ControllerName() + "Tab" });
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(appointmentReason);
        }

        // GET: AppointmentReasons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentReason = await _context.AppointmentReasons.FindAsync(id);
            if (appointmentReason == null)
            {
                return NotFound();
            }
            return View(appointmentReason);
        }

        // POST: AppointmentReasons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var appointmentReasonToUpdate = await _context.AppointmentReasons.FindAsync(id);
            if (appointmentReasonToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<AppointmentReason>(appointmentReasonToUpdate, "",
                d => d.ReasonName))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Lookups", new { Tab = ControllerName() + "Tab" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentReasonExists(appointmentReasonToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(appointmentReasonToUpdate);
        }

        // GET: AppointmentReasons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentReason = await _context.AppointmentReasons
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (appointmentReason == null)
            {
                return NotFound();
            }

            return View(appointmentReason);
        }

        // POST: AppointmentReasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointmentReason = await _context.AppointmentReasons.FindAsync(id);
            try
            {
                _context.AppointmentReasons.Remove(appointmentReason);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Lookups", new { Tab = ControllerName() + "Tab" });
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Reason for Appointment. Remember, you cannot delete a Reason that any patients have in their appointment history.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(appointmentReason);

        }

        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        {
            //Note: This is a very basic example and has 
            //no ERROR HANDLING.  It also assumes that
            //duplicate values are allowed, both in the 
            //uploaded data and the DbSet.
            ExcelPackage excel;
            using (var memoryStream = new MemoryStream())
            {
                await theExcel.CopyToAsync(memoryStream);
                excel = new ExcelPackage(memoryStream);
            }
            var workSheet = excel.Workbook.Worksheets[0];
            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;

            //Start a new list to hold imported objects
            List<AppointmentReason> appointmentReasons = new List<AppointmentReason>();

            for (int row = start.Row; row <= end.Row; row++)
            {
                // Row by row...
                AppointmentReason a = new AppointmentReason
                {
                    ReasonName = workSheet.Cells[row, 1].Text
                };
                appointmentReasons.Add(a);
            }
            _context.AppointmentReasons.AddRange(appointmentReasons);
            _context.SaveChanges();
            return RedirectToAction("Index", "Lookups", new { Tab = "AppointmentReasonsTab" });
        }

        //Add this...
        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }

        private bool AppointmentReasonExists(int id)
        {
            return _context.AppointmentReasons.Any(e => e.ID == id);
        }
    }
}
