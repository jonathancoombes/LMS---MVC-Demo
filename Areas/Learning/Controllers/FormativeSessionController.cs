using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Extentions;
using LMS.Models;
using LMS.Models.Assessment;
using LMS.Models.Learning;
using LMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Utility;

namespace LMS.Areas.Learning.Controllers
{
    [Area("Learning")]
    [Authorize(Roles = "Administrator,Learner")]
    public class FormativeSessionController : Controller
    {

        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        public FormativeSessionController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Formatives(int topicId)
        {

            var topic = await _db.CourseTopics.SingleOrDefaultAsync(p => p.Id == topicId);
            var currentUser = await GetCurrentUserAsync();

            var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);

            if (topic.FAOrder != "" && topic.FAOrder != null)
            {
                var formatives = OrderList.ItemIdOrder(topic.FAOrder);
                var foremainingFormatives = new List<Formative>();

                foreach (var form in formatives)
                {
                    var formativeFromDB = await _db.Formatives.Include(p => p.MultipleChoice).Include(p => p.TrueFalse).SingleOrDefaultAsync(p => p.Id == form);
                    foremainingFormatives.Add(formativeFromDB);
                }

                var formativeSession = await _db.FormativeSessions
                    .Where(p => p.UserId == currentUser.Id)
                    .Where(p => p.TopicId == topicId)
                    .Where(p => p.ClassEnrolId == classEnrol.Id).SingleOrDefaultAsync();


                FormativeSession newFormative = new FormativeSession();

                if (formativeSession == null)
                {

                    newFormative.ClassEnrolId = classEnrol.Id;
                    newFormative.FAQuestionIdsInOrder = topic.FAOrder;
                    newFormative.FAStart = DateTime.Now;
                    newFormative.TopicId = topicId;
                    newFormative.UserId = currentUser.Id;


                    _db.FormativeSessions.Add(newFormative);
                    _db.SaveChanges();
                }
                else {
                    newFormative = formativeSession;
                }
               


                //check what formatives are completed, if any move on to next
                //set formative in viewModel below equal to the next formative to be completed and send through to view

                FormativeSubmissionViewModel formVM = new FormativeSubmissionViewModel
                {
                   ClassEnrolId = newFormative.ClassEnrolId, TopicId = topicId,
                    FormativeSubmission = new FormativeSubmission
                    {
                        FormativeSessionId = newFormative.Id,
                        UserId = currentUser.Id    


                    }
                };

                //If there are no completed FA's, set the FA to the first on the list 

                if (classEnrol.CompletedFas == null || classEnrol.CompletedFas == "")
                {
                    formVM.FormativeSubmission.FormativeId = foremainingFormatives[0].Id;

                    if (foremainingFormatives[0].QuestionType == SD.MultipleChoice)
                    {
                        formVM.QuestionType = SD.MultipleChoice;
                        formVM.MultipleChoice = await _db.MultipleChoices.SingleOrDefaultAsync(p => p.Id == foremainingFormatives[0].MultipleChoiceId);
                    }
                    else if (foremainingFormatives[0].QuestionType == SD.TrueFalse)
                    {
                        formVM.QuestionType = SD.TrueFalse;
                        formVM.TrueFalse = await _db.TrueFalses.SingleOrDefaultAsync(p => p.Id == foremainingFormatives[0].TrueFalseId);
                    }
                    formVM.QuestionCount = foremainingFormatives.Count();

                    return View(formVM);
                }
                //If there are FA's completed, remove them from the list..Then set the FA to the first on the new list
                else 
                {
                    var completedFormativeIds = OrderList.ItemIdOrder(classEnrol.CompletedFas);
                    var completedFormatives = new List<Formative>();

                    //Create list of completed formatives
                    foreach (var form in completedFormativeIds)
                    {
                        var formativeFromDB = await _db.Formatives.Include(p => p.MultipleChoice).Include(p => p.TrueFalse).SingleOrDefaultAsync(p => p.Id == form);
                        completedFormatives.Add(formativeFromDB);
                    }

                    var remainingFormatives = new List<Formative>();

                    foreach (var forma in foremainingFormatives)
                    {

                        if (!completedFormatives.Contains(forma))
                        {
                            remainingFormatives.Add(forma);
                        };

                    }

                    //Setting the formative to the first on the new list generated from what's not comepleted in the ComepletedFA table

                    if (remainingFormatives.Count() > 0 && remainingFormatives != null)
                    {

                        formVM.FormativeSubmission.FormativeId = remainingFormatives[0].Id;


                        if (remainingFormatives[0].QuestionType == SD.MultipleChoice)
                        {
                            formVM.MultipleChoice = await _db.MultipleChoices.SingleOrDefaultAsync(p => p.Id == remainingFormatives[0].MultipleChoiceId);
                            formVM.QuestionType = SD.MultipleChoice;
                        }
                        else if (remainingFormatives[0].QuestionType == SD.TrueFalse)
                        {
                            formVM.TrueFalse = await _db.TrueFalses.SingleOrDefaultAsync(p => p.Id == remainingFormatives[0].TrueFalseId);
                            formVM.QuestionType = SD.TrueFalse;
                        }

                    }
                    formVM.QuestionCount = remainingFormatives.Count();

                    if (remainingFormatives.Count() == 0) {
                       var faSession = await _db.FormativeSessions.SingleOrDefaultAsync(p=>p.Id == formVM.FormativeSubmission.FormativeSessionId);
                        formVM.FinalGrade = faSession.PercentageAchieved;
                    }

                    return View(formVM);
                }

            }
      
