using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Models.Course;
using LMS.Models.ViewModels;
using System.IO;
using LMS.Utility;
using Microsoft.AspNetCore.Authorization;

namespace LMS.Areas.Admin.Controllers
{
    [Area("Course")]
    [Authorize(Roles = "Administrator")]
    public class CourseController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

      [BindProperty]
       public CourseViewModel CourseViewModel { get; set; }

       
        public CourseController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {

            _db = db;
            _hostingEnvironment = hostingEnvironment;

           CourseViewModel = new CourseViewModel
           {
                Category = _db.Categories,
                Course = new Models.Course.Course()

           };

        }

        public async Task<IActionResult> Index()
        {
            var courses = await _db.Courses.Include(c => c.Category).Include(c => c.SubCategory).ToListAsync();
            return View(courses);
        }

        //GET - CREATE
        public IActionResult Create() {
            
            return View(CourseViewModel);

        }

        //POST - CREATE
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse() {

                       
            //As the subcategory id is populated using JS in the viewfile, it is not available in the viewmodel
            //Adding it here to make it available

            CourseViewModel.Course.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());


            if (!ModelState.IsValid)
            {
                return View(CourseViewModel);
            }

            // SubCat Fixed
            var subCat = await _db.SubCategories.SingleOrDefaultAsync(p => p.Id == CourseViewModel.Course.SubCategoryId);
            subCat.CanDelete = false;
            _db.Update(subCat);

            // Category Fixed
            var cat = await _db.Categories.SingleOrDefaultAsync(p => p.Id == CourseViewModel.Course.CategoryId);
            cat.CanDelete = false;
            _db.Update(cat);

            // Adding Course to DB

            _db.Courses.Add(CourseViewModel.Course);

            await _db.SaveChangesAsync();

            //Processing Image

            string webRootPath = _hostingEnvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;

            var courseFromDb = await _db.Courses.FindAsync(CourseViewModel.Course.Id);

            if (files.Count() > 0)
            {
                //Files was uploaded

                var uploads = Path.Combine(webRootPath, "images");
                var extention = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads, CourseViewModel.Course.Id + extention), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);

                }

                courseFromDb.Image = @"\images\" + CourseViewModel.Course.Id + extention;

            }
            else {

                //No Files was uploaded, so use default image
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultProfileImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + CourseViewModel.Course.Id + ".png");
                courseFromDb.Image = @"\images\" + CourseViewModel.Course.Id + ".png";

            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) {
                return NotFound();
            }

            CourseViewModel.Course = await _db.Courses.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync( m =>  m.Id == id);
            
            CourseViewModel.SubCategory = await _db.SubCategories.Where(s => s.CategoryId == CourseViewModel.Course.CategoryId).ToListAsync();

            if (CourseViewModel.Course == null) {

                return NotFound();

            }

            return View(CourseViewModel);

        }

        //POST - EDIT
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse()
        {
             //As the subcategory id is populated using JS in the viewfile, it is not available in the viewmodel
            //Adding it here to make it available

            CourseViewModel.Course.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());


            if (!ModelState.IsValid)
            {
                //Populate the subcategories first before returning view (otherwise it will be empty)

                CourseViewModel.SubCategory = await _db.SubCategories.Where(s => s.CategoryId == CourseViewModel.Course.CategoryId).ToListAsync();

                return View(CourseViewModel);
            }

            var courseFromDb = await _db.Courses.FindAsync(CourseViewModel.Course.Id);

            var previousSubCatId = courseFromDb.SubCategoryId; // Current SubCatID in DB
            var previousCatId = courseFromDb.CategoryId; // Current SubCatID in DB


            //Processing Image

            string webRootPath = _hostingEnvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;

            if (files.Count() > 0)
            {
                // New Image was uploaded

                var uploads = Path.Combine(webRootPath, "images");
                var extention_new = Path.GetExtension(files[0].FileName);

                // Delete the original file - also trim the first slash as there is allready a forward slash in front of the string

                var imagePath = Path.Combine(webRootPath, courseFromDb.Image.TrimStart('\\'));

                // Check if exists then delete

                if (System.IO.File.Exists(imagePath)) {

                    System.IO.File.Delete(imagePath);
                }

                // Upload new file

                using (var filesStream = new FileStream(Path.Combine(uploads, CourseViewModel.Course.Id + extention_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);

                }

                courseFromDb.Image = @"\images\" + CourseViewModel.Course.Id + extention_new;

            }

            //Update other fields

            courseFromDb.Name = CourseViewModel.Course.Name;
            courseFromDb.Description = CourseViewModel.Course.Description;
            courseFromDb.Reference = CourseViewModel.Course.Reference;
            courseFromDb.Duration = CourseViewModel.Course.Duration;
            courseFromDb.CategoryId = CourseViewModel.Course.CategoryId;
            courseFromDb.SubCategoryId = CourseViewModel.Course.SubCategoryId;

            // SubCat Fixed
            var subCatCurrent = await _db.SubCategories.SingleOrDefaultAsync(p => p.Id == CourseViewModel.Course.SubCategoryId);
            var subCatPrevious = await _db.SubCategories.SingleOrDefaultAsync(p => p.Id == previousSubCatId);
            var subCatOtherCourses = await _db.Courses.Where(p => p.SubCategoryId == previousSubCatId).Where(p => p.Id != courseFromDb.Id).ToListAsync();

            //If the subcat changed and no others courses are using the previous subCat reset its delete status

            if (subCatCurrent.Id != subCatPrevious.Id && subCatOtherCourses.Count() == 0) {
                subCatPrevious.CanDelete = true;
                _db.Update(subCatPrevious);
            }

            // Cat Fixed
            var catCurrent = await _db.Categories.SingleOrDefaultAsync(p => p.Id == CourseViewModel.Course.CategoryId);
            var catPrevious = await _db.Categories.SingleOrDefaultAsync(p => p.Id == previousCatId);
            var catOtherCourses = await _db.Courses.Where(p => p.CategoryId == previousCatId).Where(p => p.Id != courseFromDb.Id).ToListAsync();

            //If the cat changed and no others courses are using the previous Cat reset its delete status

            if (catCurrent.Id != catPrevious.Id && catOtherCourses.Count() == 0)
            {
                catPrevious.CanDelete = true;
                _db.Update(catPrevious);
            }

            subCatCurrent.CanDelete = false;
            _db.Update(subCatCurrent);

            catCurrent.CanDelete = false;
            _db.Update(catCurrent);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }



    }


}