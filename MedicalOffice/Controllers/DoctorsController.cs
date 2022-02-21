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
using MedicalOffice.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace MedicalOffice.Controllers
{
    [Authorize]
    public class DoctorsController : Controller
    {
        private readonly MedicalOfficeContext _context;

        public DoctorsController(MedicalOfficeContext context)
        {
            _context = context;
        }

        // GET: Doctors
        public async Task<IActionResult> Index(int? page, int? pageSizeID)
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            var doctors = from d in _context.Doctors
                .Include(d => d.DoctorDocuments)
                .Include(d => d.City).ThenInclude(d => d.Province)
                .Include(d => d.DoctorSpecialties).ThenInclude(d => d.Specialty)
                          select d;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Doctor>.CreateAsync(doctors.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Doctors/Details/5
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> Details(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.City).ThenInclude(d => d.Province)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        public PartialViewResult ListOfSpecialtiesDetails(int id)
        {
            var query = from s in _context.DoctorSpecialties.Include(p => p.Specialty)
                        where s.DoctorID == id
                        orderby s.Specialty.SpecialtyName
                        select s;
            return PartialView("_ListOfSpecialities", query.ToList());
        }

        public PartialViewResult ListOfPatientsDetails(int id)
        {
            var query = from p in _context.Patients
                        where p.DoctorID == id
                        orderby p.LastName, p.FirstName
                        select p;
            return PartialView("_ListOfPatients", query.ToList());
        }

        public PartialViewResult ListOfDocumentsDetails(int id)
        {
            var query = from p in _context.DoctorDocuments
                        where p.DoctorID == id
                        orderby p.FileName
                        select p;
            return PartialView("_ListOfDocuments", query.ToList());
        }

        // GET: Doctors/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            Doctor doctor = new Doctor();
            PopulateAssignedSpecialtyData(doctor);
            PopulateDropDownLists();
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,MiddleName,LastName,CityID")] Doctor doctor, 
            string[] selectedOptions, List<IFormFile> theFiles)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            try
            {
                UpdateDoctorSpecialties(selectedOptions, doctor);
                if (ModelState.IsValid)
                {
                    await AddDocumentsAsync(doctor, theFiles);
                    _context.Add(doctor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { doctor.ID });
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            PopulateAssignedSpecialtyData(doctor);

            //Get the full city object for the Doctor and then populate DDL
            doctor.City = await _context.Cities.FindAsync(doctor.CityID);
            PopulateDropDownLists(doctor);
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
               .Include(d => d.City)
               .Include(d => d.DoctorDocuments)
               .Include(d => d.DoctorSpecialties).ThenInclude(d => d.Specialty)
               .FirstOrDefaultAsync(d => d.ID == id);

            if (doctor == null)
            {
                return NotFound();
            }

            PopulateAssignedSpecialtyData(doctor);
            PopulateDropDownLists(doctor);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions, List<IFormFile> theFiles)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            //Go get the Doctor to update
            //Go get the Doctor to update
            var doctorToUpdate = await _context.Doctors
                .Include(d=>d.City)
                .Include(d => d.DoctorDocuments)
                .Include(d => d.DoctorSpecialties)
                .ThenInclude(d => d.Specialty)
                .FirstOrDefaultAsync(p => p.ID == id);

            //Check that you got it or exit with a not found error
            if (doctorToUpdate == null)
            {
                return NotFound();
            }

            //Update the Doctor's Specialties
            UpdateDoctorSpecialties(selectedOptions, doctorToUpdate);

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Doctor>(doctorToUpdate, "",
                d => d.FirstName, d => d.MiddleName, d => d.LastName, d=>d.CityID))
            {
                try
                {
                    await AddDocumentsAsync(doctorToUpdate, theFiles);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { doctorToUpdate.ID });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctorToUpdate.ID))
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

            PopulateAssignedSpecialtyData(doctorToUpdate);
            PopulateDropDownLists(doctorToUpdate);
            return View(doctorToUpdate);
        }

        // GET: Doctors/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.DoctorSpecialties).ThenInclude(d => d.Specialty)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            var doctor = await _context.Doctors
                .Include(d => d.DoctorSpecialties).ThenInclude(d => d.Specialty)
                .FirstOrDefaultAsync(m => m.ID == id);
            try
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Doctor. Remember, you cannot delete a Doctor that has patients assigned.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(doctor);
        }

        private void PopulateAssignedSpecialtyData(Doctor doctor)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = _context.Specialties;
            var currentOptionsHS = new HashSet<int>(doctor.DoctorSpecialties.Select(b => b.SpecialtyID));
            //Instead of one list with a boolean, we will make two lists
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();
            foreach (var s in allOptions)
            {
                if (currentOptionsHS.Contains(s.ID))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.SpecialtyName
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.SpecialtyName
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }
        private void UpdateDoctorSpecialties(string[] selectedOptions, Doctor doctorToUpdate)
        {
            if (selectedOptions == null)
            {
                doctorToUpdate.DoctorSpecialties = new List<DoctorSpecialty>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(doctorToUpdate.DoctorSpecialties.Select(b => b.SpecialtyID));
            foreach (var s in _context.Specialties)
            {
                if (selectedOptionsHS.Contains(s.ID.ToString()))//it is selected
                {
                    if (!currentOptionsHS.Contains(s.ID))//but not currently in the Doctor's collection - Add it!
                    {
                        doctorToUpdate.DoctorSpecialties.Add(new DoctorSpecialty
                        {
                            SpecialtyID = s.ID,
                            DoctorID = doctorToUpdate.ID
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(s.ID))//but is currently in the Doctor's collection - Remove it!
                    {
                        DoctorSpecialty specToRemove = doctorToUpdate.DoctorSpecialties.FirstOrDefault(d => d.SpecialtyID == s.ID);
                        _context.Remove(specToRemove);
                    }
                }
            }
        }

        private SelectList ProvinceSelectList(int? selectedId)
        {
            return new SelectList(_context.Provinces
                .OrderBy(d => d.Name), "ID", "Name", selectedId);
        }

        private SelectList CitySelectList(int? ProvinceID, int? selectedId)
        {
            //The ProvinceID has been added so we can filter by it.
            var query = from c in _context.Cities.Include(c => c.Province)
                        select c;
            if (ProvinceID.HasValue)
            {
                query = query.Where(p => p.ProvinceID == ProvinceID);
            }
            return new SelectList(query.OrderBy(p => p.Name), "ID", "CityProvince", selectedId);
        }

        private void PopulateDropDownLists(Doctor doctor = null)
        {
            //Make sure you have Included the City object in the Doctor!
            if ((doctor?.CityID).HasValue)
            {
                ViewData["ProvinceID"] = ProvinceSelectList(doctor.City.ProvinceID);
                ViewData["CityID"] = CitySelectList(doctor.City.ProvinceID, doctor.CityID);
            }
            else
            {
                ViewData["ProvinceID"] = ProvinceSelectList(null);
                ViewData["CityID"] = CitySelectList(null, null);
            }
        }

        private async Task AddDocumentsAsync(Doctor doctor, List<IFormFile> theFiles)
        {
            foreach (var f in theFiles)
            {
                if (f != null)
                {
                    string mimeType = f.ContentType;
                    string fileName = Path.GetFileName(f.FileName);
                    long fileLength = f.Length;
                    //Note: you could filter for mime types if you only want to allow
                    //certain types of files.  I am allowing everything.
                    if (!(fileName == "" || fileLength == 0))//Looks like we have a file!!!
                    {
                        DoctorDocument d = new DoctorDocument();
                        using (var memoryStream = new MemoryStream())
                        {
                            await f.CopyToAsync(memoryStream);
                            d.FileContent.Content = memoryStream.ToArray();
                        }
                        d.FileContent.MimeType = mimeType;
                        d.FileName = fileName;
                        doctor.DoctorDocuments.Add(d);
                    };
                }
            }
        }

        [HttpGet]
        public JsonResult GetCities(int? ID)
        {
            return Json(CitySelectList(ID, null));
        }

        public async Task<FileContentResult> Download(int id)
        {
            var theFile = await _context.UploadedFiles
                .Include(d => d.FileContent)
                .Where(f => f.ID == id)
                .FirstOrDefaultAsync();
            return File(theFile.FileContent.Content, theFile.FileContent.MimeType, theFile.FileName);
        }

        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.ID == id);
        }
    }
}
