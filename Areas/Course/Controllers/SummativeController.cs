using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Extentions;
using LMS.Models;
using LMS.Models.Assessment;
using LMS.Models.ViewModels;
using LMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NToastNotify;

namespace LMS.Areas.Course.Controllers
{
    [Area("Course")]
    //[Authorize(Roles = "Administrator,Designer")]
    public class SummativeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IToastNotification _toastNotification;
        private UserManager<ApplicationUser> _userManager;
        private string _apiKey { get; set; }
        private string _privateKeyFile { get; set; }
        private bool _scopeUser { get; set; }

        public SummativeController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _hostingEnvironment = hostingEnvironment;
            _db = db;
            _toastNotification = toastNotification;
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




        //GET
        public async Task<IActionResult> SummativeList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SummativeViewModel model = new SummativeViewModel()
            {
                CourseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == id),
                SummativeList = new List<Summative>(),
                Summative = new Summative(),
                MultipleChoiceList = new List<MultipleChoice>(),
                TrueFalseList = new List<TrueFalse>(),
                AssignmentList = new List<Assignment>(),
                DirectQuestionList = new List<DirectQuestion>(),
                PracticalList = new List<Practical>(),
                AssessmentType = new List<string>() { SD.DirectQuestion, SD.TrueFalse, SD.MultipleChoice, SD.Assignment, SD.Practical }

            };
    
            model.CanDelete = CanDeleteUnitOrChild.CanDel(model.CourseUnit, _db);

            //Preparing a List of Formative Assessments In Order

            if (model.CourseUnit.SAOrder != null && model.CourseUnit.SAOrder != "")
            {
                var summativeIds = OrderList.ItemIdOrder(model.CourseUnit.SAOrder);

                var summativeList = new List<Summative>();

                foreach (var item in summativeIds)
                {
                    summativeList.AddRange(_db.Summatives
                        .Include(p => p.MultipleChoice)
                        .Include(p => p.TrueFalse)
                        .Include(p => p.Practical)
                        .Include(p => p.DirectQuestion)
                        .Include(p => p.Assignment)
                        .Where(p => p.Id == item).ToList());
                };

                model.SummativeList = summativeList;

            }

            return View(model);
        }

        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {

                return NotFound();

            }

            SummativeViewModel model = new SummativeViewModel
            {
                CourseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == id),
                SummativeList = new List<Summative>(),
                Summative = new Summative(),
                MultipleChoiceList = new List<MultipleChoice>(),
                TrueFalseList = new List<TrueFalse>(),
                AssignmentList = new List<Assignment>(),
                DirectQuestionList = new List<DirectQuestion>(),
                PracticalList = new List<Practical>(),
                SubmissionValidity = new SubmissionValidity(),
                AssessmentType = new List<string>() { SD.DirectQuestion, SD.TrueFalse, SD.MultipleChoice, SD.Assignment, SD.Practical }

            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SummativeViewModel summativeVM, int unitId)
        {
            var currentUser = await GetCurrentUserAsync();


            if (ModelState.IsValid)
            {
                //Check to see if there is allready a summative with the same name
                var summativeExists = _db.Summatives.Where(s =>
                    s.Title == summativeVM.Summative.Title &&
                    s.CourseUnitId == unitId);

                // If it does, oops:

                if (summativeExists.Count() > 0)
                {
                    _toastNotification.AddErrorToastMessage("Please choose another name!");
                    ModelState.AddModelError("Name", "Another Summative with the same name allready exist in this Unit. Please choose another name.");
                    SummativeViewModel model = new SummativeViewModel
                    {
                        CourseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == unitId),
                        Summative = summativeVM.Summative,
                        SummativeList = new List<Summative>(),
                        MultipleChoiceList = new List<MultipleChoice>(),
                        TrueFalseList = new List<TrueFalse>(),
                        AssignmentList = new List<Assignment>(),
                        DirectQuestionList = new List<DirectQuestion>(),
                        PracticalList = new List<Practical>(),
                        AssessmentType = new List<string>() { SD.DirectQuestion, SD.TrueFalse, SD.MultipleChoice, SD.Assignment, SD.Practical }
                    };
                    return View(model);
                }

                //Otherwise, all is well so let's go:

                else
                {
                    summativeVM.Summative.CourseUnitId = unitId;
                    

                    if (summativeVM.Summative.AssessmentType == SD.TrueFalse)
                    {
                        summativeVM.Summative.MultipleChoice = null;
                        summativeVM.Summative.Assignment = null;
                        summativeVM.Summative.DirectQuestion = null;
                        summativeVM.Summative.Practical = null;
                    }
                    else if (summativeVM.Summative.AssessmentType == SD.MultipleChoice)
                    {
                        summativeVM.Summative.Assignment = null;
                        summativeVM.Summative.DirectQuestion = null;
                        summativeVM.Summative.Practical = null;
                        summativeVM.Summative.TrueFalse = null;
                    }
                    if (summativeVM.Summative.AssessmentType == SD.Assignment)
                    {
                        summativeVM.Summative.MultipleChoice = null;
                        summativeVM.Summative.TrueFalse = null;
                        summativeVM.Summative.DirectQuestion = null;
                        summativeVM.Summative.Practical = null;
                      
                    }
                    else if (summativeVM.Summative.AssessmentType == SD.Practical)
                    {
                        summativeVM.Summative.Assignment = null;
                        summativeVM.Summative.DirectQuestion = null;
                        summativeVM.Summative.MultipleChoice = null;
                        summativeVM.Summative.TrueFalse = null;
                    }
                    if (summativeVM.Summative.AssessmentType == SD.DirectQuestion)
                    {
                        summativeVM.Summative.MultipleChoice = null;
                        summativeVM.Summative.TrueFalse = null;
                        summativeVM.Summative.Assignment = null;
                        summativeVM.Summative.Practical = null;
                    }

                    //1 - Adding the new Summative to DB

                    _db.Summatives.Add(summativeVM.Summative);
                    await _db.SaveChangesAsync();

                    if (summativeVM.Summative.AssessmentType == SD.Practical) {

                    summativeVM.Summative.Practical.SummId = summativeVM.Summative.Id;

                        _db.Update(summativeVM.Summative.Practical);
                        await _db.SaveChangesAsync();
                    }
                    else if (summativeVM.Summative.AssessmentType == SD.Assignment)
                    {
                        summativeVM.Summative.Assignment.SummId = summativeVM.Summative.Id;

                        _db.Update(summativeVM.Summative.Assignment);
                        await _db.SaveChangesAsync();
                    }

                    //2 - Update SA Order in Unit Table

                    int sumId = summativeVM.Summative.Id;
                    var currentUnit = await _db.CourseUnits.Where(c => c.Id == unitId).SingleOrDefaultAsync();
                    var currentSAOrder = currentUnit.SAOrder;

                    if (currentSAOrder == null || currentSAOrder == "")
                    {
                        currentUnit.SAOrder = sumId.ToString();
                        _db.Update(currentUnit);
                        await _db.SaveChangesAsync();
                    }
                    else if (currentSAOrder != null)
                    {
                        var finalOrder = String.Concat(currentSAOrder + "," + sumId.ToString());
                        currentUnit.SAOrder = finalOrder;
                        _db.Update(currentUnit);
                        await _db.SaveChangesAsync();

                    }


                    //If the AssessmentType is a Assignment
                    if (summativeVM.Summative.AssessmentType == SD.Assignment)
                    {
                        //Processing File
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        var files = HttpContext.Request.Form.Files;
                        var summativeFromDb = await _db.Summatives.FindAsync(summativeVM.Summative.Id);

                        if (files.Count() > 0)
                        {
                            //Files was uploaded
                            var uploads = Path.Combine(webRootPath, "assignmentfiles");
                            var extention = Path.GetExtension(files[0].FileName);

                            using (var filesStream = new FileStream(Path.Combine(uploads, summativeVM.Summative.Id + extention), FileMode.Create))
                            {
                                files[0].CopyTo(filesStream);
                            }

                            summativeFromDb.Assignment.AssignmentRequest = @"\assignmentfiles\" + summativeVM.Summative.Id + extention;
                            _db.Update(summativeFromDb);
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            _toastNotification.AddErrorToastMessage("PDF File is required!");

                            ModelState.AddModelError("AssignmentRequest", "Please select a PDF file for your assignment.");
                        }

                        return RedirectToAction("SummativeList", "Summative", new { id = unitId });

                    }

                    //If the AssessmentType is a Practical
                    if (summativeVM.Summative.AssessmentType == SD.Practical)
                    {
                        //Processing File
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        var files = HttpContext.Request.Form.Files;
                        var summativeFromDb = await _db.Summatives.FindAsync(summativeVM.Summative.Id);

                        if (files.Count() > 0)
                        {
                            //Files was uploaded
                            var uploads = Path.Combine(webRootPath, "practicalfiles");
                            var extention = Path.GetExtension(files[0].FileName);

                            using (var filesStream = new FileStream(Path.Combine(uploads, summativeVM.Summative.Id + extention), FileMode.Create))
                            {
                                files[0].CopyTo(filesStream);
                            }

                            summativeFromDb.Practical.PracticalRequest = @"\practicalfiles\" + summativeVM.Summative.Id + extention;
                            _db.Update(summativeFromDb);
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            _toastNotification.AddErrorToastMessage("PDF File is required!");
                            ModelState.AddModelError("PracticalRequest", "Please select a PDF file for your practical.");
                        }
                    }
                    _toastNotification.AddSuccessToastMessage("The " + summativeVM.Summative.AssessmentType + " was added!",
                     new ToastrOptions
                            {
                                Title = "Success",
                                ToastClass = "toastbox",
                            }
                        );

                    return RedirectToAction("SummativeList", "Summative", new { id = unitId });
                }
            }
            //If the ModelState is not valid

            SummativeViewModel modelVM = new SummativeViewModel
            {
                CourseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == unitId),
                Summative = summativeVM.Summative,
                SummativeList = new List<Summative>(),
                MultipleChoiceList = new List<MultipleChoice>(),
                TrueFalseList = new List<TrueFalse>(),
                AssignmentList = new List<Assignment>(),
                DirectQuestionList = new List<DirectQuestion>(),
                PracticalList = new List<Practical>(),
                AssessmentType = new List<string>() { SD.DirectQuestion, SD.TrueFalse, SD.MultipleChoice, SD.Assignment, SD.Practical }
            };

            return View(modelVM);

        }


        //GET - EDIT
        public async Task<IActionResult> Edit(int? Id)
        {

            if (Id == null)
            {
                return NotFound();
            }
            else
            {
                var summative = await _db.Summatives
                    .Include(p => p.Assignment)
                    .Include(p => p.DirectQuestion)
                    .Include(p => p.MultipleChoice)
                    .Include(p => p.Practical)
                    .Include(p => p.TrueFalse)
                    .SingleOrDefaultAsync(c => c.Id == Id);

                var unit = await _db.CourseUnits.SingleOrDefaultAsync(p => p.Id == summative.CourseUnitId);
              

                SummativeViewModel model = new SummativeViewModel
                {
                    CourseUnit = unit,
                    Summative = summative,
                    AssessmentType = new List<string> { summative.AssessmentType.ToString() }

                };
                return View(model);
            }

        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(SummativeViewModel summativeVM, string assessType, int Id, int unitId)
        {

            summativeVM.Summative.Id = Id;

            //Check if summative name allready exists

            var summativeExists = _db.Summatives.Where(c =>
                c.Title == summativeVM.Summative.Title &&
                c.CourseUnitId == summativeVM.Summative.CourseUnitId &&
                c.Id != Id).Count();


            summativeVM.Summative.AssessmentType = assessType;
            summativeVM.Summative.CourseUnitId = unitId;

            var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(p => p.Id == unitId);



            // If it does, oops:

            if (summativeExists > 0)
            {
                ModelState.AddModelError("Name", "Another Summative with the same name allready exist in this Unit. Please choose another name.");
                _toastNotification.AddErrorToastMessage("Please use another name",
     new ToastrOptions
     {
         Title = "Allready Exists!",
         ToastClass = "toastbox",
     }
     );

                SummativeViewModel model = new SummativeViewModel
                {
                    CourseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == summativeVM.Summative.CourseUnitId),
                    Summative = summativeVM.Summative,
                    AssessmentType = new List<string> { summativeVM.Summative.AssessmentType.ToString() }
                };
                return View(model);
            }

            else if (!ModelState.IsValid)
            {

                SummativeViewModel model = new SummativeViewModel
                {
                    CourseUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == summativeVM.Summative.CourseUnitId),
                    Summative = summativeVM.Summative,
                    AssessmentType = new List<string> { summativeVM.Summative.AssessmentType.ToString() }
                };
                return View(model);

            }


            if (summativeVM.Summative.AssessmentType == SD.Assignment)
            {

                //Processing File

                string webRootPath = _hostingEnvironment.WebRootPath;

                var files = HttpContext.Request.Form.Files;

                if (files.Count() > 0)
                {
                    //Files was uploaded

                    var uploads = Path.Combine(webRootPath, "assignmentfiles");
                    var extention = Path.GetExtension(files[0].FileName);

                    // Check if exists then delete

                    var filePath = Path.Combine(uploads, summativeVM.Summative.Id + extention);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    using (var filesStream = new FileStream(Path.Combine(uploads, summativeVM.Summative.Id + extention), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);

                    }

                }

                summativeVM.Summative.Assignment.AssignmentRequest = @"\assignmentfiles\" + summativeVM.Summative.Id + ".pdf";

            }

            else if (summativeVM.Summative.AssessmentType == SD.Practical)
            {

                //Processing File

                string webRootPath = _hostingEnvironment.WebRootPath;

                var files = HttpContext.Request.Form.Files;

                if (files.Count() > 0)
                {
                    //Files was uploaded

                    var uploads = Path.Combine(webRootPath, "practicalfiles");
                    var extention = Path.GetExtension(files[0].FileName);

                    // Check if exists then delete

                    var filePath = Path.Combine(uploads, summativeVM.Summative.Id + extention);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    using (var filesStream = new FileStream(Path.Combine(uploads, summativeVM.Summative.Id + extention), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);

                    }

                }

                summativeVM.Summative.Practical.PracticalRequest = @"\practicalfiles\" + summativeVM.Summative.Id + ".pdf";

            }

            _db.Update(summativeVM.Summative);
            await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("Assessment Updated!",
     new ToastrOptions
     {
         Title = "Success",
         ToastClass = "toastbox",
     }
     );

            return RedirectToAction("SummativeList", "Summative", new { id = summativeVM.Summative.CourseUnitId });

        }



        //GET - DELETE

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var summative = await _db.Summatives.FindAsync(id);

            if (summative == null)
            {
                return NotFound();
            }

            return View(summative);

        }


        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentSA = await _db.Summatives
                    .Include(p => p.Assignment)
                    .Include(p => p.DirectQuestion)
                    .Include(p => p.MultipleChoice)
                    .Include(p => p.Practical)
                    .Include(p => p.TrueFalse)
                    .SingleOrDefaultAsync(c => c.Id == id);




            var unitId = currentSA.CourseUnitId;
            var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == unitId);

            if (CanDeleteUnitOrChild.CanDel(currentUnit, _db))
            {
                var subValidity = await _db.SubmissionValidities.SingleOrDefaultAsync(p => p.SummativeId == id);

                var currentSAOrder = currentUnit.SAOrder;

                if (currentSAOrder == null || currentSAOrder == "" || !OrderList.ItemIdOrder(currentSAOrder).Contains(id))
                {
                    return NotFound();
                }

                currentUnit.SAOrder = RePosition.Delete(currentSAOrder, id);


                switch (currentSA.AssessmentType)
                {
                    case SD.MultipleChoice:
                        var mChoice = await _db.MultipleChoices.SingleOrDefaultAsync(c => c.Id == currentSA.MultipleChoiceId);
                        _db.MultipleChoices.Remove(mChoice);
                        break;

                    case SD.TrueFalse:
                        var trueFalse = await _db.TrueFalses.SingleOrDefaultAsync(c => c.Id == currentSA.TrueFalseId);
                        _db.TrueFalses.Remove(trueFalse);
                        break;

                    case SD.DirectQuestion:
                        var directQuestion = await _db.DirectQuestions.SingleOrDefaultAsync(c => c.Id == currentSA.DirectQuestionId);
                        _db.DirectQuestions.Remove(directQuestion);
                        break;

                    case SD.Assignment:
                        var assignment = await _db.Assignments.SingleOrDefaultAsync(c => c.Id == currentSA.AssignmentId);
                        DeleteFile.Remove(assignment.AssignmentRequest, _hostingEnvironment.WebRootPath);
                        _db.Assignments.Remove(assignment);
                        break;

                    case SD.Practical:
                        var practical = await _db.Practicals.SingleOrDefaultAsync(c => c.Id == currentSA.PracticalId);
                        DeleteFile.Remove(practical.PracticalRequest, _hostingEnvironment.WebRootPath);
                        _db.Practicals.Remove(practical);
                        break;

                }
                if (subValidity != null)
                {
                    _db.SubmissionValidities.Remove(subValidity);
                }

                _db.Summatives.Remove(currentSA);
                _db.Update(currentUnit);

                await _db.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Assessment Deleted!",
        new ToastrOptions
        {
            Title = "Success",
            ToastClass = "toastbox",
        }
        );
                return RedirectToAction("SummativeList", "Summative", new { id = unitId });

            }

            _toastNotification.AddErrorToastMessage("Assessment is contained in a Unit and Course in progress!",
               new ToastrOptions
               {
                   Title = "Assessment Not Deleted!",
                   ToastClass = "toastbox",
               }
       );
            return RedirectToAction("SummativeList", "Summative", new { id = unitId });

        } 


        public async Task<IActionResult> MoveSAUp(int Id)
        {
            var currentSA = await _db.Summatives.SingleOrDefaultAsync(c => c.Id == Id);

            var unitId = currentSA.CourseUnitId;

            var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == unitId);

            var currentSAOrder = currentUnit.SAOrder;

            if (RePosition.CanMoveUpCheck(currentSAOrder, Id))
            {
                currentUnit.SAOrder = RePosition.MoveUp(currentSAOrder, Id);
                _db.Update(currentUnit);
                await _db.SaveChangesAsync();
            }
            _toastNotification.AddSuccessToastMessage("Assessment Moved Up!",
                new ToastrOptions
                {
                    Title = "Success",
                    ToastClass = "toastbox",
                }
                );

            return RedirectToAction("SummativeList", "Summative", new { id = unitId });
        }

        public async Task<IActionResult> MoveSADown(int Id)
        {
            var currentSA = await _db.Summatives.SingleOrDefaultAsync(c => c.Id == Id);

            var unitId = currentSA.CourseUnitId;

            var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == unitId);

            var currentSAOrder = currentUnit.SAOrder;

            if (RePosition.CanMoveDownCheck(currentSAOrder, Id))
            {
                currentUnit.SAOrder = RePosition.MoveDown(currentSAOrder, Id);
                _db.Update(currentUnit);
                await _db.SaveChangesAsync();
            }
            _toastNotification.AddSuccessToastMessage("Assessment Moved Down!", new ToastrOptions
     {
         Title = "Success",
         ToastClass = "toastbox",
     }
     );
            return RedirectToAction("SummativeList", "Summative", new { id = unitId });
        }





    }
}