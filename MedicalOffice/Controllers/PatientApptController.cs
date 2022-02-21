using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalOffice.Data;
using MedicalOffice.Models;
using MedicalOffice.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace MedicalOffice.Controllers
{
    [Authorize(Roles = "Admin,Supervisor")]
    public class PatientApptController : Controller
    {
        private readonly MedicalOfficeContext _context;

        public PatientApptController(MedicalOfficeContext context)
        {
            _context = context;
        }

        // GET: PatientAppt
        public async Task<IActionResult> Index(int? PatientID, int? page, int? pageSizeID, int? AppointmentReasonID, string actionButton,
            string SearchString, string sortDirection = "desc", string sortField = "Appointment")
        {
            //Get the URL with the last filter, sort and page parameters from THE PATIENTS Index View
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, "Patients");

            if (!PatientID.HasValue)
            {
                //Get the proper return URL for the Patients controller
                return Redirect(ViewData["returnURL"].ToString());
            }

            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            PopulateDropDownLists();

            //Toggle the Open/Closed state of the collapse depending on if we are filtering
            ViewData["Filtering"] = "btn-outline-dark"; //Asume not filtering
            //Then in each "test" for filtering, add ViewData["Filtering"] = "btn-danger" if true;

            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Appt. Reason", "Appointment", "Extra Fees" };

            var appts = from a in _context.Appointments.Include(a => a.AppointmentReason).Include(a => a.Patient)
                        where a.PatientID == PatientID.GetValueOrDefault()
                        select a;

            if (AppointmentReasonID.HasValue)
            {
                appts = appts.Where(p => p.AppointmentReasonID == AppointmentReasonID);
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                appts = appts.Where(p => p.Notes.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }
            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted so lets sort!
            {
                page = 1;//Reset back to first page when sorting or filtering

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }
            //Now we know which field and direction to sort by.
            if (sortField == "Appt. Reason")
            {
                if (sortDirection == "asc")
                {
                    appts = appts
                        .OrderBy(p => p.AppointmentReason.ReasonName);
                }
                else
                {
                    appts = appts
                        .OrderByDescending(p => p.AppointmentReason.ReasonName);
                }
            }
            else if (sortField == "Extra Fees")
            {
                if (sortDirection == "asc")
                {
                    appts = appts
                        .OrderBy(p => p.ExtraFee);
                }
                else
                {
                    appts = appts
                        .OrderByDescending(p => p.ExtraFee);
                }
            }
            else //Appointment Date
            {
                if (sortDirection == "asc")
                {
                    appts = appts
                        .OrderByDescending(p => p.AppointmentDate);
                }
                else
                {
                    appts = appts
                        .OrderBy(p => p.AppointmentDate);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            //Now get the MASTER record, the patient, so it can be displayed at the top of the screen
            Patient patient = _context.Patients
                .Include(p => p.Doctor)
                .Include(p => p.MedicalTrial)
                .Include(p => p.PatientThumbnail)
                .Include(p => p.PatientConditions).ThenInclude(pc => pc.Condition)
                .Where(p => p.ID == PatientID.GetValueOrDefault()).FirstOrDefault();
            ViewBag.Patient = patient;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Appointment>.CreateAsync(appts.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }


        // GET: PatientAppt/Add
        public IActionResult Add(int? PatientID, string PatientName)
        {
            //Get the URL with the last filter, sort and page parameters
            ViewDataReturnURL();

            if (!PatientID.HasValue)
            {
                return Redirect(ViewData["returnURL"].ToString());
            }

            ViewData["PatientName"] = PatientName;
            Appointment a = new Appointment()
            {
                PatientID = PatientID.GetValueOrDefault()
            };
            PopulateDropDownLists();
            return View(a);
        }

        // POST: PatientAppt/Add
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("ID,Notes,AppointmentDate,ExtraFee,PatientID,AppointmentReasonID")] Appointment appointment, string PatientName)
        {
            //Get the URL with the last filter, sort and page parameters
            ViewDataReturnURL();

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(appointment);
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            PopulateDropDownLists(appointment);
            ViewData["PatientName"] = PatientName;
            return View(appointment);
        }

        // GET: PatientAppt/Update/5
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Get the URL with the last filter, sort and page parameters
            ViewDataReturnURL();

            var appointment = await _context.Appointments
               .Include(a => a.AppointmentReason)
               .Include(a => a.Patient)
               .FirstOrDefaultAsync(m => m.ID == id);
            if (appointment == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(appointment);
            return View(appointment);
        }

        // POST: PatientAppt/Update/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            //Get the URL with the last filter, sort and page parameters
            ViewDataReturnURL();

            var appointmentToUpdate = await _context.Appointments
                .Include(a => a.AppointmentReason)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.ID == id);

            //Check that you got it or exit with a not found error
            if (appointmentToUpdate == null)
            {
                return NotFound();
            }

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Appointment>(appointmentToUpdate, "",
                p => p.Notes, p => p.AppointmentDate, p => p.ExtraFee, p => p.AppointmentReasonID))
            {
                try
                {
                    _context.Update(appointmentToUpdate);
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointmentToUpdate.ID))
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
            PopulateDropDownLists(appointmentToUpdate);
            return View(appointmentToUpdate);
        }

        // GET: PatientAppt/Remove/5
        public async Task<IActionResult> Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Get the URL with the last filter, sort and page parameters
            ViewDataReturnURL();

            var appointment = await _context.Appointments
                .Include(a => a.AppointmentReason)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        // POST: PatientAppt/Remove/5
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.AppointmentReason)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.ID == id);

            //Get the URL with the last filter, sort and page parameters
            ViewDataReturnURL();

            try
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(appointment);
        }

        private SelectList AppointmentReasonSelectList(int? id)
        {
            var dQuery = from d in _context.AppointmentReasons
                         orderby d.ReasonName
                         select d;
            return new SelectList(dQuery, "ID", "ReasonName", id);
        }

        private void PopulateDropDownLists(Appointment appointment = null)
        {
            ViewData["AppointmentReasonID"] = AppointmentReasonSelectList(appointment?.AppointmentReasonID);
        }

        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }
        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.ID == id);
        }
    }
}
