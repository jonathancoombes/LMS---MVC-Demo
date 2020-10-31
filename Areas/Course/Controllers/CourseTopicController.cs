using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Models.Course;
using LMS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Utility;
using LMS.Extentions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using LMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using LMS.Models.Learning;

namespace LMS.Areas.Course.Controllers
{
    [Area("Course")]
    [Authorize(Roles = "Administrator,Designer")]
    public class CourseTopicController : Controller
    {
        private ApplicationDbContext _db;
        private IHostingEnvironment _hostingEnvironment;

        private UserManager<ApplicationUser> _userManager;

        private string _apiKey { get; set; }
        private string _privateKeyFile { get; set; }
        private bool _scopeUser { get; set; }

        public CourseTopicController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;

            var opts = config.GetSection("TinyDrive");
            _apiKey = opts["apiKey"];
            _privateKeyFile = opts["privateKeyFile"];
            _scopeUser = Boolean.Parse(opts["scopeUser"]);
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


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



        // GET - CourseTopics
        public async Task<IActionResult> GetCourseTopics(int id)
        {

            List<CourseTopic> courseTopics = new List<CourseTopic>();

            var courseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == id);

            if (courseUnit.CourseTopicIds != null && courseUnit.CourseTopicIds != "")
            {
                var topicIds = OrderList.ItemIdOrder(courseUnit.CourseTopicIds);

                foreach (var item in topicIds)
                {
                    courseTopics.AddRange(_db.CourseTopics.Where(p => p.Id == item).ToList());
                };
            }
                return Json(new SelectList(courseTopics, "Id", "Name"));

        }




        //GET
        public IActionResult Index(int? id)
        {
            return RedirectToAction("TopicList", "CourseTopic", new { id = id });
        }


        //GET
        public async Task<IActionResult> TopicList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _db.CourseUnits.SingleOrDefaultAsync(c=>c.Id == id);

            CourseUnitTopicViewModel model = new CourseUnitTopicViewModel
            {
                CourseUnit = unit,
                CourseTopic = new CourseTopic(),
                CourseTopicList = new List<CourseTopic>(),
                CanDelete = CanDeleteUnitOrChild.CanDel(unit, _db)
            };
                 
    

            if (model.CourseUnit.CourseTopicIds != null && model.CourseUnit.CourseTopicIds != "")
            {
                var topicIds = OrderList.ItemIdOrder(model.CourseUnit.CourseTopicIds);

                var topicsList = new List<CourseTopic>();

                foreach (var item in topicIds)
                {
                   topicsList.AddRange(_db.CourseTopics.Where(p => p.Id == item).ToList());
                };

                model.CourseTopicList = topicsList;

               
            }

