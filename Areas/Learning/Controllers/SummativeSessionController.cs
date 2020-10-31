using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Extentions;
using LMS.Models;
using LMS.Models.Assessment;
using LMS.Models.Learning;
using LMS.Models.ViewModels;
using LMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Areas.Learning.Controllers
{
    [Area("Learning")]
    [Authorize(Roles = "Administrator,Learner")]
    public class SummativeSessionController : Controller
    {

        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _hosting;

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        public SummativeSessionController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IHostingEnvironment hosting)
        {
            _db = db;
            _userManager = userManager;
            _hosting = hosting;
        }
        
        public async Task<IActionResult> ViewAllSummative(int Id) {

            var unit = await _db.CourseUnits.SingleOrDefaultAsync(p => p.Id == Id);
            var currentUser = await GetCurrentUserAsync();

            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);

            if (unit.SAOrder != "" && unit.SAOrder != null)
            {
                var summatives = OrderList.ItemIdOrder(unit.SAOrder);
                var summativeList = new List<Summative>();

                var directList = new List<DirectQuestion>();
                var mcList = new List<MultipleChoice>();
                var tfList = new List<TrueFalse>();
                var assList = new List<Assignment>();
                var practList = new List<Practical>();

                var subValList = new List<SubmissionValidity>();

                foreach (var summ in summatives) {

                    var sumFromDb = await _db.Summatives
                        .Include(p => p.DirectQuestion)
                        .Include(p => p.Assignment)
                        .Include(p => p.MultipleChoice)
                        .Include(p => p.Practical)
                        .Include(p => p.TrueFalse)
                        .SingleOrDefaultAsync(p => p.Id == summ);

                    summativeList.Add(sumFromDb);

                    if (sumFromDb.AssessmentType == SD.Assignment) {
                        var sub = await _db.Assignments.SingleOrDefaultAsync(p => p.Id == sumFromDb.AssignmentId);
                        assList.Add(sub);

                        var val = await _db.SubmissionValidities.SingleOrDefaultAsync(p=>p.SummativeId == sumFromDb.Id);
                        if (val != null) {
                            subValList.Add(val);}
                       
                    }
                    else if (sumFromDb.AssessmentType == SD.DirectQuestion)
                    {
                        var sub = await _db.DirectQuestions.SingleOrDefaultAsync(p => p.Id == sumFromDb.DirectQuestionId);
                        directList.Add(sub);

                    }
                    if (sumFromDb.AssessmentType == SD.MultipleChoice)
                    {
                        var sub = await _db.MultipleChoices.SingleOrDefaultAsync(p => p.Id == sumFromDb.MultipleChoiceId);
                        mcList.Add(sub);
                    }
                    else if (sumFromDb.AssessmentType == SD.TrueFalse)
                    {
                        var sub = await _db.TrueFalses.SingleOrDefaultAsync(p => p.Id == sumFromDb.TrueFalseId);
                        tfList.Add(sub);
                    }
                    if (sumFromDb.AssessmentType == SD.Practical)
                    {
                        var sub = await _db.Practicals.SingleOrDefaultAsync(p => p.Id == sumFromDb.PracticalId);                     
                        practList.Add(sub);

                        var val = await _db.SubmissionValidities.SingleOrDefaultAsync(p => p.SummativeId == sumFromDb.Id);
                        if (val != null) {  subValList.Add(val);}
                       
                    }

                }

                var config = await _db.ConfigOptions.SingleOrDefaultAsync();

                SummativeSubmissionViewModel sumVM = new SummativeSubmissionViewModel
                {
                    MCList = mcList,
                    PractList = practList,
                    DirectList = directList,
                    AssList = assList,
                    TFList = tfList,
                    UnitTitle = unit.Name,
                    UnitId = unit.Id, SubValList = subValList,
                    QuestionCount = mcList.Count() + tfList.Count() + directList.Count(), Practical = new Practical(),
                     QuestionMinutes = config.MinutesPerQuest
                };

                var completedQuestions = new List<Summative>();
                var requiredQuestions = new List<Summative>();


                if (classEnrol.CompeletedSas != null && classEnrol.CompeletedSas != "")
                {
                    var completedSA = OrderList.ItemIdOrder(classEnrol.CompeletedSas);
                    var completedSAList = new List<Summative>();
                   

                    foreach (var sa in completedSA) {
                        var addtoDb = await _db.Summatives.SingleOrDefaultAsync(p => p.Id == sa);
                        
                        completedSAList.Add(addtoDb);
                    }

                    //Getting the submissions for the learner of the unit related to the class enrolment

                    var subList = await _db.SummativeSubmissions
                         .Where(p => p.UserId == currentUser.Id)
                         .Where(p => p.SummativeSession.ClassEnrolId == classEnrol.Id)
                         .Where(p => p.Summative.CourseUnitId == unit.Id).ToListAsync();

                    sumVM.SubList = subList;


                    // Creating Dict List of Completed Assignment / Submission

                    var compAssSummative = new List<Summative>();
                    compAssSummative = completedSAList.Where(p => p.AssessmentType == SD.Assignment).ToList();

                    Dictionary<Assignment,SummativeSubmission> AssSub = new Dictionary<Assignment, SummativeSubmission>();

                    foreach (var ass in compAssSummative) {

                        var assignment = await _db.Assignments.SingleOrDefaultAsync(p=>p.Id == ass.AssignmentId);
                        var sub = subList.Where(p => p.SummativeId == ass.Id).SingleOrDefault();

                        if (sub != null) {
                        AssSub.Add(assignment, sub);
                        }
                   
                    }
                    sumVM.AssSub = AssSub;

                    //Ass - Dict End

                    // Creating Dict List of Completed Practicals / Submission
                    var compPracSummative = new List<Summative>();
                    compPracSummative = completedSAList.Where(p => p.AssessmentType == SD.Practical).ToList();

                    Dictionary<Practical, SummativeSubmission> PracSub = new Dictionary<Practical, SummativeSubmission>();

                    foreach (var pract in compPracSummative)
                    {

                        var practical = await _db.Practicals.SingleOrDefaultAsync(p => p.Id == pract.PracticalId);
                        var sub = subList.Where(p => p.SummativeId == pract.Id).SingleOrDefault();

                        if (sub != null)
                        {
                            PracSub.Add(practical, sub);
                        }

                    }
                    sumVM.PracSub = PracSub;

                    //Practical - Dict End



                    completedQuestions = completedSAList.Where(p => 
                    p.AssessmentType == SD.TrueFalse || 
                    p.AssessmentType == SD.MultipleChoice ||
                    p.AssessmentType == SD.DirectQuestion).Where(p=>p.CourseUnitId == unit.Id)
                    .ToList();
                    requiredQuestions = summativeList.Where(p => 
                    p.AssessmentType == SD.TrueFalse || 
                    p.AssessmentType == SD.MultipleChoice ||
                    p.AssessmentType == SD.DirectQuestion).Where(p => p.CourseUnitId == unit.Id)
                    .ToList();

                    var sumSession = new SummativeSession();

                    int? totalQcorrect = 0;
                    int? autoresult = 0;
                    bool allQuestGradesNotAvail = false;
                    
                    if (requiredQuestions != null && requiredQuestions.Count() > 0) {

                        foreach (var sum in completedQuestions)
                        {
                            var sub = await _db.SummativeSubmissions.Where(p => p.SummativeId == sum.Id).Where(p=>p.UserId == currentUser.Id).SingleOrDefaultAsync();
                            sumSession = await _db.SummativeSessions.SingleOrDefaultAsync(p=>p.Id == sub.SummativeSessionId);
                            if (sub.GradePercentage != null)
                            {
                                totalQcorrect = totalQcorrect += sub.GradePercentage;
                            
                            }
                            else if (sub.GradePercentage == null)
                            {
                                allQuestGradesNotAvail = true;
                            }
                        }

                        if (totalQcorrect != null && completedQuestions.Count() > 0) {

                        autoresult = totalQcorrect / completedQuestions.Count();
                        sumSession.QuestionInProgressGrade = autoresult;
                        }

                        

                        sumSession.QuestionGradeTotal = allQuestGradesNotAvail ? null : autoresult;

                    }

                    sumVM.PracticalCountCompleted = completedSAList.Where(p => p.AssessmentType == SD.Practical).Count();
                    sumVM.AssignmentCountCompleted = completedSAList.Where(p => p.AssessmentType == SD.Assignment).Count();
                    sumVM.QuestionCountCompleted = completedSAList
                        .Where(p => p.AssessmentType == SD.DirectQuestion || p.AssessmentType == SD.MultipleChoice || p.AssessmentType == SD.TrueFalse)
                        .Where(p=>p.CourseUnitId == unit.Id)
                        .Count();

                    sumVM.SummativeSession = sumSession;

                    //_db.Update(sumVM.SummativeSession);
                    //_db.SaveChanges();
                }
               
                return View(sumVM);
            }

            return NotFound();
        }

        public async Task<IActionResult> SummativeKnowledge(int unitId)
        {

            var unit = await _db.CourseUnits.SingleOrDefaultAsync(p => p.Id == unitId);
            var currentUser = await GetCurrentUserAsync();

            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);

            if (unit.SAOrder != "" && unit.SAOrder != null)
            {
                var summatives = OrderList.ItemIdOrder(unit.SAOrder);
                var remainingSummatives = new List<Summative>();

                //Creating a list of Summative Knowledge Assessments
                foreach (var sum in summatives)
                {
                    var summativeFromDB = await _db.Summatives.Include(p => p.DirectQuestion).Include(p => p.MultipleChoice).Include(p => p.TrueFalse).SingleOrDefaultAsync(p => p.Id == sum);
                    if (summativeFromDB.AssessmentType != SD.Assignment && summativeFromDB.AssessmentType != SD.Practical) {
                        remainingSummatives.Add(summativeFromDB);
                    }
                }

                var summativeSession = await _db.SummativeSessions
                    .Where(p => p.UserId == currentUser.Id)
                    .Where(p => p.UnitId == unitId)
                    .Where(p => p.ClassEnrolId == classEnrol.Id).SingleOrDefaultAsync();


                SummativeSession newSummative = new SummativeSession();

                if (summativeSession == null)
                {

                    newSummative.ClassEnrolId = classEnrol.Id;
                    newSummative.SAIdsInOrder = unit.SAOrder;
                    newSummative.SAStart = DateTime.Now;
                    newSummative.UnitId = unitId;
                    newSummative.UserId = currentUser.Id;


                    _db.SummativeSessions.Add(newSummative);
                    _db.SaveChanges();
                }
                else
                {
                    newSummative = summativeSession;
                }



                //check what summatives are completed, if any move on to next
                //set summative in viewModel below equal to the next summative to be completed and send through to view

                SummativeSubmissionViewModel sumVM = new SummativeSubmissionViewModel
                {
                    ClassEnrolId = newSummative.ClassEnrolId,
                    UnitId = unitId,
                    SummativeSubmission = new SummativeSubmission
                    {
                        SummativeSessionId = newSummative.Id,
                        UserId = currentUser.Id
                    }
                };

                //If there are no completed SA's, set the SA to the first on the list 

                if (classEnrol.CompeletedSas == null || classEnrol.CompeletedSas == "")
                {
                    sumVM.SummativeSubmission.SummativeId = remainingSummatives[0].Id;

                    if (remainingSummatives[0].AssessmentType == SD.MultipleChoice)
                    {
                        sumVM.QuestionType = SD.MultipleChoice;
                        sumVM.MultipleChoice = await _db.MultipleChoices.SingleOrDefaultAsync(p => p.Id == remainingSummatives[0].MultipleChoiceId);
                    }
                    else if (remainingSummatives[0].AssessmentType == SD.TrueFalse)
                    {
                        sumVM.QuestionType = SD.TrueFalse;
                        sumVM.TrueFalse = await _db.TrueFalses.SingleOrDefaultAsync(p => p.Id == remainingSummatives[0].TrueFalseId);
                    }
                    else if (remainingSummatives[0].AssessmentType == SD.DirectQuestion)
                    {
                        sumVM.QuestionType = SD.DirectQuestion;
                        sumVM.DirectQuestion = await _db.DirectQuestions.SingleOrDefaultAsync(p => p.Id == remainingSummatives[0].DirectQuestionId);
                    }
                    sumVM.QuestionCount = remainingSummatives.Count();

                    return View(sumVM);
                }
                //If there are SA's completed, remove them from the list..Then set the FA to the first on the new list
                else
                {
                    var completedSummativeIds = OrderList.ItemIdOrder(classEnrol.CompeletedSas);
                    var completedSummatives = new List<Summative>();

                    //Create list of completed summatives
                    foreach (var form in completedSummativeIds)
                    {
                        var summativeFromDB = await _db.Summatives.Include(p => p.DirectQuestion).Include(p => p.MultipleChoice).Include(p => p.TrueFalse).SingleOrDefaultAsync(p => p.Id == form);
                        if (summativeFromDB.AssessmentType != SD.Assignment && summativeFromDB.AssessmentType != SD.Practical)
                        {
                            completedSummatives.Add(summativeFromDB);
                        }
                    }

                    var newremainingSummatives = new List<Summative>();

                    foreach (var sum in remainingSummatives)
                    {

                        if (!completedSummatives.Contains(sum))
                        {
                            newremainingSummatives.Add(sum);
                        };

                    }

                    //Setting the formative to the first on the new list generated from what's not comepleted in the ComepletedFA table

                    if (newremainingSummatives.Count() > 0 && newremainingSummatives != null)
                    {

                        sumVM.SummativeSubmission.SummativeId = newremainingSummatives[0].Id;


                        if (newremainingSummatives[0].AssessmentType == SD.MultipleChoice)
                        {
                            sumVM.MultipleChoice = await _db.MultipleChoices.SingleOrDefaultAsync(p => p.Id == newremainingSummatives[0].MultipleChoiceId);
                            sumVM.QuestionType = SD.MultipleChoice;
                        }
                        else if (newremainingSummatives[0].AssessmentType == SD.TrueFalse)
                        {
                            sumVM.TrueFalse = await _db.TrueFalses.SingleOrDefaultAsync(p => p.Id == newremainingSummatives[0].TrueFalseId);
                            sumVM.QuestionType = SD.TrueFalse;
                        }
                        if (newremainingSummatives[0].AssessmentType == SD.DirectQuestion)
                        {
                            sumVM.DirectQuestion = await _db.DirectQuestions.SingleOrDefaultAsync(p => p.Id == newremainingSummatives[0].DirectQuestionId);
                            sumVM.QuestionType = SD.DirectQuestion;
                        }
                    }
                    sumVM.QuestionCount = newremainingSummatives.Count();

                    if (newremainingSummatives.Count() == 0)
                    {
                        var saSession = await _db.SummativeSessions.SingleOrDefaultAsync(p => p.Id == sumVM.SummativeSubmission.SummativeSessionId);
                    }

                    return View(sumVM);
                }

            }

            return NotFound();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CaptureSummativeKnowledge(SummativeSubmissionViewModel sumVM)
        {

            if (!ModelState.IsValid)
            {
                return View("Summatives", sumVM);
            }

            var newSum = await _db.Summatives.Include(p=>p.MultipleChoice).Include(p=>p.TrueFalse).SingleOrDefaultAsync(p=>p.Id == sumVM.SummativeSubmission.SummativeId);

            if (newSum.AssessmentType == SD.MultipleChoice)
            {
                if (newSum.MultipleChoice.CorrectAnswer.ToString() == sumVM.SummativeSubmission.MCAnswer)
                {
                    sumVM.SummativeSubmission.GradePercentage = 100;
                }
                else
                {
                    sumVM.SummativeSubmission.GradePercentage = 0;
                }

            }
            else if (newSum.AssessmentType == SD.TrueFalse)
            {
                if (newSum.TrueFalse.CorrectAnswer.ToString() == sumVM.SummativeSubmission.TFAnswer)
                {
                    sumVM.SummativeSubmission.GradePercentage = 100;
                }
                else
                {
                    sumVM.SummativeSubmission.GradePercentage = 0;
                }
            }
                       
            _db.Add(sumVM.SummativeSubmission);
            await _db.SaveChangesAsync();
                       
            var sumSession = await _db.SummativeSessions.SingleOrDefaultAsync(p => p.Id == sumVM.SummativeSubmission.SummativeSessionId);

            if (sumSession.SASubmissionInOrder == null || sumSession.SASubmissionInOrder == "")
            {
                sumSession.SASubmissionInOrder = sumVM.SummativeSubmission.Id.ToString();
            }
            else
            {
                var finalOrder = sumSession.SASubmissionInOrder + "," + sumVM.SummativeSubmission.Id.ToString();
                sumSession.SASubmissionInOrder = finalOrder;
            }
                                 
            //var finalCount = correct + incorrect;
            //var FinalGrade = 100 / finalCount * correct;

            //sumSession.PercentageAchieved = FinalGrade;

            _db.Update(sumSession);
            await _db.SaveChangesAsync();
        
            return RedirectToAction("SummativeKnowledge", new { unitId = sumSession.UnitId });
        }
   

        public async Task<IActionResult> ViewResults(int sessionId)
        {
            var summativeSession = await _db.SummativeSessions.SingleOrDefaultAsync(p => p.Id == sessionId);
            var subList = await _db.SummativeSubmissions.Where(p => p.SummativeSessionId == sessionId).ToListAsync();

            var sumList = new List<Summative>();

            if (summativeSession != null && subList.Count() > 0)
            {

                foreach (var sub in subList)
                {
                    var summ = await _db.Summatives
                        .Include(p => p.MultipleChoice)
                        .Include(p => p.TrueFalse)
                        .Include(p => p.DirectQuestion)
                        .Include(p => p.Assignment)
                        .Include(p => p.Practical)
                        .SingleOrDefaultAsync(p => p.Id == sub.SummativeId);
                    sumList.Add(summ);
                }

                SummativeSubmissionViewModel sumVM = new SummativeSubmissionViewModel
                {

                    SubList = subList,
                    SumList = sumList,
                    Summative = new Summative(),
                    ProvisionalGrade = summativeSession.QuestionInProgressGrade,
                    FinalGrade = summativeSession.QuestionGradeTotal,
                    UnitId =  summativeSession.UnitId

                };

                return View(sumVM);
            }
            return NotFound();
        }
        
        public async Task<IActionResult> AssignmentSubmission(int assignId, int unitId) {

            var currentUser = await GetCurrentUserAsync();
            var assignment = await _db.Assignments.SingleOrDefaultAsync(p=>p.Id == assignId);
            var unit = await _db.CourseUnits.SingleOrDefaultAsync(p => p.Id == unitId);

            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);
            var summ = await _db.Summatives.Include(p=>p.Assignment).SingleOrDefaultAsync(p => p.AssignmentId == assignment.Id);

            var summativeSession = await _db.SummativeSessions
             .Where(p => p.UserId == currentUser.Id)
             .Where(p => p.UnitId == unitId)
             .Where(p => p.ClassEnrolId == classEnrol.Id).SingleOrDefaultAsync();

            var subValidity = await _db.SubmissionValidities.SingleOrDefaultAsync(p=>p.SummativeId == summ.Id);

            SummativeSession newSummative = new SummativeSession();
            SummativeSubmission summSub = new SummativeSubmission();

            SummativeSubmissionViewModel sumVM = new SummativeSubmissionViewModel
            {
                UnitId = unitId,
                Assignment = assignment,
                ClassEnrolId = classEnrol.Id, SubmissionValidity = new SubmissionValidity(),
                SummativeSession = new SummativeSession()
        };

            if (subValidity != null) {
            sumVM.SubmissionValidity = subValidity;
            }
            
                       
            if (summativeSession == null)
            {

                newSummative.ClassEnrolId = classEnrol.Id;
                newSummative.SAIdsInOrder = unit.SAOrder;
                newSummative.SAStart = DateTime.Now;
                newSummative.UnitId = unitId;
                newSummative.UserId = currentUser.Id; 


                _db.SummativeSessions.Add(newSummative);
                await _db.SaveChangesAsync();

                summSub.SummativeSessionId = newSummative.Id;
                summSub.UserId = currentUser.Id;
                summSub.SummativeId = summ.Id;

                sumVM.SummativeSubmission = summSub;


            }
            else
            {

                sumVM.SummativeSession = summativeSession;                          

                var sub = await _db.SummativeSubmissions.Include(p=>p.SummativeSession)
                    .Where(p=>p.UserId == currentUser.Id)
                    .Where(p => p.SummativeSession.Id == summativeSession.Id)
                    .SingleOrDefaultAsync(p=>p.SummativeId == summ.Id);

                if (sub != null && sub.AssignmentSubmission != null && sub.AssignmentSubmission != "")
                {

                    sumVM.SummativeSubmission = sub;

                }
                else {
                    summSub.SummativeSessionId = summativeSession.Id;
                    summSub.UserId = currentUser.Id;
                    summSub.SummativeId = summ.Id;

                    sumVM.SummativeSubmission = summSub;
                }
                    
            }

           


            return View(sumVM);
        }
        
        public async Task<IActionResult> PracticalSubmission(int practId, int unitId)
        {

            var currentUser = await GetCurrentUserAsync();
            var practical = await _db.Practicals.SingleOrDefaultAsync(p => p.Id == practId);
            var unit = await _db.CourseUnits.SingleOrDefaultAsync(p => p.Id == unitId);

            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);
            var summ = await _db.Summatives.Include(p => p.Practical).SingleOrDefaultAsync(p => p.PracticalId == practical.Id);

            var summativeSession = await _db.SummativeSessions
             .Where(p => p.UserId == currentUser.Id)
             .Where(p => p.UnitId == unitId)
             .Where(p => p.ClassEnrolId == classEnrol.Id).SingleOrDefaultAsync();

            var subValidity = await _db.SubmissionValidities.SingleOrDefaultAsync(p => p.SummativeId == summ.Id);

            SummativeSession newSummative = new SummativeSession();
            SummativeSubmission summSub = new SummativeSubmission();

            SummativeSubmissionViewModel sumVM = new SummativeSubmissionViewModel
            {
                UnitId = unitId,
                Practical = practical,
                ClassEnrolId = classEnrol.Id,
                SubmissionValidity = new SubmissionValidity(),
                SummativeSession = new SummativeSession()
            };

            if (subValidity != null)
            {
                sumVM.SubmissionValidity = subValidity;
            }

            if (summativeSession == null)
            {

                newSummative.ClassEnrolId = classEnrol.Id;
                newSummative.SAIdsInOrder = unit.SAOrder;
                newSummative.SAStart = DateTime.Now;
                newSummative.UnitId = unitId;
                newSummative.UserId = currentUser.Id;

                _db.SummativeSessions.Add(newSummative);
                await _db.SaveChangesAsync();

                summSub.SummativeSessionId = newSummative.Id;
                summSub.UserId = currentUser.Id;
                summSub.SummativeId = summ.Id;

                sumVM.SummativeSubmission = summSub;

            }
            else
            {
                sumVM.SummativeSession = summativeSession;

                var sub = await _db.SummativeSubmissions.Include(p => p.SummativeSession)
                    .Where(p => p.UserId == currentUser.Id)
                    .Where(p => p.SummativeSession.Id == summativeSession.Id)
                    .SingleOrDefaultAsync(p => p.SummativeId == summ.Id);

                if (sub != null && sub.PracticalSubmission != null && sub.PracticalSubmission != "")
                {
                    sumVM.SummativeSubmission = sub;
                }
                else
                {
                    summSub.SummativeSessionId = summativeSession.Id;
                    summSub.UserId = currentUser.Id;
                    summSub.SummativeId = summ.Id;

                    sumVM.SummativeSubmission = summSub;
                }
            }
            return View(sumVM);
        }

        public IActionResult GetPDF(string pdf)
        {
            string webRootPath = _hosting.WebRootPath;

            return PhysicalFile(webRootPath + pdf, "application/pdf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPractical(SummativeSubmissionViewModel sumVM, int subId, int sesId, int practId)
        {
            //Check to see if there is allready a file submitted for this practical.
            var currentUser = await GetCurrentUserAsync();

            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);
            var summ = await _db.Summatives.SingleOrDefaultAsync(p => p.PracticalId == practId);

            var sub = await _db.SummativeSubmissions.SingleOrDefaultAsync(p => p.Id == subId);

            var sumSub = new SummativeSubmission();

            if (ModelState.IsValid)
            {
                //Processing File
                string webRootPath = _hosting.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if (files.Count() > 0)
                {
                    if (sub == null)
                    {
                        sumSub.SummativeId = summ.Id;
                        sumSub.SummativeSessionId = sesId;
                        sumSub.UserId = currentUser.Id;

                        _db.Add(sumSub);
                        await _db.SaveChangesAsync();
                    }

                    //Files was uploaded
                    var uploads = Path.Combine(webRootPath, "submissions\\practicals" + "\\" + classEnrol.Id);
                    var extention = Path.GetExtension(files[0].FileName);

                    Directory.CreateDirectory(uploads);

                    using (var filesStream = new FileStream(Path.Combine(uploads, sumSub.Id + extention), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }

                    sumSub.PracticalSubmission = @"\submissions\practicals\" + classEnrol.Id + @"\" + sumSub.Id + extention;
                    _db.Update(sumSub);

                }
                else
                {
                    ModelState.AddModelError("files", "No File is submitted. Please select a file to submit.");
                }

                var sumSession = await _db.SummativeSessions.SingleOrDefaultAsync(p => p.Id == sesId);

                if (sumSession.SASubmissionInOrder == null || sumSession.SASubmissionInOrder == "")
                {
                    sumSession.SASubmissionInOrder = sumSub.Id.ToString();
                }
                else
                {
                    var finalOrder = sumSession.SASubmissionInOrder + "," + sumSub.Id.ToString();
                    sumSession.SASubmissionInOrder = finalOrder;
                }

                if (classEnrol.CompeletedSas != null && classEnrol.CompeletedSas != "")
                {
                    var saList = OrderList.ItemIdOrder(classEnrol.CompeletedSas);

                    if (!saList.Contains(summ.Id))
                    {
                        //Adding SA to completed SA List
                        var finalOrder = String.Concat(classEnrol.CompeletedSas + "," + summ.Id.ToString());
                        classEnrol.CompeletedSas = finalOrder;
                    }
                }
                else
                {
                    classEnrol.CompeletedSas = summ.Id.ToString();
                }
                _db.Update(classEnrol);
                _db.Update(sumSession);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("PracticalSubmission", "SummativeSession", new { practId = practId, unitId = summ.CourseUnitId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAssignment(SummativeSubmissionViewModel sumVM, int subId, int sesId, int assignId)
        {
              //Check to see if there is allready a file submitted for this assignment.
                var currentUser = await GetCurrentUserAsync();
               
                var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);
                var summ = await _db.Summatives.SingleOrDefaultAsync(p => p.AssignmentId == assignId);

            var sub = await _db.SummativeSubmissions.SingleOrDefaultAsync(p=>p.Id == subId);

            var sumSub = new SummativeSubmission();


            if (ModelState.IsValid)
            {
  
                    //Processing File

                    string webRootPath = _hosting.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
         

                if (files.Count() > 0)
                    {


                    if (sub == null)
                    {

                        sumSub.SummativeId = summ.Id;
                        sumSub.SummativeSessionId = sesId;
                        sumSub.UserId = currentUser.Id;

                        _db.Add(sumSub);
                        await _db.SaveChangesAsync();
                    }

                    

                    //Files was uploaded
                    
                    var uploads = Path.Combine(webRootPath, "submissions\\assignments" + "\\" + classEnrol.Id);
                        var extention = Path.GetExtension(files[0].FileName);

                    Directory.CreateDirectory(uploads);

                    using (var filesStream = new FileStream(Path.Combine(uploads, sumSub.Id + extention), FileMode.Create))
                        {
                            files[0].CopyTo(filesStream);
                        }

                        sumSub.AssignmentSubmission = @"\submissions\assignments\" + classEnrol.Id + @"\" + sumSub.Id + extention;
                        _db.Update(sumSub);

                    }
                    else
                    {
                        ModelState.AddModelError("files", "No File is submitted. Please select a file to submit.");

                    }

                    var sumSession = await _db.SummativeSessions.SingleOrDefaultAsync(p => p.Id == sesId);

                    if (sumSession.SASubmissionInOrder == null || sumSession.SASubmissionInOrder == "")
                    {
                        sumSession.SASubmissionInOrder = sumSub.Id.ToString();
                    }
                    else
                    {
                        var finalOrder = sumSession.SASubmissionInOrder + "," + sumSub.Id.ToString();
                        sumSession.SASubmissionInOrder = finalOrder;
                    }

                if (classEnrol.CompeletedSas != null && classEnrol.CompeletedSas != "")
                {
                    var saList = OrderList.ItemIdOrder(classEnrol.CompeletedSas);

                    if (!saList.Contains(summ.Id))
                    {
                        //Adding SA to completed SA List
                        var finalOrder = String.Concat(classEnrol.CompeletedSas + "," + summ.Id.ToString());
                        classEnrol.CompeletedSas = finalOrder;

                    }

                }
                else
                {

                    classEnrol.CompeletedSas = summ.Id.ToString();

                }

                _db.Update(classEnrol);
                _db.Update(sumSession);
                await _db.SaveChangesAsync();

                
               

         }
            return RedirectToAction("AssignmentSubmission", "SummativeSession", new { assignId = assignId, unitId = summ.CourseUnitId });
            
        }

        public async Task<IActionResult> DeleteAssignment(int subId){

            var currentUser = await GetCurrentUserAsync();

            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);
            var sub = await _db.SummativeSubmissions.Include(p=>p.Summative).SingleOrDefaultAsync(p => p.Id == subId);

            var unitId = sub.Summative.CourseUnitId;
            var assignId = sub.Summative.AssignmentId;

            if (sub == null) {

                return NotFound("Delete Action Not Permitted");
            
            }

            var session = await _db.SummativeSessions.SingleOrDefaultAsync(p=>p.Id == sub.SummativeSessionId);
            
            var currentSAOrder = classEnrol.CompeletedSas;
            var currentSubOrder = session.SASubmissionInOrder;

            classEnrol.CompeletedSas = RePosition.Delete(currentSAOrder, sub.SummativeId);
            session.SASubmissionInOrder = RePosition.Delete(currentSubOrder, sub.Id);

            _db.Update(classEnrol);
            _db.Update(session);
            

            _db.SummativeSubmissions.Remove(sub);
            await _db.SaveChangesAsync();

            string webRootPath = _hosting.WebRootPath;

                var assPath = Path.Combine(webRootPath, sub.AssignmentSubmission.TrimStart('\\'));

                // Check if exists then delete

                if (System.IO.File.Exists(assPath))
                {
                    System.IO.File.Delete(assPath);
                }
            

          

            return RedirectToAction("AssignmentSubmission", "SummativeSession", new { assignId = assignId, unitId = unitId });

        }

        public async Task<IActionResult> DeletePractical(int subId)
        {

            var currentUser = await GetCurrentUserAsync();

            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);
            var sub = await _db.SummativeSubmissions.Include(p => p.Summative).SingleOrDefaultAsync(p => p.Id == subId);

            var unitId = sub.Summative.CourseUnitId;
            var practId = sub.Summative.PracticalId;

            if (sub == null)
            {

                return NotFound("Delete Action Not Permitted");

            }

            var session = await _db.SummativeSessions.SingleOrDefaultAsync(p => p.Id == sub.SummativeSessionId);

            var currentSAOrder = classEnrol.CompeletedSas;
            var currentSubOrder = session.SASubmissionInOrder;

            classEnrol.CompeletedSas = RePosition.Delete(currentSAOrder, sub.SummativeId);
            session.SASubmissionInOrder = RePosition.Delete(currentSubOrder, sub.Id);

            _db.Update(classEnrol);
            _db.Update(session);
            _db.SummativeSubmissions.Remove(sub);


            string webRootPath = _hosting.WebRootPath;

            var pracPath = Path.Combine(webRootPath, sub.PracticalSubmission.TrimStart('\\'));

            // Check if exists then delete

            if (System.IO.File.Exists(pracPath))
            {
                System.IO.File.Delete(pracPath);
            }


            await _db.SaveChangesAsync();

            return RedirectToAction("PracticalSubmission", "SummativeSession", new { practId = practId, unitId = unitId });

        }


    }
}