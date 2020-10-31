using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LMS.Utility;
using NToastNotify;
using Microsoft.EntityFrameworkCore;
using LMS.LMSConfig;

namespace LMS.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class ConfigController : Controller
    {

        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification;

        public ConfigController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _db = db;
            _toastNotification = toastNotification;

        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);




        public async Task<IActionResult> ConfigOptions()
        {

            var currentUser = await GetCurrentUserAsync();
            var configCheck = await _db.ConfigOptions.SingleOrDefaultAsync();

            //If running for the first time, create DB entry and set defaults

            if (configCheck == null) {

                var config = new ConfigOptions();

                config.AssessorCanSupport = false;
                config.FacilitatorCanSupport = true;
                config.CompanyName = "MyCompany";
                config.CompletedCourseMessage = "Well Done! You have completed all topics of your Course. You will be notified of your final results as soon as it is available.";
                config.MinutesPerQuest = 3;
                config.MultipleUserRolePerClassEnrol = false;

                _db.ConfigOptions.Add(config);
                await _db.SaveChangesAsync();

            }

            //Get the config settings

            var configOptions = await _db.ConfigOptions.SingleOrDefaultAsync();

            if (!User.IsInRole(SD.Administrator)) {

                return NotFound("You are not a administrator of the LMS!");

            }
            
            return View(configOptions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetConfig(string company, string facSup, string assSup, string multiRole, string courseCom, int quesMin)
        {
            if (!ModelState.IsValid) {
               _toastNotification.AddErrorToastMessage("Complete All Fields Correctly!",
                       new ToastrOptions
                       {
                           Title = "Error! Not Successfull!",
                           ToastClass = "toastbox",
                       }
                       );
                return RedirectToAction("ConfigOptions","Config");
            }

            var config = await _db.ConfigOptions.SingleOrDefaultAsync(p=>p.Id == 1);

            config.CompanyName = company;   
            config.MinutesPerQuest = quesMin;     
            config.CompletedCourseMessage = courseCom;

            config.FacilitatorCanSupport = facSup == "true" ? true : false;
            config.AssessorCanSupport = assSup == "true" ? true : false;
            config.MultipleUserRolePerClassEnrol = multiRole == "true" ? true : false;

            _db.ConfigOptions.Update(config);
            await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("Configuration Updated!",
                       new ToastrOptions
                       {
                           Title = "Success!",
                           ToastClass = "toastbox",
                       }
                       );
            return RedirectToAction("ConfigOptions", "Config");
        }


    }
}