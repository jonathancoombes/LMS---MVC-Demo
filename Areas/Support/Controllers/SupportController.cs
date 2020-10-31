using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Models;
using LMS.Models.Support;
using LMS.Models.ViewModels;
using LMS.Utility;
using LMS.LMSConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace LMS.Areas.Support.Controllers
{
    [Area("Support")]
    public class SupportController : Controller
    {

        private readonly ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification;

        public SupportController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IToastNotification toastNotification)
        {
            _db = db;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        
        [Authorize(Roles = "Administrator,Facilitator,Assessor")]
        [Route("support/classlist")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ClassList()
        {

            SupportViewModel supportVM = new SupportViewModel()
            {
                Classes = new List<Class>(),
                Courses = new List<Models.Course.Course>(),
                Class = new Class(),
                SupportRequestList = await _db.SupportRequests.ToListAsync()
            };

            supportVM.Classes = _db.Classes.ToList();
            
            var currentUser = await GetCurrentUserAsync();
            var config = await _db.ConfigOptions.SingleOrDefaultAsync();
            var classEnrolments = new List<ClassEnrolment>(); 

            if (config.AssessorCanSupport == false && config.FacilitatorCanSupport == false) {
                classEnrolments = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).Where(p => p.UserRole != SD.Assessor && p.UserRole != SD.Facilitator).ToListAsync(); 
            }
            if (config.FacilitatorCanSupport == false && config.AssessorCanSupport == true)
            {
                classEnrolments = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).Where(p => p.UserRole != SD.Facilitator && p.UserRole == SD.Assessor).ToListAsync();
            }
            if (config.FacilitatorCanSupport == true && config.AssessorCanSupport == false)
            {
                classEnrolments = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).Where(p => p.UserRole == SD.Facilitator && p.UserRole != SD.Assessor).ToListAsync();
            }
            if (config.FacilitatorCanSupport == true && config.AssessorCanSupport == true)
            {
                classEnrolments = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).Where(p => p.UserRole == SD.Facilitator || p.UserRole == SD.Assessor).ToListAsync();
            }

            

            List<Class> userClasses = new List<Class>();
            

                foreach (var enrolItem in classEnrolments) {
                var classe = await _db.Classes.SingleOrDefaultAsync(p=>p.Id == enrolItem.ClassId);
                if (!userClasses.Contains(classe)) {
                    userClasses.Add(classe); 
                }                    
                    }
                

            supportVM.Classes = userClasses;
            supportVM.Courses = _db.Courses.ToList();

            return View(supportVM);
        }


        [Authorize(Roles = "Administrator,Facilitator,Assessor")]
        [Route("support/openrequests")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SupportRequestList(int Id)
        {

            var requestForClass = _db.SupportRequests.Where(p => p.ClassId == Id).Where(p=>p.Status != SD.StatusClosed).ToList();
            var responses = await _db.SupportResponse.ToListAsync();
            SupportViewModel supportVM = new SupportViewModel()
            {
                SupportRequestList = requestForClass,
                SupportRequest =  new SupportRequest(),
                SupportResponseList = responses,
                ClassId = Id
            };

            return View(supportVM);

        }

        [Authorize(Roles = "Administrator,Facilitator,Assessor")]
        [Route("support/closedrequests")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SupportRequestListClosed(int Id)
        {

            var requestForClass = _db.SupportRequests.Where(p => p.ClassId == Id).Where(p =>p.Status == SD.StatusClosed).ToList();
            var responses = await _db.SupportResponse.ToListAsync();
            SupportViewModel supportVM = new SupportViewModel()
            {
                SupportRequestList = requestForClass,
                SupportRequest = new SupportRequest(),
                SupportResponseList = responses,
                ClassId = Id
            };

            return View(supportVM);

        }

        [Route("support/viewrequest")]
        [Authorize(Roles = "Administrator,Facilitator,Assessor,Learner")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ViewRequest(int Id)
        {

            var requestFromDb = await _db.SupportRequests.SingleOrDefaultAsync(p => p.Id == Id);
            var responsesFromDb = await _db.SupportResponse.Where(p => p.SupportRequestId == Id).OrderBy(p=>p.ResponseDate).ToListAsync();

            SupportViewModel supportVM = new SupportViewModel()
            {
                SupportResponseList = responsesFromDb,
                SupportRequest = requestFromDb,
                SupportResponse = new SupportResponse() { ResponseDate = DateTime.Now, SupportRequestId = requestFromDb.Id },
                ApplicationUser = new ApplicationUser(),
                Sender = new ApplicationUser(),
                Statuses = new List<string>() {SD.StatusOpen, SD.StatusPending, SD.StatusClosed }
               
            };
            
            supportVM.ApplicationUser = await _db.ApplicationUser.SingleOrDefaultAsync(p=>p.Id == requestFromDb.UserId);

            return View(supportVM);

        }

        [Route("support/viewrequest")]
        [Authorize(Roles = "Administrator,Facilitator,Assessor,Learner")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewRequest(SupportViewModel supportVM, int reqId, string senderId) {
                        
            if (supportVM.SupportResponse.ResponseBody == null || supportVM.SupportResponse.ResponseBody == "") {

                ModelState.AddModelError("SupportRequest.RequestBody", "The message cannot be empty");
                _toastNotification.AddErrorToastMessage("Message Field cannot be empty!",
                    new ToastrOptions
                    {
                        Title = "Error! No message included..",
                        ToastClass = "toastbox",
                    }
                    );

                var responsesFromDb = await _db.SupportResponse.Where(p => p.SupportRequestId == reqId).OrderBy(p => p.ResponseDate).ToListAsync();
                var requestFromDb = await _db.SupportRequests.SingleOrDefaultAsync(p => p.Id == reqId);

                supportVM = new SupportViewModel()
                {
                    SupportResponseList = responsesFromDb,
                    SupportRequest = requestFromDb,
                    SupportResponse = new SupportResponse() { ResponseDate = DateTime.Now, SupportRequestId = requestFromDb.Id },
                    ApplicationUser = new ApplicationUser(),
                    Sender = new ApplicationUser(),
                    Statuses = new List<string>() { SD.StatusOpen, SD.StatusPending, SD.StatusClosed }

                };

                return View(supportVM);

            }
                var currentReqFromDB = await _db.SupportRequests.SingleOrDefaultAsync(p => p.Id == reqId);

            if (ModelState.IsValid)
            {

                if (supportVM.SupportRequest.Status == SD.StatusClosed && currentReqFromDB.Status != SD.StatusClosed) {

                    currentReqFromDB.Closed = DateTime.Now;
                    _db.Update(currentReqFromDB);
                    await _db.SaveChangesAsync();
                }

                supportVM.SupportResponse.UserId = senderId;
                supportVM.SupportResponse.SupportRequestId = reqId;
                supportVM.SupportResponse.ResponseDate = DateTime.Now;

                _db.Add(supportVM.SupportResponse);
                await _db.SaveChangesAsync();


                if (currentReqFromDB.ResponseIds == null || currentReqFromDB.ResponseIds == "")
                {

                    currentReqFromDB.ResponseIds = supportVM.SupportResponse.Id.ToString();

                    currentReqFromDB.Status = supportVM.SupportRequest.Status;
                    _db.Update(currentReqFromDB);
                    await _db.SaveChangesAsync();

                }
                else if (currentReqFromDB.ResponseIds != null)
                {

                    var finalOrder = String.Concat(currentReqFromDB.ResponseIds + "," + supportVM.SupportResponse.Id.ToString());
                    currentReqFromDB.ResponseIds = finalOrder;
                    currentReqFromDB.Status = supportVM.SupportRequest.Status;
                    _db.Update(currentReqFromDB);

                    await _db.SaveChangesAsync();
                }
            }

            if (User.IsInRole(SD.Learner))
            { return RedirectToAction("MySupportOpen", "Support"); }
            else { return RedirectToAction("SupportRequestList", "Support", new { Id = currentReqFromDB.ClassId }); }

            } 






        [Authorize(Roles = "Administrator,Learner")]
        [Route("support/request")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SubmitRequest()
        {

            var currentUser = await GetCurrentUserAsync();

            var userClasses = await _db.ClassEnrolments.Include(p => p.Class.Course).Where(p => p.UserId == currentUser.Id).ToListAsync();

            if (userClasses.Count() > 0)
            {

                Dictionary<int?, string> classes = new Dictionary<int?, string>();

                var i = 1;
                foreach (var item in userClasses.OrderBy(p => p.Class.Name))
                {                   
                    if (item.Class.Course.Name.Length > 30 && item.Class.Name.Length > 20) {
                       item.Class.Name = i++ + ". " + item.Class.Course.Name.Substring(0, 30) + ".." + " || Class: " + item.Class.Name.Substring(0, 20) + "..";
                    }
                    else if (item.Class.Course.Name.Length > 30 && item.Class.Name.Length < 20)
                    {
                        item.Class.Name = i++ + ". " + item.Class.Course.Name.Substring(0, 30) + ".." + " || Class: " + item.Class.Name;
                    }
                    if (item.Class.Course.Name.Length < 30 && item.Class.Name.Length < 20)
                    {
                        item.Class.Name = i++ + ". " + item.Class.Course.Name + ".." + " || Class: " + item.Class.Name.Substring(0, 20) + "..";
                    }
                    else {
                        item.Class.Name = i++ + ". " + item.Class.Course.Name + ".." + "|| Class: " + item.Class.Name.Substring(0, 20) + "..";
                    }
                    
                    classes.Add(item.ClassId, item.Class.Name);

                }

                SelectList userClassSelect = new SelectList(
                classes.Select(x => new { Value = x.Key, Text = x.Value }),
                "Value",
                "Text"
                );

                SupportViewModel supportVM = new SupportViewModel()
                {
                    UserClasses = userClassSelect,
                    SupportRequest = new SupportRequest(),
                    ApplicationUser = currentUser
                };

                return View(supportVM);
            }

            return Content("You are not enrolled in any classes. Support is not available.");

        }

        [Route("support/request")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRequest(SupportViewModel supportVM) {

            if (!ModelState.IsValid) {
                _toastNotification.AddErrorToastMessage("Please complete all fields",
                    new ToastrOptions
                    {
                        Title = "Error!",
                        ToastClass = "toastbox",
                    }
                    );
               
                return View(supportVM);
            }

            supportVM.SupportRequest.UserId = supportVM.ApplicationUser.Id;

            _db.SupportRequests.Add(supportVM.SupportRequest);

            await _db.SaveChangesAsync();

            return RedirectToAction("MySupportOpen", "Support");
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> MySupportClosed()
        {
            var currentUser = await GetCurrentUserAsync();

            var requestFromDb = await _db.SupportRequests.Where(p => p.UserId == currentUser.Id).Where(p => p.Status == SD.StatusClosed).OrderByDescending(p => p.Open).ToListAsync();
            var responsesFromDb = await _db.SupportResponse.ToListAsync();

            List<List<SupportResponse>> responsesToRequests = new List<List<SupportResponse>>();

            foreach (var request in requestFromDb)
            {

                var responsesPerRequest = await _db.SupportResponse.Where(p => p.SupportRequestId == request.Id).ToListAsync();
                responsesToRequests.Add(responsesPerRequest);
            }


            SupportViewModel supportVM = new SupportViewModel()
            {
                SupportRequestList = requestFromDb,
                SupportResponseListofLists = responsesToRequests

            };

            return View(supportVM);

        }



        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> MySupportOpen()
        {
            var currentUser = await GetCurrentUserAsync();

            var requestFromDb = await _db.SupportRequests.Where(p => p.UserId == currentUser.Id).Where(p=>p.Status == SD.StatusOpen).OrderByDescending(p => p.Open).ToListAsync();
            var responsesFromDb = await _db.SupportResponse.ToListAsync();

            List<List<SupportResponse>> responsesToRequests = new List<List<SupportResponse>>();

            foreach (var request in requestFromDb)
            {

                var responsesPerRequest = await _db.SupportResponse.Where(p => p.SupportRequestId == request.Id).ToListAsync();
                responsesToRequests.Add(responsesPerRequest);
            }


            SupportViewModel supportVM = new SupportViewModel()
            {
                SupportRequestList = requestFromDb,
                SupportResponseListofLists = responsesToRequests

            };

            return View(supportVM);

        }









        public IActionResult Index()
        {
            return View();
        }
    }
}