            return View(model);
        }


        //GET - Create
        public async Task<IActionResult> Create(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

           
            CourseUnitTopicViewModel model = new CourseUnitTopicViewModel
            {
                CourseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == id),
                CourseTopic = new CourseTopic(),
                ContentType = new List<string>() { SD.ContentCustom, SD.ContentPDF }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseUnitTopicViewModel courseTopicVM, int Id)
        {

            if (ModelState.IsValid)
            {

                //Check to see if there is allready a topic with the same name

                var TopicExists = _db.CourseTopics.Where(s => s.Name == courseTopicVM.CourseTopic.Name);

                // If it does, oops:

                if (TopicExists.Count() > 0)
                {
                    ModelState.AddModelError("Name", "Another Topic with the same name allready exist in this Unit. Please choose another name.");
                }

                //Otherwise, all is well so let's go:

                else
                {


                    courseTopicVM.CourseTopic.CourseUnitId = Id;

                    //1 - Adding the new Topic to DB

                    _db.CourseTopics.Add(courseTopicVM.CourseTopic);

                    await _db.SaveChangesAsync();


                    //If the ContentType is a PDF

                    if (courseTopicVM.CourseTopic.ContentType == SD.ContentPDF) {

                        //Processing File

                        string webRootPath = _hostingEnvironment.WebRootPath;

                        var files = HttpContext.Request.Form.Files;

                        var topicFromDb = await _db.CourseTopics.FindAsync(courseTopicVM.CourseTopic.Id);

                        if (files.Count() > 0)
                        {
                            //Files was uploaded

                            var uploads = Path.Combine(webRootPath, "topicfiles");
                            var extention = Path.GetExtension(files[0].FileName);

                            using (var filesStream = new FileStream(Path.Combine(uploads, courseTopicVM.CourseTopic.Id + extention), FileMode.Create))
                            {
                                files[0].CopyTo(filesStream);

                            }

                            topicFromDb.PDFContent = @"\topicfiles\" + courseTopicVM.CourseTopic.Id + extention;

                            _db.Update(topicFromDb);
                        }
                        else
                        {

                            ModelState.AddModelError("PDFContent", "Please select a PDF file for your topic.");

                        }

                        
                    

                    }
                    

                    //2 - Update Topic Order in Unit Table

                    int topicId = courseTopicVM.CourseTopic.Id;

                    var currentUnit = await _db.CourseUnits.Where(c => c.Id == Id).SingleOrDefaultAsync();

                    var currentTopicOrder = currentUnit.CourseTopicIds;

                    if (currentTopicOrder == null || currentTopicOrder == "")
                    {

                        currentUnit.CourseTopicIds = topicId.ToString();


                        _db.Update(currentUnit);
                        await _db.SaveChangesAsync();

                    }
                    else if (currentTopicOrder != null)
                    {

                        var finalOrder = String.Concat(currentTopicOrder + "," + topicId.ToString());
                        currentUnit.CourseTopicIds = finalOrder;

                        _db.Update(currentUnit);

                        await _db.SaveChangesAsync();


                    }
                    return RedirectToAction("TopicList", "CourseTopic", new { id = Id });

                }

            }
            //If the ModelState is not valid

            CourseUnitTopicViewModel modelVM = new CourseUnitTopicViewModel
            {
                CourseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == Id),
                CourseTopic = courseTopicVM.CourseTopic,
                ContentType = new List<string>() { SD.ContentCustom, SD.ContentPDF }
            };

            return View(modelVM);

        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var topic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == id);

                if (topic == null)
                {

                    return NotFound();
                }
                else
                {
                                       
                    CourseUnitTopicViewModel courseTopicVM = new CourseUnitTopicViewModel
                    {
                        CourseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == topic.CourseUnitId),
                        CourseTopic = topic,
                        ContentType = new List<string>() { SD.ContentCustom, SD.ContentPDF }
                    };
                    return View(courseTopicVM);
                }
            }
        }

        // !!!! Check note about hiddenfield requirement. Must set the value of the courseUnitId




        //EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseTopic courseTopic, int unitId, int topicId, string contentType, string faOrder)
        {

            //Check if topic name allready exists
            
            var TopicExists = _db.CourseTopics.Where(s => s.Name == courseTopic.Name).Where(s => s.Id != topicId).Count();
            courseTopic.FAOrder = faOrder;
            courseTopic.Id = topicId;
            courseTopic.CourseUnitId = unitId;

            courseTopic.ContentType = contentType;
            // If it does, oops:

            if (TopicExists > 0)
            {
                ModelState.AddModelError("Name", "Another Topic with the same name allready exist in this Unit. Please choose another name.");
                CourseUnitTopicViewModel courseTopicVM = new CourseUnitTopicViewModel
                {
                    CourseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == courseTopic.CourseUnitId),
                    CourseTopic = courseTopic
                };
                return View(courseTopicVM);         
            }

            else if (!ModelState.IsValid)
            {

                CourseUnitTopicViewModel courseTopicVM = new CourseUnitTopicViewModel
                {
                    CourseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == courseTopic.CourseUnitId),
                    CourseTopic = courseTopic
                };
                return View(courseTopicVM);

            }

            if (courseTopic.ContentType == SD.ContentPDF)
            {

                //Processing File

                string webRootPath = _hostingEnvironment.WebRootPath;

                var files = HttpContext.Request.Form.Files;

                if (files.Count() > 0)
                {
                    //Files was uploaded

                    var uploads = Path.Combine(webRootPath, "topicfiles");
                    var extention = Path.GetExtension(files[0].FileName);

                    // Check if exists then delete

                    var filePath = Path.Combine(uploads, courseTopic.Id + extention);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    using (var filesStream = new FileStream(Path.Combine(uploads, courseTopic.Id + extention), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);

                    }

                }

                courseTopic.PDFContent = @"\topicfiles\" + courseTopic.Id + ".pdf";

            }
  
            _db.Update(courseTopic);

            await _db.SaveChangesAsync();

            return RedirectToAction("TopicList", "CourseTopic", new { id = unitId });

        }


        //GET - DELETE

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var topic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == id);

            if (topic == null)
            {
                return NotFound();
            }

            return View(topic);

        }


        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentTopic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == id);

            var unitId = currentTopic.CourseUnitId;
            var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == unitId);

            var currentTopicOrder = currentUnit.CourseTopicIds;

            if (currentTopicOrder == null || currentTopicOrder == "" || (!OrderList.ItemIdOrder(currentTopicOrder).Contains(id)))
            {
                return NotFound();
            }

            currentUnit.CourseTopicIds = RePosition.Delete(currentTopicOrder, id);

            _db.CourseTopics.Remove(currentTopic);
            _db.Update(currentUnit);


            // IF PDF Topic, Delete topicFile 

            if (currentTopic.ContentType == SD.ContentPDF) {

                string webRootPath = _hostingEnvironment.WebRootPath;

                var topicPath = Path.Combine(webRootPath, currentTopic.PDFContent.TrimStart('\\'));

                // Check if exists then delete

                if (System.IO.File.Exists(topicPath))
                {
                    System.IO.File.Delete(topicPath);
                }

            }

            await _db.SaveChangesAsync();

            return RedirectToAction("TopicList", "CourseTopic", new { id = unitId });


        }


        public async Task<IActionResult> MoveTopicUp(int Id)
        {
            var currentTopic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == Id);

            var unitId = currentTopic.CourseUnitId;
            var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == unitId);

            var currentTopicOrder = currentUnit.CourseTopicIds;

            if (RePosition.CanMoveUpCheck(currentTopicOrder, Id))
            {
                currentUnit.CourseTopicIds = RePosition.MoveUp(currentTopicOrder, Id);
                _db.Update(currentUnit);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("TopicList", "CourseTopic", new { id = unitId });
        }

        public async Task<IActionResult> MoveTopicDown(int Id)
        {
            var currentTopic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == Id);

            var unitId = currentTopic.CourseUnitId;
            var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == unitId);

            var currentTopicOrder = currentUnit.CourseTopicIds;

            if (RePosition.CanMoveDownCheck(currentTopicOrder, Id))
            {
                currentUnit.CourseTopicIds = RePosition.MoveDown(currentTopicOrder, Id);
                _db.Update(currentUnit);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("TopicList", "CourseTopic", new { id = unitId });
        }


    }
}