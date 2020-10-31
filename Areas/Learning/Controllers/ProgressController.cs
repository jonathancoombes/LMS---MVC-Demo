using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Models;
using LMS.Models.Assessment;
using LMS.Models.Course;
using LMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace LMS.Areas.Learning.Controllers
{
    [Area("Learning")]
    public class ProgressController : Controller
    {

        private readonly ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification;

        public ProgressController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IToastNotification toastNotification)
        {
            _db = db;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        [Authorize(Roles = "Administrator,Learner")]

        public async Task<IActionResult> ProgressReport()
        {
            var currentUser = await GetCurrentUserAsync();

            var classEnrolment = await _db.ClassEnrolments.Include(p => p.Class.Course).Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);
            var courseUnits = await _db.CourseUnitAssignments
                .Include(p => p.Course)
                .Include(p => p.CourseUnit)
                .Where(p => p.CourseId == classEnrolment.ClassId).ToListAsync();

            var topicsList = new List<CourseTopic>();

            foreach (var unit in courseUnits) {
                var courseTopics = await _db.CourseTopics.Where(p => p.CourseUnitId == unit.CourseUnitId).ToListAsync();
                topicsList.AddRange(courseTopics);

            }

            var summativeList = new List<Summative>();

            foreach (var unit in courseUnits)
            {
                var summ = await _db.Summatives.Where(p => p.CourseUnitId == unit.CourseUnitId).ToListAsync();
                summativeList.AddRange(summ);

            }

            LearnerProgressViewModel progressVM = new LearnerProgressViewModel
            {
                //CourseName = classEnrolment.Class.Course.Name,
                //ClassEnrolment = classEnrolment,
                //EnrolledFor = classEnrolment.EnrolmentDate - DateTime.Now,
                //TotalSA = summativeList.Count(),
                //TotalSACompleted = ,
                //TotalTopics = topicsList.Count(),
                //TotalTopicsCompleted = ,
                //TotalUnits = courseUnits.Count(),
                //TotalUnitsCompleted = ,
                //TotalProgressPercentage = ,


            };

            return View(progressVM);
        }


    } }