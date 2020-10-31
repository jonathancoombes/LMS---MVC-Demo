using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Extentions;
using LMS.Models;
using LMS.Utility;
using LMS.Models.Course;
using LMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LMS.Areas.Learning.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LearningSessionController : ControllerBase
    {

        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        public LearningSessionController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _db = db;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        //1. GET api/learningsession
        // Returns the current topic page number from db

        [HttpGet]

        public async Task<IActionResult> GetPage(int Id)
        {

            var currentUser = await GetCurrentUserAsync();
            var classEnrol = await _db.ClassEnrolments.Where(p=>p.UserId == currentUser.Id).Where(p => p.Id == Id).SingleOrDefaultAsync();

            if (classEnrol.CurrentPage == null || classEnrol.CurrentPage == 0) {

                return NotFound();

            }
      
            return Ok(classEnrol.CurrentPage);
          

        }

        //2. POST api/learningSession
        // Returns the current topic page number from db
        [HttpPost("{Id:int}/{page:int}")]
        public async Task<IActionResult> SetPage(int Id, int? page)
        {
           
            var currentUser = await GetCurrentUserAsync();
            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == Id);

            classEnrol.CurrentPage = page;

            _db.Update(classEnrol);
            await _db.SaveChangesAsync();

            return Ok();


        }




        //2. POST api/learningSession/completedtopic/
        // Adds the completed topic Id to the enrolment table in db
        [HttpPost("{classId:int}/{topicId:int}/{isLastTopic:bool}")]
        public async Task<IActionResult> CompletedTopic(int classId, int topicId, bool isLastTopic)
        {

            var currentUser = await GetCurrentUserAsync();
            var classEnrol = await _db.ClassEnrolments.Include(p => p.Class.Course).Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == classId);

            if (currentUser != null && classEnrol != null)
            {
                               
                if (classEnrol.CompletedTopics != null && classEnrol.CompletedTopics != "")
                {
                    var topicList = OrderList.ItemIdOrder(classEnrol.CompletedTopics);

                    if (!topicList.Contains(topicId))
                    {  
                        //Adding Topic to completed Topic List
                    var finalOrder = String.Concat(classEnrol.CompletedTopics + "," + topicId.ToString());
                    classEnrol.CompletedTopics = finalOrder;
                        classEnrol.CurrentPage = 1;
                        _db.Update(classEnrol);
                        await _db.SaveChangesAsync();
                    }                
                }
                else
                {

                        classEnrol.CompletedTopics = topicId.ToString();
                    classEnrol.CurrentPage = 1;
                    _db.Update(classEnrol);
                    await _db.SaveChangesAsync();
                }

                //Setting Current Topic
                var topic = await _db.CourseTopics.Include(p => p.CourseUnit).SingleOrDefaultAsync(p => p.Id == topicId);
                var orderedTopics = OrderList.ItemIdOrder(topic.CourseUnit.CourseTopicIds);

                var currentPosition = orderedTopics.BinarySearch(topic.Id);

                if (currentPosition + 1 != orderedTopics.Count())
                {
                    classEnrol.CurrentTopicId = orderedTopics[currentPosition + 1];

                    _db.Update(classEnrol);
                    await _db.SaveChangesAsync();

                }
                //ELSE, THIS WAS THE LAST TOPIC
                else
                {

                    var units = "";
                    var unitList = new List<int>();
                    var moduleList = new List<int>();

                    if (classEnrol == null)
                    {

                        return NotFound("User Not Enrolled in a Course");

                    }
                    var currentCourse = await _db.Courses.SingleOrDefaultAsync(p => p.Id == classEnrol.Class.CourseId);

                    //from current course -> first module
                    if (currentCourse.CourseModuleOrder != null && currentCourse.CourseModuleOrder != "")
                    {
                        moduleList = OrderList.ItemIdOrder(currentCourse.CourseModuleOrder);

                        foreach (var mod in moduleList)
                        {

                            var module = await _db.CourseModules.SingleOrDefaultAsync(p=>p.Id == mod);

                            if (units == "" && module.CourseUnitOrder != null && module.CourseUnitOrder != "")
                            {
                                units = module.CourseUnitOrder;
                            }
                            else if(units != "" && (module.CourseUnitOrder != null && module.CourseUnitOrder != ""))
                            { units =  units + "," + module.CourseUnitOrder; }
                            
                        }

                        if (units != null && units != "")
                        {

                            var unitIds = OrderList.ItemIdOrder(units);

                            var currentUnitIndex = unitIds.FindIndex(p => p == topic.CourseUnitId);
                            //var currentUnitIndex = unitIds.BinarySearch(topic.CourseUnitId);

                            var nextUnit = await _db.CourseUnits.SingleOrDefaultAsync(p => p.Id == unitIds[currentUnitIndex + 1]);


                            if (isLastTopic)
                            {
                                var completedUnitList = new List<int>();

                                //IF There are comepleted Units listed in column
                                if (classEnrol.CompletedUnits != null && classEnrol.CompletedUnits != "")
                                {
                                    completedUnitList = OrderList.ItemIdOrder(classEnrol.CompletedUnits);
                                    if (!completedUnitList.Contains(topic.CourseUnitId))
                                    {
                                        //Adding Unit to completed Unit List
                                        var finalOrder = String.Concat(classEnrol.CompletedUnits + "," + topic.CourseUnitId.ToString());
                                        classEnrol.CompletedUnits = finalOrder;
                                    }
                                }

                                //If this is the first completed unit in column
                                else if (classEnrol.CompletedUnits == null || classEnrol.CompletedUnits != "")
                                {
                                    classEnrol.CompletedUnits = topic.CourseUnitId.ToString();
                                }
                                //Setting CurrentTopic to first topic of next unit
                                if (nextUnit != null && nextUnit.CourseTopicIds != null && nextUnit.CourseTopicIds != "")
                                {
                                    var nextUnitTopics = OrderList.ItemIdOrder(nextUnit.CourseTopicIds);
                                    classEnrol.CurrentTopicId = nextUnitTopics[0];
                                    classEnrol.CurrentPage = 1;

                                }

                                _db.Update(classEnrol);
                                await _db.SaveChangesAsync();

                                //If this is the last unit that is completed - ALL Topics Completed
                                if (topic.CourseUnitId == unitIds.ElementAt(unitIds.Count() - 1)
                                    || (nextUnit.CourseTopicIds == null || nextUnit.CourseTopicIds == "")) 
                                    {

                                        classEnrol.CompletedTopics = SD.AllComplete;
                                        _db.Update(classEnrol);
                                        await _db.SaveChangesAsync();

                                        return RedirectToAction("CourseCompleted", "Home");
                                    }

                              
                            }
                        }                
                    }

                    _db.Update(classEnrol);
                    await _db.SaveChangesAsync();

                }                           
                return Ok();                         
            }           
            return NotFound("The Completed Topic was not updated in the DB");
        }


        //3. POST api/learningSession/completedFA/
        // Adds the completed FA Id to the enrolment table in db
        [HttpPost("{classId:int}/{formativeId:int}")]
        public async Task<IActionResult> CompletedFA(int classId, int formativeId)
        {

            var currentUser = await GetCurrentUserAsync();
            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == classId);

            if (currentUser != null && classEnrol != null)
            {

                if (classEnrol.CompletedFas != null && classEnrol.CompletedFas != "")
                {
                    var faList = OrderList.ItemIdOrder(classEnrol.CompletedFas);

                    if (!faList.Contains(formativeId))
                    {
                        //Adding FA to completed FA List
                        var finalOrder = String.Concat(classEnrol.CompletedFas + "," + formativeId.ToString());
                        classEnrol.CompletedFas = finalOrder;
                       
                    }

                }
                else
                {

                    classEnrol.CompletedFas = formativeId.ToString();
             
                }
           
                _db.Update(classEnrol);
                await _db.SaveChangesAsync();

                return Ok();

            }

            return NotFound("The Completed Formative Assessment was not updated in the DB");

        }
                          

        //3. POST api/learningSession/completedSA/
        // Adds the completed SA Id to the enrolment table in db
        [HttpPost("{classId:int}/{summativeId:int}")]
        public async Task<IActionResult> CompletedSA(int classId, int summativeId)
        {

            var currentUser = await GetCurrentUserAsync();
            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == classId);

            if (currentUser != null && classEnrol != null)
            {

                if (classEnrol.CompeletedSas != null && classEnrol.CompeletedSas != "")
                {
                    var saList = OrderList.ItemIdOrder(classEnrol.CompeletedSas);

                    if (!saList.Contains(summativeId))
                    {
                        //Adding SA to completed SA List
                        var finalOrder = String.Concat(classEnrol.CompeletedSas + "," + summativeId.ToString());
                        classEnrol.CompeletedSas = finalOrder;

                    }

                }
                else
                {

                    classEnrol.CompeletedSas = summativeId.ToString();

                }

                _db.Update(classEnrol);
                await _db.SaveChangesAsync();

                return Ok();

            }

            return NotFound("The Completed Summative Assessment was not updated in the DB");

        }



        //2. POST api/learningsession/valclose
       
        [HttpPost("{valId:int}/{classId:int}/{closeDate:DateTime}")]
        public async Task<IActionResult> ValClose(int valId, int classId, DateTime closeDate)
        {

            var currentUser = await GetCurrentUserAsync();

            var valFromDb = await _db.SubmissionValidities
                  .Include(p => p.Class)
                  .Where(p => p.ClassId == classId)
                  .SingleOrDefaultAsync(p => p.Id == valId);

            var classe = await _db.Classes.SingleOrDefaultAsync(p => p.Id == classId);

            //string dateInput = closeDate;
            //DateTime parsedDate = DateTime.Parse(dateInput);

            valFromDb.Close = closeDate;

            _db.Update(valFromDb);
                       
            await _db.SaveChangesAsync();

            return Ok();


        }

        //2. POST api/learningsession/valclose

        [HttpPost("{valId:int}/{classId:int}/{openDate:DateTime}")]
        public async Task<IActionResult> ValOpen(int valId, int classId, DateTime openDate)
        {

            var currentUser = await GetCurrentUserAsync();

            var valFromDb = await _db.SubmissionValidities
                  .Include(p => p.Class)
                  .Where(p => p.ClassId == classId)
                  .SingleOrDefaultAsync(p => p.Id == valId);

            var classe = await _db.Classes.SingleOrDefaultAsync(p => p.Id == classId);

            //string dateInput = openDate;
            //DateTime parsedDate = DateTime.Parse(dateInput);

            valFromDb.Close = openDate;

            _db.Update(valFromDb);

            await _db.SaveChangesAsync();

            return Ok();


        }


    }
}