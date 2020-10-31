using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Models;
using LMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LMS.Areas.Admin.Controllers
{
    [Area("Course")]
    [Authorize(Roles = "Administrator")]
    public class SubCategoryController : Controller
    {

        private readonly ApplicationDbContext _db;

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            //Loads the subcats and eager loads cats

            var subCategories = await _db.SubCategories.Include(c => c.Category).ToListAsync();

            return View(subCategories);
        }

        // GET - CREATE
        public async Task<IActionResult> Create()
        {
            CategoryAndSubCategoryViewModel model = new CategoryAndSubCategoryViewModel
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync()

            };
            return View(model);
        }


        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryAndSubCategoryViewModel model) {

            if (ModelState.IsValid) {

                //Check to see if there is allready a subcategory in the category with the same name

                var SubCatExists = _db.SubCategories.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                // If it does, oops:

                if (SubCatExists.Count() > 0)
                {
                    //ERROR OCURRED
                }

                //Otherwise, all is well so let's go:

                else {

                    _db.SubCategories.Add(model.SubCategory);
                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));

                }

            }

            //If the ModelState is not valid

            CategoryAndSubCategoryViewModel modelVM = new CategoryAndSubCategoryViewModel
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync()
            };

            return View(modelVM);

        }


        // GET - GetSubCategory
        public async Task<IActionResult> GetSubCategories(int id)
        {

            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategories
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
    
    }


        // GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var subCategory = await _db.SubCategories.SingleOrDefaultAsync(c => c.Id == id);

                if (subCategory == null)

                    return NotFound();

                else
                {

                    CategoryAndSubCategoryViewModel model = new CategoryAndSubCategoryViewModel
                    {
                        CategoryList = await _db.Categories.ToListAsync(),
                        SubCategory = subCategory,
                        SubCategoryList = await _db.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync()

                    };
                    return View(model);
                }
            }
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryAndSubCategoryViewModel model)
        {

            if (ModelState.IsValid)
            {

                //Check to see if there is allready a subcategory in the category with the same name

                var SubCatExists = _db.SubCategories.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                // If it does, oops:

                if (SubCatExists.Count() > 0)
                {
                    //ERROR OCURRED
                }

                //Otherwise, all is well so let's go and update the name:

                else
                {
                    var subCatFromDb = await _db.SubCategories.FindAsync(model.SubCategory.Id);
                    subCatFromDb.Name = model.SubCategory.Name;
                                                                            
                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));

                }

            }

            //If the ModelState is not valid

            CategoryAndSubCategoryViewModel modelVM = new CategoryAndSubCategoryViewModel
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync()
            };

            return View(modelVM);

        }


        // GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var subCategory = await _db.SubCategories.SingleOrDefaultAsync(c => c.Id == id);

                if (subCategory == null)

                    return NotFound();

                else
                {

                    CategoryAndSubCategoryViewModel model = new CategoryAndSubCategoryViewModel
                    {
                        CategoryList = await _db.Categories.ToListAsync(),
                        SubCategory = subCategory,
                        SubCategoryList = await _db.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync()

                    };
                    return View(model);
                }
            }
        }

        //GET - DELETE

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategories.FindAsync(id);

            if (subCategory == null)
            {
                return NotFound();
            }

            CategoryAndSubCategoryViewModel model = new CategoryAndSubCategoryViewModel
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync()

            };
            return View(model);
        

    }


        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var subCategory = await _db.SubCategories.FindAsync(id);
            var subCatInCourse = _db.Courses.Where(p => p.SubCategoryId == id);

            if (subCategory == null)
            {

                return NotFound();
            }
            else if (subCatInCourse.Count() > 0) {
                //!!Implement Message to user
                throw new Exception("Cannot Delete Subcategory currently used by a Course");
            }


            _db.SubCategories.Remove(subCategory);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
   

    
