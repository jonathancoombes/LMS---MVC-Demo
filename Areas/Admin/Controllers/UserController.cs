using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Areas.Identity;
using LMS.Utility;
using Microsoft.AspNetCore.Identity;
using LMS.Models;
using LMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using NToastNotify;

namespace LMS.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification;

        public UserController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IToastNotification toastNotification)
        {
            _db = db;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            //Goal is to display a list of all the users, except the current(this) logged in user

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await _db.ApplicationUser.Where(u => u.Id != claim.Value).ToListAsync());


        }



        //_toastNotification.AddErrorToastMessage("There are no users are available for enrolment",
        //               new ToastrOptions
        //               {
        //                   Title = "No Users Available!",
        //                   ToastClass = "toastbox",
        //               }
        //               );

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Admins() {

            var admins = await _userManager.GetUsersInRoleAsync(SD.Administrator);
            var facilitators = await _userManager.GetUsersInRoleAsync(SD.Facilitator);
            var assessors = await _userManager.GetUsersInRoleAsync(SD.Assessor);
            var designers = await _userManager.GetUsersInRoleAsync(SD.Designer);

            if (admins.Count > 0)
            {

                UserRoleViewModel userRoleVM = new UserRoleViewModel()
                {
                    Administrators = admins,
                    Facilitators = facilitators,
                    Assessors = assessors,
                    Designers = designers,
                    DisplayRole = SD.Administrator,
                    ApplicationUser = new ApplicationUser()
                };

                return View(userRoleVM);
            }
            else {
                return Content("There are no Administrators");
            }


        }

        [Authorize(Roles = "Administrator")]

        public async Task<IActionResult> EditRoles(string Id) {

            var user = await _db.ApplicationUser.SingleOrDefaultAsync(p => p.Id == Id);
            var isAdmin = await _userManager.IsInRoleAsync(user, SD.Administrator);
            var isFacilitator = await _userManager.IsInRoleAsync(user, SD.Facilitator);
            var isAssessor = await _userManager.IsInRoleAsync(user, SD.Assessor);
            var isDesigner = await _userManager.IsInRoleAsync(user, SD.Designer);

            var currentUser = await GetCurrentUserAsync();

            

            UserRoleViewModel userRoleVM = new UserRoleViewModel()
            {
                AdminSelected = isAdmin,
                AssessorSelected = isAssessor,
                DesignerSelected = isDesigner,
                FacilitatorSelected = isFacilitator,
                ApplicationUser = user

            };
            if (currentUser == user)
            { userRoleVM.EditorIsAdmin = true; }  

            return View(userRoleVM);

        }



        [Authorize(Roles = "Administrator")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoles(string Id, UserRoleViewModel userRoleVM) {

            var user = await _db.ApplicationUser.SingleOrDefaultAsync(p=>p.Id == Id);

            var facilitatorEnrollments = await _db.ClassEnrolments.Where(p => p.UserId == Id).Where(p=>p.UserRole == SD.Facilitator).ToListAsync();
            var assessorEnrollments = await _db.ClassEnrolments.Where(p => p.UserId == Id).Where(p => p.UserRole == SD.Assessor).ToListAsync();

            if (userRoleVM.FacilitatorSelected == true && !await _userManager.IsInRoleAsync(user, SD.Facilitator))
            {
                await _userManager.AddToRoleAsync(user, SD.Facilitator);
                _toastNotification.AddSuccessToastMessage("Facilitator Role Added for " + user.Name + " " + user.Surname,
                                  new ToastrOptions
                                  {
                                      Title = "Success",
                                      ToastClass = "toastbox",
                                  }
                                  );
            }
            else if (userRoleVM.FacilitatorSelected == false && await _userManager.IsInRoleAsync(user, SD.Facilitator))
            {
                if (facilitatorEnrollments.Count() > 0) {
                                       
                    _toastNotification.AddErrorToastMessage("Solution: Remove Facilitator from Class first.",
                                   new ToastrOptions
                                   {
                                       Title = "Facilitator Enrolled in a Class!",
                                       ToastClass = "toastbox",
                                   }
                                   );
                    return RedirectToAction("EditRoles", "User", new { Id = Id });
                }
                await _userManager.RemoveFromRoleAsync(user, SD.Facilitator);
                _toastNotification.AddSuccessToastMessage("Facilitator Role removed for " + user.Name + " " + user.Surname,
                                 new ToastrOptions
                                 {
                                     Title = "Success",
                                     ToastClass = "toastbox",
                                 }
                                 );
            }


            if (userRoleVM.AdminSelected == true && !await _userManager.IsInRoleAsync(user, SD.Administrator))
            {
                await _userManager.AddToRoleAsync(user, SD.Administrator);
                _toastNotification.AddSuccessToastMessage("Administrator Role added for " + user.Name + " " + user.Surname,
                                 new ToastrOptions
                                 {
                                     Title = "Success",
                                     ToastClass = "toastbox",
                                 }
                                 );
            }
            else if (userRoleVM.AdminSelected == false && await _userManager.IsInRoleAsync(user, SD.Administrator))
            {
                await _userManager.RemoveFromRoleAsync(user, SD.Administrator);
                _toastNotification.AddSuccessToastMessage("Administrator Role removed for " + user.Name + " " + user.Surname,
                                 new ToastrOptions
                                 {
                                     Title = "Success",
                                     ToastClass = "toastbox",
                                 }
                                 );
            }


            if (userRoleVM.AssessorSelected == true && !await _userManager.IsInRoleAsync(user, SD.Assessor))
            {
                await _userManager.AddToRoleAsync(user, SD.Assessor);
                _toastNotification.AddSuccessToastMessage("Assessor Role Added for " + user.Name + " " + user.Surname,
                                 new ToastrOptions
                                 {
                                     Title = "Success",
                                     ToastClass = "toastbox",
                                 }
                                 );
            }
            if (userRoleVM.AssessorSelected == false && await _userManager.IsInRoleAsync(user, SD.Assessor))
            {
                if (assessorEnrollments.Count() > 0)
                {

                    _toastNotification.AddErrorToastMessage("Solution: Remove Assessor from Class first.",
                                   new ToastrOptions
                                   {
                                       Title = "Assessor Enrolled in a Class!",
                                       ToastClass = "toastbox",
                                   }
                                   );
                    return RedirectToAction("EditRoles", "User", new { Id = Id });
                }
                    await _userManager.RemoveFromRoleAsync(user, SD.Assessor);
                _toastNotification.AddSuccessToastMessage("Assessor Role removed for " + user.Name + " " + user.Surname,
                                 new ToastrOptions
                                 {
                                     Title = "Success",
                                     ToastClass = "toastbox",
                                 }
                                 );
            }

            if (userRoleVM.DesignerSelected == true && !await _userManager.IsInRoleAsync(user, SD.Designer))
            {
                await _userManager.AddToRoleAsync(user, SD.Designer);
                _toastNotification.AddSuccessToastMessage("Designer Role Added for " + user.Name + " " + user.Surname,
                                 new ToastrOptions
                                 {
                                     Title = "Success",
                                     ToastClass = "toastbox",
                                 }
                                 );
            }
            else if (userRoleVM.DesignerSelected == false && await _userManager.IsInRoleAsync(user, SD.Designer))
            {
                await _userManager.RemoveFromRoleAsync(user, SD.Designer);
                _toastNotification.AddSuccessToastMessage("Designer Role removed for " + user.Name + " " + user.Surname,
                                 new ToastrOptions
                                 {
                                     Title = "Success",
                                     ToastClass = "toastbox",
                                 }
                                 );
            }
                                          
                       
            return RedirectToAction("EditRoles", "User", new { Id = Id});
                        
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Designers()
        {

            var admins = await _userManager.GetUsersInRoleAsync(SD.Administrator);
            var facilitators = await _userManager.GetUsersInRoleAsync(SD.Facilitator);
            var assessors = await _userManager.GetUsersInRoleAsync(SD.Assessor);
            var designers = await _userManager.GetUsersInRoleAsync(SD.Designer);

            if (admins.Count > 0)
            {

                UserRoleViewModel userRoleVM = new UserRoleViewModel()
                {
                    Administrators = admins,
                    Facilitators = facilitators,
                    Assessors = assessors,
                    Designers = designers,
                    DisplayRole = SD.Designer,
                    ApplicationUser = new ApplicationUser()
                };

                return View(userRoleVM);
            }
            else
            {
                return Content("There are no Designers");
            }


        }
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Facilitators()
        {

            var admins = await _userManager.GetUsersInRoleAsync(SD.Administrator);
            var facilitators = await _userManager.GetUsersInRoleAsync(SD.Facilitator);
            var assessors = await _userManager.GetUsersInRoleAsync(SD.Assessor);
            var designers = await _userManager.GetUsersInRoleAsync(SD.Designer);

            if (admins.Count > 0)
            {

                UserRoleViewModel userRoleVM = new UserRoleViewModel()
                {
                    Administrators = admins,
                    Facilitators = facilitators,
                    Assessors = assessors,
                    Designers = designers,
                    DisplayRole = SD.Facilitator,
                    ApplicationUser = new ApplicationUser()
                };

                return View(userRoleVM);
            }
            else
            {
                return Content("There are no Facilitators");
            }


        }
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Assessors()
        {
            var admins = await _userManager.GetUsersInRoleAsync(SD.Administrator);
            var facilitators = await _userManager.GetUsersInRoleAsync(SD.Facilitator);
            var assessors = await _userManager.GetUsersInRoleAsync(SD.Assessor);
            var designers = await _userManager.GetUsersInRoleAsync(SD.Designer);

            if (admins.Count > 0)
            {

                UserRoleViewModel userRoleVM = new UserRoleViewModel()
                {
                    Administrators = admins,
                    Facilitators = facilitators,
                    Assessors = assessors,
                    Designers = designers,
                    DisplayRole = SD.Assessor,
                    ApplicationUser = new ApplicationUser()
                };

                return View(userRoleVM);
            }
            else
            {
                return Content("There are no Assessors");
            }

        }

        [Authorize(Roles = "Administrator,Facilitator,Assessor")]
        public async Task<IActionResult> Learners()
        {
            var learners = await _userManager.GetUsersInRoleAsync(SD.Learner);

            UserRoleViewModel userRoleVM = new UserRoleViewModel()
            {
                 Learners = learners

            };

            return View(userRoleVM);

        }
        [Authorize(Roles = "Administrator,Facilitator,Assessor")]
        public async Task<IActionResult> Lock(string id) {

            if (id == null) {

                return NotFound();
            }

            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null) {

                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);

            await _db.SaveChangesAsync();


            if (await _userManager.IsInRoleAsync(applicationUser, SD.Administrator))
            {
                return RedirectToAction(nameof(Admins));
            }
            else if(await _userManager.IsInRoleAsync(applicationUser, SD.Assessor)){
                return RedirectToAction(nameof(Assessors));
            }
            if (await _userManager.IsInRoleAsync(applicationUser, SD.Designer))
            {
                return RedirectToAction(nameof(Designers));
            }
            else if (await _userManager.IsInRoleAsync(applicationUser, SD.Facilitator))
            {
                return RedirectToAction(nameof(Facilitators));
            }
            else
            {
                return RedirectToAction(nameof(Learners));
            }
        }
        [Authorize(Roles = "Administrator,Facilitator,Assessor")]
        public async Task<IActionResult> UnLock(string id)
        {

            if (id == null)
            {

                return NotFound();
            }

            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {

                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now;

            await _db.SaveChangesAsync();


            if (await _userManager.IsInRoleAsync(applicationUser, SD.Administrator))
            {
                return RedirectToAction(nameof(Admins));
            }
            else if (await _userManager.IsInRoleAsync(applicationUser, SD.Assessor))
            {
                return RedirectToAction(nameof(Assessors));
            }
            if (await _userManager.IsInRoleAsync(applicationUser, SD.Designer))
            {
                return RedirectToAction(nameof(Designers));
            }
            else if (await _userManager.IsInRoleAsync(applicationUser, SD.Facilitator))
            {
                return RedirectToAction(nameof(Facilitators));
            }
            else
            {
                return RedirectToAction(nameof(Learners));
            }

        }

    }
}