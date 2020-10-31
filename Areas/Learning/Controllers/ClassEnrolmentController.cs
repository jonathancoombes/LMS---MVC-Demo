using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Extentions;
using LMS.Models;
using LMS.Models.Course;
using LMS.Models.ViewModels;
using LMS.Utility;
using LMS.LMSConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace LMS.Areas.Learning.Controllers
{

    [Area("Learning")]

    public class ClassEnrolmentController : Controller
    {
        private IToastNotification _toastNotification;
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public ClassEnrolmentController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _db = db;
            _toastNotification = toastNotification;
        }

        [Authorize(Roles = "Administrator,Designer,Facilitator")]
        public IActionResult ClassEnrolmentList(int Id)
        {

            ClassEnrolmentViewModel classVM = new ClassEnrolmentViewModel()
            {
                ClassEnrolments = new List<ClassEnrolment>(),
                ClassEnrolment = new ClassEnrolment(),
                ApplicationUser = _db.ApplicationUser.ToList(),
                Class = new Class()

            };

            classVM.ClassEnrolments = _db.ClassEnrolments.Where(p => p.ClassId == Id);
            classVM.Class = _db.Classes.SingleOrDefault(p => p.Id == Id);
            //classVM.Learners = await _userManager.GetUsersInRoleAsync(SD.Learner);

            return View(classVM);
        }


        
        [Route("enrolment/myclasses")]
        [Authorize(Roles = "Learner")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> LearnerClasses()
        {

            var currentUser = await GetCurrentUserAsync();

            if (!User.IsInRole(SD.Learner)) {

                return NotFound("You are trying to access a resource intended for a learner");
            }

            ClassEnrolmentViewModel classVM = new ClassEnrolmentViewModel()
            {
                ClassEnrolments = new List<ClassEnrolment>(),
                Class = new Class(),
                User = new ApplicationUser()
                 
            };
            classVM.User = await _userManager.FindByIdAsync(currentUser.Id); 
            classVM.Courses = await _db.Courses.ToListAsync();
            classVM.ClassEnrolments = await _db.ClassEnrolments.Include(p=>p.Class.Course).Where(p => p.UserId == currentUser.Id).ToListAsync();

            return View(classVM);
        }







        public IActionResult EnrolLearner(int Id)
        {
            return RedirectToAction("Enrol", new { Id = Id, type = SD.Learner });
        }
        public IActionResult EnrolFacilitator(int Id)
        {
            return RedirectToAction("Enrol", new { Id = Id, type = SD.Facilitator });
        }
        public IActionResult EnrolAssessor(int Id)
        {
            return RedirectToAction("Enrol", new { Id = Id, type = SD.Assessor });
        }


        public async Task<IActionResult> Enrol(int Id, string type)
        {
            var currentClass = _db.Classes.SingleOrDefault(p => p.Id == Id);

            ClassEnrolmentViewModel classVM = new ClassEnrolmentViewModel()
            {
                ClassEnrolment = new ClassEnrolment() { ClassId = Id },
                ApplicationUser = new List<ApplicationUser>(),
                Class = new Class()
            };

            //All users
            classVM.ApplicationUser = await _userManager.GetUsersInRoleAsync(type);

            //All Class Enrolments for selected Class
            var currentClassEnrolments = _db.ClassEnrolments.Where(p => p.ClassId == Id).ToList();        
    
            //Get a list of users that is enrolled in Class
            var usersInClass = new List<ApplicationUser>();

            foreach (var enrol in currentClassEnrolments) {

                foreach (var user in classVM.ApplicationUser) {

                  if (user.Id == enrol.UserId && enrol.UserRole == type) {

                        usersInClass.Add(user);
                    }
                }
            }

            //Create a list of users in role except those alleady in class
            classVM.ApplicationUser = classVM.ApplicationUser.Except(usersInClass);
                              

            if (classVM.ApplicationUser.Count() > 0)
            {                        
                
                Dictionary<string, string> dictionaryUsers = new Dictionary<string, string>();

                var i = 1;
                foreach (var item in classVM.ApplicationUser.OrderBy(p => p.Surname))
                {

                    item.Name = i++ + ". " + item.Surname + " " + item.Name + " | ID No: " + item.IdentityNumber;
                    dictionaryUsers.Add(item.Id, item.Name);

                }

                SelectList finalUserList = new SelectList(
                dictionaryUsers.Select(x => new { Value = x.Key, Text = x.Value }),
                "Value",
                "Text"
                );

                classVM.UsersInRole = finalUserList;
                
                classVM.Class = currentClass;
                classVM.ClassEnrolment.Class = currentClass;

                return View("Enrol" + type, classVM);
            }

            else { 
            _toastNotification.AddErrorToastMessage("There are no users are available for enrolment",
                       new ToastrOptions
                       {
                           Title = "No Users Available!",
                           ToastClass = "toastbox",
                       }
                       );

                return RedirectToAction("ClassEnrolmentList", "ClassEnrolment", new { Id = Id });

            }
                  }

                              

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrol(ClassEnrolmentViewModel classVM, string type)
        {

            if (!ModelState.IsValid || type == null)
            {

                _toastNotification.AddErrorToastMessage("Enrolment not successfull. Please try again",
                         new ToastrOptions
                         {
                             Title = "Error Occured!",
                             ToastClass = "toastbox",
                         }
                         );

            };


            //Check to see if the User is allready enrolled in the same class || or if in the same class / role per ConfigSetting
            // It does not check if the user is enrolled in the same course in another class!!

            IQueryable<ClassEnrolment> classEnrolExists;

                classEnrolExists = _db.ClassEnrolments.Where(s =>
                s.UserId == classVM.ClassEnrolment.UserId &&
                s.ClassId == classVM.ClassEnrolment.ClassId);

            var config = await _db.ConfigOptions.SingleOrDefaultAsync();

            if (config.MultipleUserRolePerClassEnrol == true)
            {
               classEnrolExists = classEnrolExists.Where(s =>
               s.UserRole == classVM.ClassEnrolment.UserRole);
            }

            var selClass = _db.Classes.SingleOrDefault(p => p.Id == classVM.ClassEnrolment.ClassId);
            var selectedUser = _db.ApplicationUser.SingleOrDefault(p => p.Id == classVM.ClassEnrolment.UserId);

            // If it does, oops:

            if (classEnrolExists.Count() > 0)
            {
                _toastNotification.AddErrorToastMessage("User allready enrolled as an " + type + ". Multiple Roles Per User/Class is disabled.",
                    new ToastrOptions
                    {
                        Title = "Allready Enrolled!",
                        ToastClass = "toastbox",
                    });
                return RedirectToAction("Enrol", "ClassEnrolment", new { Id = selClass.Id, type = type});
               
            }
                           
                classVM.ClassEnrolment.UserName = selectedUser.Name;
                classVM.ClassEnrolment.UserSurname = selectedUser.Surname;
                classVM.ClassEnrolment.Identity = selectedUser.IdentityNumber;

                _db.Add(classVM.ClassEnrolment);
                await _db.SaveChangesAsync();


                if (selClass.EnrolIds == null || selClass.EnrolIds == "")
                {
                    selClass.EnrolIds = classVM.ClassEnrolment.Id.ToString();
                }
                else if (selClass.EnrolIds != null && selClass.EnrolIds != "")
                {
                    var finalOrder = String.Concat(selClass.EnrolIds + "," + classVM.ClassEnrolment.Id.ToString());
                    selClass.EnrolIds = finalOrder;

                }

                _db.Update(selClass);
                await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("The " + type + " is enrolled in the class!",
                        new ToastrOptions
                        {
                            Title = "Success",
                            ToastClass = "toastbox",
                        }
                        );
                return RedirectToAction("ClassEnrolmentList", "ClassEnrolment", new { Id = selClass.Id });

            }

                     

        public async Task<IActionResult> Delete(int Id)
        {

            var classEnrolFromDb = await _db.ClassEnrolments.SingleOrDefaultAsync(p => p.Id == Id);

            return View(classEnrolFromDb);

        }
               
        public async Task<IActionResult> Details(int Id)
        {

            var classEnrolFromDb = await _db.ClassEnrolments.SingleOrDefaultAsync(p => p.Id == Id);

            return View(classEnrolFromDb);

        }


        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var classEnrolFromDb = await _db.ClassEnrolments.SingleOrDefaultAsync(p => p.Id == id);
            var classId = classEnrolFromDb.ClassId;
            var selClass = _db.Classes.SingleOrDefault(p => p.Id == classId);
            
            //Removing Enrol ID from Class Table
                       
            var currentEnrolOrder = selClass.EnrolIds;

            if (currentEnrolOrder == null || currentEnrolOrder == "" || !OrderList.ItemIdOrder(currentEnrolOrder).Contains(id))
            {
                return NotFound();
            }

            selClass.EnrolIds = RePosition.Delete(currentEnrolOrder, id);

            _db.ClassEnrolments.Remove(classEnrolFromDb);
            _db.Update(selClass);

            var currentUser = await GetCurrentUserAsync();

            var learnerClass = await _db.ApplicationUser.Where(p => p.Id == classEnrolFromDb.UserId).SingleOrDefaultAsync();

            learnerClass.ActiveClassId = null;

            _db.Update(learnerClass);

            await _db.SaveChangesAsync();

            //!!!!!!!! - To complete!!!

            // All study data of learner saved in DB or Storage must be removed now / here. 
            // This is when / where the above actions should be defined.

            //!!!!!!!!


            _toastNotification.AddSuccessToastMessage("Class Enrolment Deleted!",
                new ToastrOptions
                {
                    Title = "Success",
                    ToastClass = "toastbox",
                }
                );
            return RedirectToAction("ClassEnrolmentList", "ClassEnrolment", new { Id = classId });


        }






        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> Activate(int Id)
        {
            var currentUser = await GetCurrentUserAsync();
          

            if (currentUser == null)
            {

                return NotFound("User not found");
            }
            currentUser.ActiveClassId = Id;
            _db.Update(currentUser);
            await _db.SaveChangesAsync();

            return RedirectToAction("LearnerClasses", "ClassEnrolment");
        }













    }
}