using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Extentions;
using LMS.Models;
using LMS.Models.Assessment;
using LMS.Models.Course;
using LMS.Models.Learning;
using LMS.Models.ViewModels;
using LMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace LMS.Areas.Learning.Controllers
{
    [Area("Learning")]
    public class AssessmentController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification;
        private IHostingEnvironment _hosting;


        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public AssessmentController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IToastNotification toastNotification, IHostingEnvironment hosting)
        {
            _db = db;
            _userManager = userManager;
            _toastNotification = toastNotification;
            _hosting = hosting;
        }

        [Authorize(Roles = SD.Assessor)]
        public async Task<IActionResult> AssessorClassList()
        {
            var currentUser = await GetCurrentUserAsync();
            //  var isAssessor = await _userManager.IsInRoleAsync(currentUser, SD.Assessor);

            var classesEnrolled = await _db.ClassEnrolments.Where(p => p.UserRole == SD.Assessor).Where(p => p.UserId == currentUser.Id).ToListAsync();

            ClassViewModel classVM = new ClassViewModel()
            {
                Classes = new List<Class>()
            };
            var classList = new List<Class>();

            foreach (var enrol in classesEnrolled)
            {
                var classe = await _db.Classes.Include(p => p.Course).SingleOrDefaultAsync(p => p.Id == enrol.ClassId);
                classList.Add(classe);
            }

            classVM.Classes = classList;
            return View(classVM);
        }

        [Authorize(Roles = SD.Assessor)]
        public async Task<IActionResult> DirectList(int classId)
        {
            var currentUser = await GetCurrentUserAsync();

            var classesEnrolled = await _db.ClassEnrolments.Include(p => p.Class).Where(p => p.UserRole == SD.Assessor).Where(p => p.UserId == currentUser.Id).ToListAsync();

            var classe = await _db.Classes.SingleOrDefaultAsync(p => p.Id == classId);
            var classeEnrol = classesEnrolled.SingleOrDefault(p => p.ClassId == classe.Id);

            var learnerEnrols = await _db.ClassEnrolments.Where(p => p.ClassId == classId).ToListAsync();

            Dictionary<ClassEnrolment, List<SummativeSubmission>> EnrolSubDirectList = new Dictionary<ClassEnrolment, List<SummativeSubmission>>();

            if (learnerEnrols != null && learnerEnrols.Any())
            {

                foreach (var enrol in learnerEnrols)
                {

                    var questionSubListPerEnrol = await _db.SummativeSubmissions.Include(p => p.SummativeSession).Where(p => p.SummativeSession.ClassEnrolId == enrol.Id).Where(p => p.DirectAnswer != null).ToListAsync();

                    if (questionSubListPerEnrol != null && questionSubListPerEnrol.Any())
                    {

                        EnrolSubDirectList.Add(enrol, questionSubListPerEnrol);
                    }
                }
            }
            AssessmentViewModel assessVM = new AssessmentViewModel()
            {
                EnrolSubList = EnrolSubDirectList,
                ClassEnrolment = await _db.ClassEnrolments.Include(p => p.Class.Course).SingleOrDefaultAsync(p => p.Id == classeEnrol.Id)
            };

            return View(assessVM);
        }

        [Authorize(Roles = SD.Assessor)]
        public async Task<IActionResult> AssignmentList(int classId)
        {
            var currentUser = await GetCurrentUserAsync();

            var classesEnrolled = await _db.ClassEnrolments.Include(p => p.Class).Where(p => p.UserRole == SD.Assessor).Where(p => p.UserId == currentUser.Id).ToListAsync();

            var classe = await _db.Classes.SingleOrDefaultAsync(p => p.Id == classId);
            var classeEnrol = classesEnrolled.SingleOrDefault(p => p.ClassId == classe.Id);

            var learnerEnrols = await _db.ClassEnrolments.Where(p => p.ClassId == classId).ToListAsync();

            Dictionary<ClassEnrolment, List<SummativeSubmission>> EnrolSubAssignmentList = new Dictionary<ClassEnrolment, List<SummativeSubmission>>();

            if (learnerEnrols != null && learnerEnrols.Any())
            {
                foreach (var enrol in learnerEnrols)
                {
                    var assignmentSubListPerEnrol = await _db.SummativeSubmissions.Include(p => p.SummativeSession).Include(p => p.Summative).Where(p => p.SummativeSession.ClassEnrolId == enrol.Id).Where(p => p.AssignmentSubmission != null).ToListAsync();

                    if (assignmentSubListPerEnrol != null && assignmentSubListPerEnrol.Any())
                    {
                        EnrolSubAssignmentList.Add(enrol, assignmentSubListPerEnrol);
                    }
                }
            }
            AssessmentViewModel assessVM = new AssessmentViewModel()
            {
                EnrolSubList = EnrolSubAssignmentList,
                ClassEnrolment = await _db.ClassEnrolments.Include(p => p.Class.Course).SingleOrDefaultAsync(p => p.Id == classeEnrol.Id)
            };

            return View(assessVM);
        }

        [Authorize(Roles = SD.Assessor)]
        public async Task<IActionResult> PracticalList(int classId)
        {
            var currentUser = await GetCurrentUserAsync();

            var classesEnrolled = await _db.ClassEnrolments.Include(p => p.Class).Where(p => p.UserRole == SD.Assessor).Where(p => p.UserId == currentUser.Id).ToListAsync();

            var classe = await _db.Classes.SingleOrDefaultAsync(p => p.Id == classId);
            var classeEnrol = classesEnrolled.SingleOrDefault(p => p.ClassId == classe.Id);

            var learnerEnrols = await _db.ClassEnrolments.Where(p => p.ClassId == classId).ToListAsync();

            Dictionary<ClassEnrolment, List<SummativeSubmission>> EnrolSubPracticalList = new Dictionary<ClassEnrolment, List<SummativeSubmission>>();

            if (learnerEnrols != null && learnerEnrols.Any())
            {

                foreach (var enrol in learnerEnrols)
                {
                    var practicalSubListPerEnrol = await _db.SummativeSubmissions.Include(p => p.SummativeSession).Where(p => p.SummativeSession.ClassEnrolId == enrol.Id).Where(p => p.PracticalSubmission != null).ToListAsync();

                    if (practicalSubListPerEnrol != null && practicalSubListPerEnrol.Count() > 0)
                    {
                        EnrolSubPracticalList.Add(enrol, practicalSubListPerEnrol);
                    }
                }
            }
            AssessmentViewModel assessVM = new AssessmentViewModel()
            {
                EnrolSubList = EnrolSubPracticalList,
                ClassEnrolment = await _db.ClassEnrolments.Include(p => p.Class.Course).SingleOrDefaultAsync(p => p.Id == classeEnrol.Id)
            };

            return View(assessVM);
        }

        [Authorize(Roles = SD.Assessor)]
        public async Task<IActionResult> DirectSessions(int learnerId, int enrolId)
        {

            //Get Assessor Info
            var currentUser = await GetCurrentUserAsync();
            //Get Learner Enrol Info
            var learnerEnrol = await _db.ClassEnrolments.Include(p => p.Class).SingleOrDefaultAsync(p => p.Id == enrolId);
            //Get the Enrolment of the assessor in the class
            var classesEnrolled = await _db.ClassEnrolments.Include(p => p.Class).Where(p => p.UserRole == SD.Assessor).Where(p => p.ClassId == learnerEnrol.ClassId).Where(p => p.UserId == currentUser.Id).ToListAsync();

            var assessors = await _userManager.GetUsersInRoleAsync(SD.Assessor);

            if (classesEnrolled == null)
            {

                return NotFound("The assessor is not enrolled into this class");
            }

            var directSessions = await _db.SummativeSessions.Where(p => p.ClassEnrolId == learnerEnrol.Id).ToListAsync();

            Dictionary<SummativeSession, List<SummativeSubmission>> SessionSubDirectList = new Dictionary<SummativeSession, List<SummativeSubmission>>();

            if (directSessions != null && directSessions.Count() > 0)
            {

                foreach (var session in directSessions)
                {
                    var questionSubListPerSession = await _db.SummativeSubmissions
                        .Include(p => p.SummativeSession)
                        .Include(p => p.Summative)
                        .Where(p => p.SummativeSession.ClassEnrolId == learnerEnrol.Id)
                        .Where(p => p.Summative.AssessmentType == SD.DirectQuestion)
                        .Where(p => p.DirectAnswer != null)
                        .ToListAsync();

                    if (questionSubListPerSession != null && questionSubListPerSession.Count() > 0)
                    {
                        SessionSubDirectList.Add(session, questionSubListPerSession);
                    }
                }
            }
            AssessmentViewModel assessVM = new AssessmentViewModel()
            {
                SessionSubList = SessionSubDirectList,
                ClassEnrolment = await _db.ClassEnrolments.Include(p => p.Class.Course).SingleOrDefaultAsync(p => p.Id == learnerEnrol.Id),
                FirstName = learnerEnrol.UserName,
                LastName = learnerEnrol.UserSurname,
                IdNumber = learnerEnrol.Identity,
                UnitList = await _db.CourseUnits.ToListAsync(),
                Assessors = assessors
            };

            return View(assessVM);
        }

        [Authorize(Roles = SD.Assessor)]
        public async Task<IActionResult> EditGradeDirect(int sessionId)
        {
            //Get Assessor Info
            var currentUser = await GetCurrentUserAsync();
            //Get Session Info
            var session = await _db.SummativeSessions.SingleOrDefaultAsync(p => p.Id == sessionId);

            //Get Learner Enrol Info
            var learnerEnrol = await _db.ClassEnrolments.Include(p => p.Class).SingleOrDefaultAsync(p => p.Id == session.ClassEnrolId);

            //Get the Enrolment of the assessor in the class
            var classesEnrolled = await _db.ClassEnrolments.Include(p => p.Class).Where(p => p.UserRole == SD.Assessor).Where(p => p.ClassId == learnerEnrol.ClassId).Where(p => p.UserId == currentUser.Id).ToListAsync();

            var assessors = await _userManager.GetUsersInRoleAsync(SD.Assessor);

            if (classesEnrolled == null)
            {

                return NotFound("The assessor is not enrolled into this class");
            }

            var directSubmissions = await _db.SummativeSubmissions
                .Include(p => p.Summative.DirectQuestion)
                .Where(p => p.SummativeSessionId == session.Id)
                .Where(p => p.Summative.AssessmentType == SD.DirectQuestion)
                .ToListAsync();

            var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(p => p.Id == session.UnitId);

            AssessmentViewModel assessVM = new AssessmentViewModel()
            {
                SubmissionList = directSubmissions,
                ClassEnrolment = await _db.ClassEnrolments.Include(p => p.Class.Course).SingleOrDefaultAsync(p => p.Id == learnerEnrol.Id),
                FirstName = learnerEnrol.UserName,
                LastName = learnerEnrol.UserSurname,
                IdNumber = learnerEnrol.Identity,
                Assessors = assessors,
                Summative = new Summative(),
                SummativeSubmission = new SummativeSubmission(),
                DirectQuestion = new DirectQuestion(),
                UnitTitle = currentUnit.Name

            };

            return View(assessVM);
        }

        [Authorize(Roles = SD.Assessor)]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("SaveGradeDirect")]
        public async Task<IActionResult> SaveGradeDirect(AssessmentViewModel assessVM, int subId, int grade)
        {

            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("The grade was not updated!",
                       new ToastrOptions
                       {
                           Title = "Error!",
                           ToastClass = "toastbox",
                       }
                       );
                return View();
            }
            var currentUser = await GetCurrentUserAsync();

            var sub = await _db.SummativeSubmissions.SingleOrDefaultAsync(p => p.Id == subId);

            if (Request.Form[subId.ToString()] != "")
            {
                sub.GradePercentage = Convert.ToInt32(Request.Form[subId.ToString()]);
            }
            else
            {
                _toastNotification.AddErrorToastMessage("The grade was not updated!",
                                      new ToastrOptions
                                      {
                                          Title = "Error!",
                                          ToastClass = "toastbox",
                                      }
                                      );
                return RedirectToAction("EditGradeDirect", "Assessment", new { sessionId = sub.SummativeSessionId });
            }

            sub.GradingDate = DateTime.Now;
            sub.AssessorId = currentUser.Id;

            _db.Update(sub);
            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("The grade was successfully updated",
                        new ToastrOptions
                        {
                            Title = "Success",
                            ToastClass = "toastbox",
                        }
                        );
            return RedirectToAction("EditGradeDirect", "Assessment", new { sessionId = sub.SummativeSessionId });
        }




        [Authorize(Roles = SD.Assessor)]
        public async Task<IActionResult> AssignmentSessions(int learnerId, int enrolId)
        {

            //Get Assessor Info
            var currentUser = await GetCurrentUserAsync();
            //Get Learner Enrol Info
            var learnerEnrol = await _db.ClassEnrolments.Include(p => p.Class).SingleOrDefaultAsync(p => p.Id == enrolId);
            //Get the Enrolment of the assessor in the class
            var classesEnrolled = await _db.ClassEnrolments.Include(p => p.Class).Where(p => p.UserRole == SD.Assessor).Where(p => p.ClassId == learnerEnrol.ClassId).Where(p => p.UserId == currentUser.Id).ToListAsync();

            var assessors = await _userManager.GetUsersInRoleAsync(SD.Assessor);

            if (classesEnrolled == null)
            {

                return NotFound("The assessor is not enrolled into this class");
            }

            var assignmentSessions = await _db.SummativeSessions.Where(p => p.ClassEnrolId == learnerEnrol.Id).ToListAsync();

            Dictionary<SummativeSession, List<SummativeSubmission>> SessionSubAssignmentList = new Dictionary<SummativeSession, List<SummativeSubmission>>();

            if (assignmentSessions != null && assignmentSessions.Count() > 0)
            {

                foreach (var session in assignmentSessions)
                {
                    var assignmentSubListPerSession = await _db.SummativeSubmissions
                        .Include(p => p.SummativeSession)
                        .Include(p => p.Summative)
                        .Where(p => p.SummativeSession.ClassEnrolId == learnerEnrol.Id)
                        .Where(p => p.Summative.AssessmentType == SD.Assignment)
                        .Where(p => p.AssignmentSubmission != null)
                        .ToListAsync();

                    if (assignmentSubListPerSession != null && assignmentSubListPerSession.Count() > 0)
                    {
                        SessionSubAssignmentList.Add(session, assignmentSubListPerSession);
                    }
                }
            }
            AssessmentViewModel assessVM = new AssessmentViewModel()
            {
                SessionSubList = SessionSubAssignmentList,
                ClassEnrolment = await _db.ClassEnrolments.Include(p => p.Class.Course).SingleOrDefaultAsync(p => p.Id == learnerEnrol.Id),
                FirstName = learnerEnrol.UserName,
                LastName = learnerEnrol.UserSurname,
                IdNumber = learnerEnrol.Identity,
                UnitList = await _db.CourseUnits.ToListAsync(),
                Assessors = assessors
            };

            return View(assessVM);
        }

        [Authorize(Roles = SD.Assessor)]
        public async Task<IActionResult> EditGradeAssignment(int sessionId)
        {
            //Get Assessor Info
            var currentUser = await GetCurrentUserAsync();
            //Get Session Info
            var session = await _db.SummativeSessions.SingleOrDefaultAsync(p => p.Id == sessionId);

            //Get Learner Enrol Info
            var learnerEnrol = await _db.ClassEnrolments.Include(p => p.Class).SingleOrDefaultAsync(p => p.Id == session.ClassEnrolId);

            //Get the Enrolment of the assessor in the class
            var classesEnrolled = await _db.ClassEnrolments.Include(p => p.Class).Where(p => p.UserRole == SD.Assessor).Where(p => p.ClassId == learnerEnrol.ClassId).Where(p => p.UserId == currentUser.Id).ToListAsync();

            var assessors = await _userManager.GetUsersInRoleAsync(SD.Assessor);

            if (classesEnrolled == null)
            {

                return NotFound("The assessor is not enrolled into this class");
            }

            var assignmentSubmissions = await _db.SummativeSubmissions
                .Include(p => p.Summative.Assignment)
                .Where(p => p.SummativeSessionId == session.Id)
                .Where(p => p.Summative.AssessmentType == SD.Assignment)
                .ToListAsync();

            var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(p => p.Id == session.UnitId);

            AssessmentViewModel assessVM = new AssessmentViewModel()
            {
                SubmissionList = assignmentSubmissions,
                ClassEnrolment = await _db.ClassEnrolments.Include(p => p.Class.Course).SingleOrDefaultAsync(p => p.Id == learnerEnrol.Id),
                FirstName = learnerEnrol.UserName,
                LastName = learnerEnrol.UserSurname,
                IdNumber = learnerEnrol.Identity,
                Assessors = assessors,
                Summative = new Summative(),
                SummativeSubmission = new SummativeSubmission(),
                Assignment = new Assignment(),
                UnitTitle = currentUnit.Name

            };

            return View(assessVM);
        }


        [Authorize(Roles = SD.Assessor)]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("SaveGradeAssignmentProject")]
        public async Task<IActionResult> SaveGradeAssignmentProject(AssessmentViewModel assessVM, int subId, int grade)
        {

            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("The grade was not updated!",
                       new ToastrOptions
                       {
                           Title = "Error!",
                           ToastClass = "toastbox",
                       }
                       );
                return View();
            }
            else
            {

                var currentUser = await GetCurrentUserAsync();

                var sub = await _db.SummativeSubmissions.Include(p=>p.Summative).SingleOrDefaultAsync(p => p.Id == subId);

                if (Request.Form[subId.ToString()] != "")
                {
                    sub.GradePercentage = Convert.ToInt32(Request.Form[subId.ToString()]);
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("The grade was not updated!",
                                          new ToastrOptions
                                          {
                                              Title = "Error!",
                                              ToastClass = "toastbox",
                                          }
                                          );

                    if (sub.Summative.AssessmentType == SD.Assignment) {
                        return RedirectToAction("EditGradeAssignment", "Assessment", new { sessionId = sub.SummativeSessionId });
                    }
                    else
                    {
                        return RedirectToAction("EditGradePractical", "Assessment", new { sessionId = sub.SummativeSessionId });
                    }
                    
                }


                //Processing File

                string webRootPath = _hosting.WebRootPath;
                var files = HttpContext.Request.Form.Files;


                if (files.Count() > 0)
                {

                    //Files was uploaded

                    var uploads = Path.Combine(webRootPath, "submissions\\gradings");
                    var extention = Path.GetExtension(files[0].FileName);

                    Directory.CreateDirectory(uploads);

                    using (var filesStream = new FileStream(Path.Combine(uploads, subId + extention), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }
                    if (sub.Summative.AssessmentType == SD.Assignment)
                    {
                        sub.AssignmentGraded = @"\submissions\gradings\" + subId + extention;
                    }
                    else if(sub.Summative.AssessmentType == SD.Practical)
                    {
                        sub.PracticalGraded = @"\submissions\gradings\" + subId + extention;
                    }
                    

                }
                else
                {
                    ModelState.AddModelError("files", "No File is submitted. Please select a file to submit.");

                }

                sub.GradingDate = DateTime.Now;
                sub.AssessorId = currentUser.Id;

                _db.Update(sub);
                await _db.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("The grade was successfully updated",
                            new ToastrOptions
                            {
                                Title = "Success",
                                ToastClass = "toastbox",
                            }
                            );

                if (sub.Summative.AssessmentType == SD.Assignment)
                {
                    return RedirectToAction("EditGradeAssignment", "Assessment", new { sessionId = sub.SummativeSessionId });
                }
                else
                {
                    return RedirectToAction("EditGradePractical", "Assessment", new { sessionId = sub.SummativeSessionId });
                }
            }
         }





        [Authorize(Roles = SD.Assessor)]
        public async Task<IActionResult> PracticalSessions(int learnerId, int enrolId)
        {

            //Get Assessor Info
            var currentUser = await GetCurrentUserAsync();
            //Get Learner Enrol Info
            var learnerEnrol = await _db.ClassEnrolments.Include(p => p.Class).SingleOrDefaultAsync(p => p.Id == enrolId);
            //Get the Enrolment of the assessor in the class
            var classesEnrolled = await _db.ClassEnrolments.Include(p => p.Class).Where(p => p.UserRole == SD.Assessor).Where(p => p.ClassId == learnerEnrol.ClassId).Where(p => p.UserId == currentUser.Id).ToListAsync();

            var assessors = await _userManager.GetUsersInRoleAsync(SD.Assessor);

            if (classesEnrolled == null)
            {

                return NotFound("The assessor is not enrolled into this class");
            }

            var practicalSessions = await _db.SummativeSessions.Where(p => p.ClassEnrolId == learnerEnrol.Id).ToListAsync();

            Dictionary<SummativeSession, List<SummativeSubmission>> SessionSubPracticalList = new Dictionary<SummativeSession, List<SummativeSubmission>>();

            if (practicalSessions != null && practicalSessions.Count() > 0)
            {

                foreach (var session in practicalSessions)
                {
                    var practicalSubListPerSession = await _db.SummativeSubmissions
                        .Include(p => p.SummativeSession)
                        .Include(p => p.Summative)
                        .Where(p => p.SummativeSession.ClassEnrolId == learnerEnrol.Id)
                        .Where(p => p.Summative.AssessmentType == SD.Practical)
                        .Where(p => p.PracticalSubmission != null)
                        .ToListAsync();

                    if (practicalSubListPerSession != null && practicalSubListPerSession.Count() > 0)
                    {
                        SessionSubPracticalList.Add(session, practicalSubListPerSession);
                    }
                }
            }
            AssessmentViewModel assessVM = new AssessmentViewModel()
            {
                SessionSubList = SessionSubPracticalList,
                ClassEnrolment = await _db.ClassEnrolments.Include(p => p.Class.Course).SingleOrDefaultAsync(p => p.Id == learnerEnrol.Id),
                FirstName = learnerEnrol.UserName,
                LastName = learnerEnrol.UserSurname,
                IdNumber = learnerEnrol.Identity,
                UnitList = await _db.CourseUnits.ToListAsync(),
                Assessors = assessors
            };

            return View(assessVM);
        }

        [Authorize(Roles = SD.Assessor)]
        public async Task<IActionResult> EditGradePractical(int sessionId)
        {
            //Get Assessor Info
            var currentUser = await GetCurrentUserAsync();
            //Get Session Info
            var session = await _db.SummativeSessions.SingleOrDefaultAsync(p => p.Id == sessionId);

            //Get Learner Enrol Info
            var learnerEnrol = await _db.ClassEnrolments.Include(p => p.Class).SingleOrDefaultAsync(p => p.Id == session.ClassEnrolId);

            //Get the Enrolment of the assessor in the class
            var classesEnrolled = await _db.ClassEnrolments.Include(p => p.Class).Where(p => p.UserRole == SD.Assessor).Where(p => p.ClassId == learnerEnrol.ClassId).Where(p => p.UserId == currentUser.Id).ToListAsync();

            var assessors = await _userManager.GetUsersInRoleAsync(SD.Assessor);

            if (classesEnrolled == null)
            {

                return NotFound("The assessor is not enrolled into this class");
            }

            var practicalSubmissions = await _db.SummativeSubmissions
                .Include(p => p.Summative.Practical)
                .Where(p => p.SummativeSessionId == session.Id)
                .Where(p => p.Summative.AssessmentType == SD.Practical)
                .ToListAsync();

            var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(p => p.Id == session.UnitId);

            AssessmentViewModel assessVM = new AssessmentViewModel()
            {
                SubmissionList = practicalSubmissions,
                ClassEnrolment = await _db.ClassEnrolments.Include(p => p.Class.Course).SingleOrDefaultAsync(p => p.Id == learnerEnrol.Id),
                FirstName = learnerEnrol.UserName,
                LastName = learnerEnrol.UserSurname,
                IdNumber = learnerEnrol.Identity,
                Assessors = assessors,
                Summative = new Summative(),
                SummativeSubmission = new SummativeSubmission(),
                Practical = new Practical(),
                UnitTitle = currentUnit.Name

            };

            return View(assessVM);
        }




        public async Task<IActionResult> DeleteGrading(int subId)
        {

            var currentUser = await GetCurrentUserAsync();
            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);


            var sub = await _db.SummativeSubmissions.Include(p => p.Summative).SingleOrDefaultAsync(p => p.Id == subId);

         
            if (sub == null)
            {

                return NotFound("Delete Action Not Permitted");

            }
            
            string webRootPath = _hosting.WebRootPath;

            var thePath = "";

            if (sub.Summative.AssessmentType == SD.Assignment)
            {
                thePath = Path.Combine(webRootPath, sub.AssignmentGraded.TrimStart('\\'));
            }
            else if (sub.Summative.AssessmentType == SD.Practical)
            {
                thePath = Path.Combine(webRootPath, sub.PracticalGraded.TrimStart('\\'));
            }
                      

            if (System.IO.File.Exists(thePath))
            {
                System.IO.File.Delete(thePath);
            }
                       
            sub.PracticalGraded = null;
            sub.AssignmentGraded = null;

            _db.Update(sub);

            await _db.SaveChangesAsync();

            if (sub.Summative.AssessmentType == SD.Assignment)
            {
                return RedirectToAction("EditGradeAssignment", "Assessment", new { sessionId = sub.SummativeSessionId });
            }
            else
            {
                return RedirectToAction("EditGradePractical", "Assessment", new { sessionId = sub.SummativeSessionId });
            }
        }



    }
}