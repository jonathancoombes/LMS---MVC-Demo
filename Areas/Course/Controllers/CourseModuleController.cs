using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Models.Course;
using LMS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Extentions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace LMS.Areas.Course.Controllers
{
    [Area("Course")]
    [Authorize(Roles = "Administrator,Designer")]
    public class CourseModuleController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CourseModuleController(ApplicationDbContext db)
        {
            _db = db;


        }

        // GET - CourseModules
        public async Task<IActionResult> GetCourseModules(int id)
        {

            List<CourseModule> courseModules = new List<CourseModule>();

            courseModules = await (from courseModule in _db.CourseModules
                                   where courseModule.CourseId == id
                                   select courseModule).ToListAsync();
            return Json(new SelectList(courseModules, "Id", "Title"));

        }



        //GET
        public async Task<IActionResult> Index()
        {
            return View(await _db.Courses.ToListAsync());
        }
        //GET
        public async Task<IActionResult> ModuleList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CourseModuleViewModel model = new CourseModuleViewModel
            {
                Course = await _db.Courses.SingleOrDefaultAsync(c => c.Id == id),
                CourseModule = new CourseModule(),
                CourseModuleList = await _db.CourseModules.Where(c => c.CourseId == id).ToListAsync()

            };

            var currentModuleOrder = model.Course.CourseModuleOrder;
            List<CourseModule> newList = new List<CourseModule>();

            if (currentModuleOrder != null)
            {

                var elements = currentModuleOrder?.Split(',').ToList();

                //Convert list of strings to list of ints
                List<int> modulesAsInts = new List<int>();

                foreach (var mods in elements)
                {

                    int result;

                    int.TryParse(mods, out result);

                    modulesAsInts.Add(result);
                }

                foreach (var modId in modulesAsInts)
                {

                    foreach (var module in model.CourseModuleList)
                    {

                        if (modId == module.Id)
                        {
                            newList.Add(module);
                        }
                    }
                }

                model.CourseModuleList = newList;

            }
            

            return View(model);



        }


        //GET - Create
        public async Task<IActionResult> Create(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            CourseModuleViewModel model = new CourseModuleViewModel
            {
                Course = await _db.Courses.SingleOrDefaultAsync(c => c.Id == id),
                CourseModule = new CourseModule(),
                CourseModuleList = await _db.CourseModules.Where(c => c.CourseId == id).ToListAsync()

            };


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseModuleViewModel courseModuleVM, int courseId)
        {

            if (ModelState.IsValid)
            {

                //Check to see if there is allready a module in the course with the same name

                var ModuleExists = _db.CourseModules.Include(s => s.Course).Where(s => s.Title == courseModuleVM.CourseModule.Title && s.Course.Id == courseModuleVM.CourseModule.CourseId);

                // If it does, oops:

                if (ModuleExists.Count() > 0)
                {
                    //ERROR OCURRED
                }

                //Otherwise, all is well so let's go:

                else
                {

                    courseModuleVM.CourseModule.CourseId = courseId;
                    _db.CourseModules.Add(courseModuleVM.CourseModule);


                    await _db.SaveChangesAsync();

                    int moduleId = courseModuleVM.CourseModule.Id;

                    var currentCourse = await _db.Courses.Where(c => c.Id == courseId).SingleOrDefaultAsync();
                    var currentModuleOrder = currentCourse.CourseModuleOrder;


                    if (currentModuleOrder == null || currentModuleOrder == "")
                    {

                        currentCourse.CourseModuleOrder = moduleId.ToString();

                        _db.Update(currentCourse);

                        await _db.SaveChangesAsync();

                    }
                    else if (currentModuleOrder != null)
                    {

                        var finalOrder = String.Concat(currentModuleOrder + "," + moduleId.ToString());
                        currentCourse.CourseModuleOrder = finalOrder;

                        _db.Update(currentCourse);

                        await _db.SaveChangesAsync();


                    }



                    return RedirectToAction("ModuleList", "CourseModule", new { id = courseId });

                }

            }
            //If the ModelState is not valid

            CourseModuleViewModel modelVM = new CourseModuleViewModel
            {
                Course = await _db.Courses.SingleOrDefaultAsync(c => c.Id == courseModuleVM.Course.Id),
                CourseModule = courseModuleVM.CourseModule,
                CourseModuleList = await _db.CourseModules.Where(c => c.CourseId == courseModuleVM.Course.Id).ToListAsync()
            };

            return View(modelVM);

        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var module = await _db.CourseModules.SingleOrDefaultAsync(c => c.Id == id);

                if (module == null)
                {

                    return NotFound();
                }
                else
                {
                    CourseModuleViewModel courseModuleVM = new CourseModuleViewModel
                    {

                        Course = await _db.Courses.SingleOrDefaultAsync(c => c.Id == id),
                        CourseModule = module,
                        CourseModuleList = await _db.CourseModules.Where(c => c.CourseId == id).ToListAsync()

                    };
                    return View(courseModuleVM);
                }
            }
        }
        //EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseModule courseModule, int courseId, string unitOrder)
        {

            if (ModelState.IsValid)
            {
                courseModule.CourseId = courseId;
                courseModule.CourseUnitOrder = unitOrder;

                _db.Update(courseModule);

                await _db.SaveChangesAsync();

                return RedirectToAction("ModuleList", "CourseModule", new { id = courseId });

            }

            return View(courseModule);

        }


        //GET - DELETE

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var module = await _db.CourseModules.FindAsync(id);

            if (module == null)
            {
                return NotFound();
            }

            return View(module);

        }


        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {       
            var currentModule = await _db.CourseModules.Include(c => c.Course).Where(c => c.Id == id).SingleOrDefaultAsync();
            var currentCourseId = currentModule.CourseId;
            var currentCourse = await _db.Courses.Where(c => c.Id == currentCourseId).SingleOrDefaultAsync();
            var currentModuleOrder = currentCourse.CourseModuleOrder;

            if (currentModule == null)
            {
                return NotFound();
            }
          
            currentCourse.CourseModuleOrder = RePosition.Delete(currentModuleOrder, id);

            _db.CourseModules.Remove(currentModule);     
            _db.Update(currentCourse);

            await _db.SaveChangesAsync();

            return RedirectToAction("ModuleList", "CourseModule", new { id = currentCourseId });

        }

        //DETAILS - GET
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var module = await _db.CourseModules.FindAsync(id);

            if (module == null)
            {
                return NotFound();
            }

            return View(module);

        }

        public async Task<IActionResult> MoveModuleUp(int Id)
        {
            var currentModule = await _db.CourseModules.Include(c => c.Course).Where(c => c.Id == Id).SingleOrDefaultAsync();
            var currentCourseId = currentModule.CourseId;

            var currentCourse = await _db.Courses.Where(c => c.Id == currentCourseId).SingleOrDefaultAsync();
            var currentModuleOrder = currentCourse.CourseModuleOrder;

            if (RePosition.CanMoveUpCheck(currentModuleOrder, Id))
            {
                currentCourse.CourseModuleOrder = RePosition.MoveUp(currentModuleOrder, Id);
                _db.Update(currentCourse);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("ModuleList", "CourseModule", new { id = currentCourseId });
        }

        public async Task<IActionResult> MoveModuleDown(int Id)
        {
            var currentModule = await _db.CourseModules.Include(c => c.Course).Where(c => c.Id == Id).SingleOrDefaultAsync();
            var currentCourseId = currentModule.CourseId;

            var currentCourse = await _db.Courses.Where(c => c.Id == currentCourseId).SingleOrDefaultAsync();
            var currentModuleOrder = currentCourse.CourseModuleOrder;

            if (RePosition.CanMoveDownCheck(currentModuleOrder, Id))
            {
                currentCourse.CourseModuleOrder = RePosition.MoveDown(currentModuleOrder, Id);
                _db.Update(currentCourse);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("ModuleList", "CourseModule", new { id = currentCourseId });
        }
                                   

    }
}