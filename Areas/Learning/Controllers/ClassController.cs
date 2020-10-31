using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Extentions;
using LMS.Models;
using LMS.Models.Assessment;
using LMS.Models.Course;
using LMS.Models.ViewModels;
using LMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace LMS.Areas.Learning.Controllers
{

    [Area("Learning")]
    [Authorize(Roles = "Administrator,Designer")]
    public class ClassController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;


        private RoleManager<IdentityRole> _roleManager;

        public ClassController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _db = db;
            _roleManager = roleManager;
            _toastNotification = toastNotification;
        }


        public IActionResult ClassList() {

            ClassViewModel classVM = new ClassViewModel()
            {
                Classes = new List<Class>(),
                Courses = new List<Models.Course.Course>()
            };


            classVM.Classes = _db.Classes.ToList();
            classVM.Courses = _db.Courses.ToList();


            return View(classVM);
        }


        public IActionResult Create() {

            ClassViewModel classVM = new ClassViewModel()
            {
                Courses = new List<Models.Course.Course>(),
                Class = new Class() { StartDate = DateTime.Now, EndDate = DateTime.Now }

            };

            classVM.Courses = _db.Courses.ToList();

            return View(classVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassViewModel classVM)
        {

            var selectedCourse = _db.Courses.Where(p => p.Id == classVM.Class.CourseId);
            //classVM.Class.Course.Name = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                ClassViewModel model = new ClassViewModel()
                {
                    Courses = new List<Models.Course.Course>(),
                    Class = classVM.Class
                };

                model.Courses = _db.Courses.ToList();
                return View(model);
            }

            //Check to see if there is allready a summative with the same name

            var classExists = _db.Classes.Where(s =>
                s.Name == classVM.Class.Name &&
                s.CourseId == classVM.Class.CourseId);

            // If it does, oops:

            if (classExists.Any())
            {
                _toastNotification.AddErrorToastMessage("Please use another name",
                    new ToastrOptions
                    {
                        Title = "Allready Exists!",
                        ToastClass = "toastbox",
                    }
                    );
                ModelState.AddModelError("Name", "Another Class with the same name allready exist for this Course. Please choose another name.");

                ClassViewModel model = new ClassViewModel()
                {
                    Courses = new List<Models.Course.Course>(),
                    Class = classVM.Class

                };

                model.Courses = _db.Courses.ToList();

                return View(model);

            }


            _db.Add(classVM.Class);
            await _db.SaveChangesAsync();

            //FOREACH ASSIGNMENT & PRACT in Course of class
            //SubmissionValidity subVal = new SubmissionValidity();
            //subVal.ClassId = classVM.Class.Id;

            //_db.Add(subVal);

            //await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("New Class was Created!",
                    new ToastrOptions
                    {
                        Title = "Success",
                        ToastClass = "toastbox",
                    }
                    );
            return RedirectToAction("ClassList", "Class");

        }


        public async Task<IActionResult> Edit(int Id)
        {

            var classFromDb = await _db.Classes.SingleOrDefaultAsync(p => p.Id == Id);

            return View(classFromDb);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Class editedClass)
        {

            var selectedCourse = _db.Courses.Where(p => p.Id == editedClass.CourseId);

            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please check the fields",
                    new ToastrOptions
                    {
                        Title = "Error!",
                        ToastClass = "toastbox",
                    }
                    );
                return View(editedClass);
            }

            //Check to see if there is allready a Class with the same name in the selected course

            var classExists = _db.Classes.Where(s =>
                s.Name == editedClass.Name &&
                s.CourseId == editedClass.CourseId).Where(s => s.Id != editedClass.Id);

            // If it does, oops:

            if (classExists.Any())
            {
                _toastNotification.AddErrorToastMessage("Please use another name",
                    new ToastrOptions
                    {
                        Title = "Allready Exists!",
                        ToastClass = "toastbox",
                    }
                    );
                ModelState.AddModelError("Name", "Another Class with the same name allready exist for this Course. Please choose another name.");

                return View(editedClass);

            }

            _db.Update(editedClass);
            await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("Class was Updated!",
                    new ToastrOptions
                    {
                        Title = "Success",
                        ToastClass = "toastbox",
                    }
                    );
            return RedirectToAction("ClassList", "Class");

        }


        public async Task<IActionResult> Delete(int Id)
        {

            var classFromDb = await _db.Classes.SingleOrDefaultAsync(p => p.Id == Id);

            if (classFromDb.EnrolIds != null && classFromDb.EnrolIds != "")
            {
                _toastNotification.AddErrorToastMessage("Solution: Remove all enrolments first!",
                    new ToastrOptions
                    {
                        Title = "Action Not Permitted! The Class Contains Enrolments. ",
                        ToastClass = "toastbox",
                    }
                    );
                return RedirectToAction("ClassList", "Class");
            }

            return View(classFromDb);

        }




        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var classFromDb = await _db.Classes.SingleOrDefaultAsync(p => p.Id == id);

            if (!string.IsNullOrEmpty(classFromDb.EnrolIds))
            {
                _toastNotification.AddErrorToastMessage("Solution: Remove all enrolments first!",
                    new ToastrOptions
                    {
                        Title = "Action Not Permitted! The Class Contains Enrolments. ",
                        ToastClass = "toastbox",
                    }
                    );
                return RedirectToAction("ClassList", "Class");
            }

            var subValidities = await _db.SubmissionValidities.Where(p => p.ClassId == classFromDb.Id).ToListAsync();


            _db.RemoveRange(subValidities);

            _db.Classes.Remove(classFromDb);

            await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("Class Deleted!",
                new ToastrOptions
                {
                    Title = "Success",
                    ToastClass = "toastbox",
                }
                );
            return RedirectToAction("ClassList", "Class");


        }



        public async Task<IActionResult> ClassUnitSubmission(int? courseId, int classId)
        {
            if (courseId == null)
            {
                return NotFound();
            }

            CourseUnitViewModel model = new CourseUnitViewModel
            {
                Course = await _db.Courses.SingleOrDefaultAsync(c => c.Id == courseId),
                CourseUnit = new CourseUnit(), SubmissionValidity = new SubmissionValidity(),
                CourseUnitAssignments = await _db.CourseUnitAssignments.Where(c => c.CourseId == courseId).ToListAsync()
            };
            var units = new List<CourseUnit>();
            var modules = new List<CourseModule>();

            if (model.CourseUnitAssignments != null)
            {

                foreach (var item in model.CourseUnitAssignments)
                {

                    units.AddRange(_db.CourseUnits.Where(p => p.Id == item.CourseUnitId).ToList());
                    modules.AddRange(_db.CourseModules.Where(p => p.Id == item.CourseModuleId).ToList());
                };

                model.CourseUnitList = units;
                model.CourseModuleList = modules;

            }

            //Peparing Module List in correct order ?????? 

            if (model.Course.CourseModuleOrder != null && model.Course.CourseModuleOrder != "")
            {

                List<CourseModule> orderedModuleList = new List<CourseModule>();
                CourseModule selectedModule = new CourseModule();

                foreach (var modId in OrderList.ItemIdOrder(model.Course.CourseModuleOrder))
                {

                    selectedModule = model.CourseModuleList.Where(p => p.Id == modId).FirstOrDefault();

                    if (selectedModule != null)
                    {
                        orderedModuleList.Add(selectedModule);
                    }

                }

                model.CourseModuleList = orderedModuleList;


                //Peparing Unit List in correct order 

                List<CourseUnit> orderedUnitList = new List<CourseUnit>();
                CourseUnit selectedUnit = new CourseUnit();


                //List<SubmissionValidity> UnitSummValList = new List<SubmissionValidity>();

                var unitSummList = new List<Summative>();
                var unitSummListFinal = new List<Summative>();

                var unitValList = new List<SubmissionValidity>();

                Dictionary<int, List<Summative>> unitSumPairs = new Dictionary<int, List<Summative>>();
                Dictionary<int, List<SubmissionValidity>> unitValPairs = new Dictionary<int, List<SubmissionValidity>>();

                foreach (var mod in orderedModuleList)
                {
                    if (!string.IsNullOrEmpty(mod.CourseUnitOrder))
                    {
                        foreach (var unitId in OrderList.ItemIdOrder(mod.CourseUnitOrder))
                        {

                            selectedUnit = model.CourseUnitList.FirstOrDefault(p => p.Id == unitId);

                            if (selectedUnit != null)
                            {
                                orderedUnitList.Add(selectedUnit);

                                unitSummList = await _db.Summatives
                               .Where(p => p.AssessmentType == SD.Assignment || p.AssessmentType == SD.Practical)
                               .Where(p => p.CourseUnitId == selectedUnit.Id)
                               .ToListAsync();

                                unitSumPairs.Add(selectedUnit.Id, unitSummList);


                            }

                        }
                    }
                }

                List<SubmissionValidity> valsToDb = new List<SubmissionValidity>();
                List<SubmissionValidity> valsToDbFinal = new List<SubmissionValidity>();

                List<int> moduleIds = new List<int>();
                List<int> unitIds = new List<int>();

                foreach (var item in unitSumPairs)
                {

                    foreach (var summ in item.Value) {

                        var val = await _db.SubmissionValidities.Where(p => p.ClassId == classId).FirstOrDefaultAsync(p => p.SummativeId == summ.Id);

                        if (val == null)
                        {

                            var newVal = new SubmissionValidity() { Open = DateTime.Now, Close = DateTime.Now };
                            newVal.SummativeId = summ.Id;
                            newVal.ClassId = classId;

                            valsToDb.Add(newVal);

                        }

                    }


                }
                if (valsToDb.Count() > 0) {

                    _db.SubmissionValidities.AddRange(valsToDb);
                    await _db.SaveChangesAsync();
                }

                foreach (var item in unitSumPairs)
                {

                    foreach (var summ in item.Value)
                    {

                        var val = await _db.SubmissionValidities.Include(p => p.Summative).Where(p => p.ClassId == classId).SingleOrDefaultAsync(p => p.SummativeId == summ.Id);

                        if (val != null)
                        {

                            valsToDbFinal.Add(val);
                            unitIds.Add(item.Key);

                            foreach (var mod in orderedModuleList)
                            {
                                if (OrderList.ItemIdOrder(mod.CourseUnitOrder).Contains(item.Key))
                                {
                                    moduleIds.Add(mod.Id);
                                }
                            }

                        }

                    }

                    unitValPairs.Add(item.Key, valsToDbFinal);

                }


                model.ModsWithVals = moduleIds;
                model.UnitValidityPairs = unitValPairs;
                model.CourseUnitList = orderedUnitList;

            }


            return View(model);
        }



        public async Task<IActionResult> DateValidation(int valId)
        {
            if (!ModelState.IsValid)
            {

                return View();
            }

            var val = await _db.SubmissionValidities.Include(p=>p.Class).Include(p=>p.Summative).SingleOrDefaultAsync(p => p.Id == valId);

            var currentVal = new SubmissionValidity() {  };
                
           currentVal = val;
            
           return View(currentVal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int valId, SubmissionValidity val)
        {
            if (!ModelState.IsValid) {

                return View();

            }

            var valFromDb = await _db.SubmissionValidities
                .Include(p => p.Class)
                .SingleOrDefaultAsync(p => p.Id == valId);

            var classe = await _db.Classes.SingleOrDefaultAsync(p => p.Id == valFromDb.ClassId);

            valFromDb.Open = val.Open;
            valFromDb.Close = val.Close;

           _db.Update(valFromDb);
                       
            await _db.SaveChangesAsync();

            return RedirectToAction("ClassUnitSubmission", "Class", new { courseId = classe.CourseId, classId = valFromDb.ClassId });
        }
    }
}