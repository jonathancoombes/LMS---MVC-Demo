using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LMS.Models;
using LMS.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using LMS.Models.ViewModels;
using LMS.Extentions;
using LMS.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using LMS.Models.Assessment;
using LMS.Models.Course;

namespace LMS.Controllers
{
    [Area("Learning")]

    public class HomeController : Controller 
    {
       

        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        private string _apiKey { get; set; }
        private string _privateKeyFile { get; set; }
        private bool _scopeUser { get; set; }
     

        public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IConfiguration config)
        {

            _db = db;
            _userManager = userManager;

            var opts = config.GetSection("TinyDrive");
            _apiKey = opts["apiKey"];
            _privateKeyFile = opts["privateKeyFile"];
            _scopeUser = Boolean.Parse(opts["scopeUser"]);
           
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public IActionResult Index()
        {
            return View();

        }



        [HttpPost]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<JsonResult> Jwt()
        {
            var currentUser = await GetCurrentUserAsync();
            var username = currentUser.UserName;
            var surname = currentUser.Surname;

            if (username != null)
            {
                var token = JwtHelper.CreateTinyDriveToken(username, surname, _scopeUser, _privateKeyFile);
                return Json(new { token });
            }
            else
            {
                var result = Json(new { error = "Failed to auth." });
                result.StatusCode = 403;
                return result;
            }
        }
        
        


        public async Task<IActionResult> Classroom()
        {
            var currentUser = await GetCurrentUserAsync();

            if (currentUser == null) {

                return NotFound("User is not logged in");

            }

            var currentEnrolment = await _db.ClassEnrolments.Include(p=>p.Class.Course).SingleOrDefaultAsync(p=>p.Id == currentUser.ActiveClassId);

            if (currentEnrolment == null)
            {

                return NotFound("User Not Enrolled in a Course");

            }
            else if (currentEnrolment.CompletedTopics == SD.AllComplete) {

                return RedirectToAction("CourseCompleted", "Home");

            }
                       
            if (currentEnrolment.CurrentTopicId == null)
            {
                // from enrolment -> current course
                var currentCourse = await _db.Courses.SingleOrDefaultAsync(p => p.Id == currentEnrolment.Class.CourseId);

            //from current course -> first module
            if (currentCourse.CourseModuleOrder != null && currentCourse.CourseModuleOrder != "")
            {
                var orderedModuleList = OrderList.ItemIdOrder(currentCourse.CourseModuleOrder);
                var currentModule = await _db.CourseModules.SingleOrDefaultAsync(p => p.Id == orderedModuleList[0]);

                //from first module -> first unit
                if (currentModule.CourseUnitOrder != null && currentModule.CourseUnitOrder != "")
                {
                    var orderedUnitList = OrderList.ItemIdOrder(currentModule.CourseUnitOrder);
                    var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(p => p.Id == orderedUnitList[0]);

                    //from first unit -> first topic
                    if (currentUnit.CourseTopicIds != null && currentUnit.CourseTopicIds != "")
                    {
                        var orderedTopicList = OrderList.ItemIdOrder(currentUnit.CourseTopicIds);
                        var currentTopic = await _db.CourseTopics.SingleOrDefaultAsync(p => p.Id == orderedTopicList[0]);


                        // CurrentTopicId = First Time / First Topic View (will be null on first view as not set previously, so we set it here)
                        // This is the "latest" possible time to accommodate possible changes to course design (order)..

                            currentEnrolment.CurrentTopicId = currentTopic.Id;
                            currentEnrolment.CurrentPage = 1;

                            _db.Update(currentEnrolment);
                            await _db.SaveChangesAsync();

                        }

                    }
                }

            }


            var topic = await _db.CourseTopics.Include(p=>p.CourseUnit).SingleOrDefaultAsync(p => p.Id == currentEnrolment.CurrentTopicId);

            if (topic == null)
            {

                return NotFound("Error. No current topic is set in the current enrolment.");

            }

            bool? isLast = null;
            var orderedTopics = new List<int>();

            //Checking if Topic is last in order for Unit
            if (topic.CourseUnit.CourseTopicIds != null && topic.CourseUnit.CourseTopicIds != "") {
            orderedTopics = OrderList.ItemIdOrder(topic.CourseUnit.CourseTopicIds);
            isLast = topic.Id == orderedTopics[orderedTopics.Count() - 1] ? true : false;

            }
                       

            ClassroomViewModel classVM = new ClassroomViewModel
            {
                ClassId = currentEnrolment.Id,
                CourseTopic = topic,
                IsLastTopicInUnit = isLast

            };
            
            //Check is Current Topic's FA's are all completed.

            if (topic.FAOrder != null && topic.FAOrder != "") {
            var topicFAList = OrderList.ItemIdOrder(topic.FAOrder);

                if (currentEnrolment.CompletedFas != null && currentEnrolment.CompletedFas != "") {
                var completedFAList = OrderList.ItemIdOrder(currentEnrolment.CompletedFas);
              
                foreach (var fa in topicFAList) {

                        if (!completedFAList.Contains(fa))
                        {
                            classVM.TopicFANotComplete = true;
                        }
                        else {
                            classVM.TopicFANotComplete = false;
                        }
                }
                }
                else {
                    classVM.TopicFANotComplete = true;
                    }
               
            }
            
            //Checking if topic contains formatives
                        if (topic.FAOrder != null && topic.FAOrder != "" && OrderList.ItemIdOrder(topic.FAOrder).Count() > 0) {

                        var formatives = OrderList.ItemIdOrder(topic.FAOrder);
                        bool result = formatives.Count() > 0 && formatives != null ? true : false;

                classVM.ContainsFormative = result;
                        }

                return View(classVM);
            
                }


        public IActionResult Privacy()
        {
            return View();
        }



        public IActionResult CourseCompleted()
        {
            var config = _db.ConfigOptions.SingleOrDefault();

            ViewBag.Message = config.CompletedCourseMessage;
            
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