                return NotFound();

          

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CaptureFormative(FormativeSubmissionViewModel formVM) {


            if (!ModelState.IsValid) {

                return View("Formatives", formVM);
            }
            
            _db.Add(formVM.FormativeSubmission);
            await _db.SaveChangesAsync();

            var currentUser = await GetCurrentUserAsync();

            //Adding the FA Submission to the FormativeSession Table

            var formSession = await _db.FormativeSessions.SingleOrDefaultAsync(p => p.Id == formVM.FormativeSubmission.FormativeSessionId);

            if (formSession.FASubmissionInOrder == null || formSession.FASubmissionInOrder == "")
            {
                formSession.FASubmissionInOrder = formVM.FormativeSubmission.Id.ToString();
            }
            else {
                var finalOrder = formSession.FASubmissionInOrder + "," + formVM.FormativeSubmission.Id.ToString();
                formSession.FASubmissionInOrder = finalOrder;
            }

            //Check if all FA's are answered

            var faQuestions = OrderList.ItemIdOrder(formSession.FAQuestionIdsInOrder);
            var faSubmissions = OrderList.ItemIdOrder(formSession.FASubmissionInOrder);
            var faSubmissionsList = new List<FormativeSubmission>();

            //Creating a list of submissions
            foreach (var sub in faSubmissions) {
            var subMission = await _db.FormativeSubmissions.SingleOrDefaultAsync(p=>p.Id == sub);
                faSubmissionsList.Add(subMission);
            }

            //Check each submission's FA Id, if it is in the FAQuestionslist, remove it
            foreach (var submission in faSubmissionsList) {
                if (faQuestions.Contains(submission.FormativeId)) {
                    faQuestions.Remove(submission.FormativeId);
                }
            }

            //Setting the EndDate
            if (faQuestions == null || faQuestions.Count() < 1) {

                formSession.FAEnd = DateTime.Now;


                //Calculating Results
                //1.Getting List of Questions ready
                var questList = OrderList.ItemIdOrder(formSession.FAQuestionIdsInOrder);
                var formQuest = new List<Formative>();

                foreach (var ques in questList)
                {
                    var addFromDb = await _db.Formatives.Include(p => p.MultipleChoice).Include(p => p.TrueFalse).SingleOrDefaultAsync(p => p.Id == ques);
                    formQuest.Add(addFromDb);
                }

                int correct = 0;
                int incorrect = 0;

                foreach (var question in formQuest)
                {
                    var formSub = await _db.FormativeSubmissions.Where(p => p.FormativeId == question.Id).Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync();

                    if (question.QuestionType == SD.MultipleChoice) {


                        if (question.MultipleChoice.CorrectAnswer.ToString() == formSub.MCAnswer)
                        {

                            if (formSession.FAGradesInOrder == null || formSession.FAGradesInOrder == "")
                            {
                                formSession.FAGradesInOrder = SD.Competent;
                            }
                            else {
                                formSession.FAGradesInOrder = formSession.FAGradesInOrder + "," + SD.Competent;                                 
                            }

                            formSub.Result = SD.Competent;

                            correct++;
                        }
                        else {
                            if (formSession.FAGradesInOrder == null || formSession.FAGradesInOrder == "")
                            {
                                formSession.FAGradesInOrder = SD.NotYetCompetent;
                            }
                            else
                            {
                                formSession.FAGradesInOrder = formSession.FAGradesInOrder + "," + SD.NotYetCompetent;
                            }
                            formSub.Result = SD.NotYetCompetent;

                            incorrect++;    
                        }
                       
                    }
                    else if (question.QuestionType == SD.TrueFalse) {

                        
                        if (question.TrueFalse.CorrectAnswer.ToString() == formSub.TFAnswer)
                        {

                            if (formSession.FAGradesInOrder == null || formSession.FAGradesInOrder == "")
                            {
                                formSession.FAGradesInOrder = SD.Competent;
                            }
                            else
                            {
                                formSession.FAGradesInOrder = formSession.FAGradesInOrder + "," + SD.Competent;
                            }
                            formSub.Result = SD.Competent;

                            correct++;
                        }
                        else
                        {
                            if (formSession.FAGradesInOrder == null || formSession.FAGradesInOrder == "")
                            {
                                formSession.FAGradesInOrder = SD.NotYetCompetent;
                            }
                            else
                            {
                                formSession.FAGradesInOrder = formSession.FAGradesInOrder + "," + SD.NotYetCompetent;
                            }
                            formSub.Result = SD.NotYetCompetent;

                            incorrect++;
                        }

                    }
                }

                //Calculating Final Grade

                var finalCount = correct + incorrect;
                var FinalGrade = 100 / finalCount * correct;
                var result = FinalGrade == 99 ? 100 : FinalGrade;
                formSession.PercentageAchieved = result;

                 _db.Update(formVM.FormativeSubmission);

                
                //Moving Topic to next
                var classEnrol = await _db.ClassEnrolments.Where(p => p.UserId == currentUser.Id).SingleOrDefaultAsync(p => p.Id == currentUser.ActiveClassId);

                if (classEnrol.CompletedTopics != null && classEnrol.CompletedTopics != "")
                {
                    var topicList = OrderList.ItemIdOrder(classEnrol.CompletedTopics);

                    if (!topicList.Contains(formVM.TopicId))
                    {
                        //Adding Topic to completed Topic List
                        var finalOrder = String.Concat(classEnrol.CompletedTopics + "," + formVM.TopicId.ToString());
                        classEnrol.CompletedTopics = finalOrder;
                        classEnrol.CurrentPage = 1;
                    }

                }
                else
                {

                    classEnrol.CompletedTopics = formVM.TopicId.ToString();
                    classEnrol.CurrentPage = 1;
                }

                //Setting Current Topic
                var topic = await _db.CourseTopics.Include(p => p.CourseUnit).SingleOrDefaultAsync(p => p.Id == formVM.TopicId);
                var orderedTopics = OrderList.ItemIdOrder(topic.CourseUnit.CourseTopicIds);

                var currentPosition = orderedTopics.BinarySearch(topic.Id);

                if (currentPosition + 1 != orderedTopics.Count())
                {

                    classEnrol.CurrentTopicId = orderedTopics[currentPosition + 1];

                }

                _db.Update(classEnrol);
               
            }



            _db.Update(formSession);
            await _db.SaveChangesAsync();


            return RedirectToAction("Formatives", new { topicId = formSession.TopicId }); 
        }





        public async Task<IActionResult> ViewResults(int sessionId)
        {

            var formativeSession = await _db.FormativeSessions.SingleOrDefaultAsync(p=>p.Id == sessionId);
            var subList = await _db.FormativeSubmissions.Where(p => p.FormativeSessionId == sessionId).ToListAsync();

            var formList = new List<Formative>();

            if (formativeSession != null && subList.Count() > 0) {

                foreach (var sub in subList) {
                            var form = await _db.Formatives.Include(p => p.MultipleChoice).Include(p => p.TrueFalse).SingleOrDefaultAsync(p=>p.Id == sub.FormativeId);
                            formList.Add(form);  
                        }

                FormativeSubmissionViewModel formVM = new FormativeSubmissionViewModel {

                     SubList = subList, 
                     FormList = formList,
                    Formative = new Formative(),
                    FinalGrade = formativeSession.PercentageAchieved

                };

                        return View(formVM);


            }

            return NotFound();
        }


    }
}