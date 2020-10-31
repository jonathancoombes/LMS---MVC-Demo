using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Extentions;
using LMS.Models;
using LMS.Models.Course;
using LMS.Models.Learning;
using LMS.Models.ViewModels;
using LMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Areas.Learning.Controllers
{
    [Area("Learning")]
    [Authorize(Roles = "Administrator,Learner")]
    public class CourseOutlineController : Controller
    {

        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        public CourseOutlineController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _db = db;

        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);



        //GET
        public async Task<IActionResult> ContentOutline()
        {
            var currentUser = await GetCurrentUserAsync();

            if (currentUser.ActiveClassId == null || currentUser.ActiveClassId == 0)
            {
                return NotFound("The user has no active class set!");
            }
            else
            {
                var classEnrol = await _db.ClassEnrolments.Include(p => p.Class).Where(p => p.UserId == currentUser.Id).Where(p => p.Id == currentUser.ActiveClassId).SingleOrDefaultAsync();


                CourseUnitViewModel model = new CourseUnitViewModel
                {
                    Course = await _db.Courses.SingleOrDefaultAsync(c => c.Id == classEnrol.Class.CourseId),
                    CourseUnit = new CourseUnit(),
                    CourseUnitAssignments = await _db.CourseUnitAssignments.Where(c => c.CourseId == classEnrol.Class.CourseId).ToListAsync(),
                    ClassEnrol = classEnrol,
                    SummSubs = await _db.SummativeSessions.Where(p=>p.ClassEnrolId == currentUser.ActiveClassId).Where(p=>p.UserId == currentUser.Id).ToListAsync()

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

                    foreach (var mod in orderedModuleList)
                    {
                        if (mod.CourseUnitOrder != "" && mod.CourseUnitOrder != null)
                        {
                            foreach (var unitId in OrderList.ItemIdOrder(mod.CourseUnitOrder))
                            {

                                selectedUnit = model.CourseUnitList.Where(p => p.Id == unitId).FirstOrDefault();

                                if (selectedUnit != null)
                                {
                                    orderedUnitList.Add(selectedUnit);
                                    
                                }

                            }
                        }
                    }
                    model.CourseUnitList = orderedUnitList;

                }


                return View(model);
            }
        }
                                         

        //GET
        public async Task<IActionResult> TopicListOutline(int Id)
        {
            var currentUser = await GetCurrentUserAsync();

            if (currentUser.ActiveClassId == null || currentUser.ActiveClassId == 0)
            {
                return NotFound("The user has no active class set!");
            }
            else
            {
                var classEnrol = await _db.ClassEnrolments.Include(p => p.Class).Where(p => p.UserId == currentUser.Id).Where(p => p.Id == currentUser.ActiveClassId).SingleOrDefaultAsync();

                CourseUnitTopicViewModel model = new CourseUnitTopicViewModel
                {
                    CourseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == Id),
                    CourseTopic = new CourseTopic(),
                    CourseTopicList = new List<CourseTopic>()

                };

                if (model.CourseUnit.CourseTopicIds != null && model.CourseUnit.CourseTopicIds != "")
                {
                    var topicIds = OrderList.ItemIdOrder(model.CourseUnit.CourseTopicIds);

                    var topicsList = new List<CourseTopic>();
                    var formativeSessionList = new List<FormativeSession>();
                                       
                   classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).Include(p => p.Class).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);

                    foreach (var item in topicIds)
                    {
                        topicsList.AddRange(_db.CourseTopics.Where(p => p.Id == item).ToList());
                        var session = await _db.FormativeSessions.Where(p => p.TopicId == item).Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.ClassEnrolId == currentUser.ActiveClassId);
                        formativeSessionList.Add(session);
                    };

                    model.CourseTopicList = topicsList;
                    model.FormativeSessions = formativeSessionList;

                }
           

            return View(model);
        } }



        public async Task<IActionResult> TopicViewer(int Id)
        {

            var currentUser = await GetCurrentUserAsync();
            var topic = await _db.CourseTopics.Include(p => p.CourseUnit).SingleOrDefaultAsync(p => p.Id == Id);


            //Geting the unit that the topic
            var unitAssignment = await _db.CourseUnitAssignments.Where(p => p.CourseUnitId == topic.CourseUnitId).FirstOrDefaultAsync();


            //Get enrolments of user of a specific classId
            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).Include(p => p.Class).Where(p => p.Class.CourseId == unitAssignment.CourseId).SingleOrDefaultAsync();

            if (classEnrol.Class.CourseId == unitAssignment.CourseId)
            {

                ViewBag.topic = classEnrol.CurrentTopicId;
                ViewBag.completed = classEnrol.CompletedTopics;

                return View(topic);


            }

            else
            {

                return NotFound("Topic Display not Possible. Learner not allowed to view topic");
            }
                                 
        }




    }
}