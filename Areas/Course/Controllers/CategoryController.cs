using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Areas.Admin.Controllers
{

    [Area("Course")]
    [Authorize(Roles = "Administrator")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        
        }

        //GET
        public async Task<IActionResult> Index()
        {
            return View(await _db.Categories.ToListAsync());
        }

        //GET - Create
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category) {

            if (ModelState.IsValid) {

                _db.Add(category);

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            //IF Modelstate not valid, return the view with the object passed in
            return View(category);
        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? id) {

            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Categories.FindAsync(id);

            if (category == null) {

                return NotFound();
            }

            return View(category);

        }
        //EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category) {

            if (ModelState.IsValid) {

                _db.Update(category);

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }

            return View(category);

        }


        //GET - DELETE

        public async Task<IActionResult> Delete(int? id) {

            if (id == null) {
                return NotFound();
            }

            var category = await _db.Categories.FindAsync(id);

            if (category == null) {
                return NotFound();
            }

            return View(category);

        }


        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {

            var category = await _db.Categories.FindAsync(id);

            if (category == null) {

                return NotFound();
            }

            _db.Categories.Remove(category);

            await _db.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
        }

        //DETAILS - GET
        public async Task<IActionResult> Details(int? id) {

            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);

            

        }



    }
}