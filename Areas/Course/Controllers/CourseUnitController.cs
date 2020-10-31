using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Models.Course;
using LMS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Extentions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using LMS.Models.Assessment;
using LMS.Utility;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using LMS.Models;
using Microsoft.AspNetCore.Identity;

namespace LMS.Areas.Course.Controllers
{

    [Area("Course")]
    [Authorize(Roles = "Administrator,Designer")]
    public class CourseUnitController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IHostingEnvironment _hostingEnvironment;
        private UserManager<ApplicationUser> _userManager;


        public CourseUnitController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET - CourseModules
        public async Task<IActionResult> GetCourseModules(int id)
        {

            List<CourseModule> courseModules = new List<CourseModule>();

            courseModules = await (from courseModule in _db.CourseModules
                                   where courseModule.CourseId == id
                                   select courseModule).ToListAsync();
            return Json(new SelectList(courseModules, "Id", "Title"));

        }


        // GET - CourseModules
        public async Task<IActionResult> GetCourseUnits(int id)
        {

            List<CourseUnit> courseUnits = new List<CourseUnit>();


            var currentModuleUnitAssignments = await _db.CourseUnitAssignments.Where(c => c.CourseModuleId == id).ToListAsync();

            if (currentModuleUnitAssignments != null)
            {

                foreach (var item in currentModuleUnitAssignments)
                {

                    courseUnits.Add(await _db.CourseUnits.Where(p => p.Id == item.CourseUnitId).SingleOrDefaultAsync());
                }
            }

            return Json(new SelectList(courseUnits, "Id", "Name"));

        }

        //GET
        public async Task<IActionResult> Index()
        {

            return View(await _db.Courses.ToListAsync());
        }
        //GET
        public async Task<IActionResult> UnitList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
         

            CourseUnitViewModel model = new CourseUnitViewModel
            {
                Course = await _db.Courses.SingleOrDefaultAsync(c => c.Id == id),
                CourseUnit = new CourseUnit(),
                CourseUnitAssignments = await _db.CourseUnitAssignments.Where(c => c.CourseId == id).ToListAsync()
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

       


        //GET - Create
        public async Task<IActionResult> Create(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            CourseUnitViewModel model = new CourseUnitViewModel
            {
                Course = await _db.Courses.SingleOrDefaultAsync(c => c.Id == id),
                CourseUnit = new CourseUnit(),
                CourseModuleList = await _db.CourseModules.Where(c => c.CourseId == id).ToListAsync(),
                CourseUnitList = await _db.CourseUnits.ToListAsync(),
                CourseUnitAssignment = new CourseUnitAssignment(

                    )

            };


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseUnitViewModel courseUnitVM, int Id)
        {

            if (ModelState.IsValid)
            {

                //Check to see if there is allready a unit with the same name

                var UnitExists = _db.CourseUnits.Where(s => s.Name == courseUnitVM.CourseUnit.Name);

                // If it does, oops:

                if (UnitExists.Count() > 0)
                {
                    //ERROR OCURRED
                }

                //Otherwise, all is well so let's go:

                else
                {

                    courseUnitVM.Course.Id = Id;

                    //1 - Adding the new Unit to DB

                    _db.CourseUnits.Add(courseUnitVM.CourseUnit);

                    await _db.SaveChangesAsync();


                    //2 - Update Unit Order in Module Table

                    int unitId = courseUnitVM.CourseUnit.Id;
                    int moduleId = courseUnitVM.CourseUnitAssignment.CourseModuleId;

                    var currentModule = await _db.CourseModules.Where(c => c.Id == moduleId).SingleOrDefaultAsync();
                    var currentUnitOrder = currentModule.CourseUnitOrder;


                    if (currentUnitOrder == null || currentUnitOrder == "")
                    {

                        currentModule.CourseUnitOrder = unitId.ToString();

                        _db.Update(currentModule);

                        await _db.SaveChangesAsync();

                    }
                    else if (currentUnitOrder != null)
                    {

                        var finalOrder = String.Concat(currentUnitOrder + "," + unitId.ToString());
                        currentModule.CourseUnitOrder = finalOrder;

                        _db.Update(currentModule);

                        await _db.SaveChangesAsync();


                    }
                    //3 - Create Unit Assignment 
                    courseUnitVM.CourseUnitAssignment.CourseId = Id;
                    courseUnitVM.CourseUnitAssignment.CourseUnitId = unitId;
                    courseUnitVM.CourseUnitAssignment.CourseModuleId = moduleId;

                    _db.CourseUnitAssignments.Add(courseUnitVM.CourseUnitAssignment);

                    await _db.SaveChangesAsync();


                    return RedirectToAction("UnitList", "CourseUnit", new { id = Id });

                }

            }
            //If the ModelState is not valid

            CourseUnitViewModel modelVM = new CourseUnitViewModel
            {
                Course = await _db.Courses.SingleOrDefaultAsync(c => c.Id == courseUnitVM.Course.Id),
                CourseUnit = courseUnitVM.CourseUnit,
                CourseModuleList = await _db.CourseModules.Where(c => c.CourseId == Id).ToListAsync(),
                CourseUnitList = await _db.CourseUnits.ToListAsync(),
                CourseUnitAssignment = new CourseUnitAssignment()
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
                var unit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == id);

                if (unit == null)
                {

                    return NotFound();
                }
                else
                {
                    // !! This only addresses a single occurence of a Unit being assigned !! If the same unit is assigned / copied (possible later functionality) this should be altered to check for which course it is assigned to


                    var courseAssignment = await _db.CourseUnitAssignments.SingleOrDefaultAsync(p => p.CourseUnit.Id == id);
                    var courseId = courseAssignment.CourseId;

                    CourseUnitViewModel courseUnitVM = new CourseUnitViewModel
                    {

                        Course = await _db.Courses.SingleOrDefaultAsync(c => c.Id == courseId),
                        CourseUnit = unit,
                        CourseModuleList = await _db.CourseModules.Where(c => c.CourseId == courseId).ToListAsync(),
                        CourseUnitAssignment = courseAssignment

                    };
                    return View(courseUnitVM);
                }
            }
        }
        //EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseUnit courseUnit, int courseId, int unitId, string topicOrder)
        {

            if (ModelState.IsValid)
            {
                courseUnit.Id = unitId;
                courseUnit.CourseTopicIds = topicOrder;
                _db.Update(courseUnit);

                await _db.SaveChangesAsync();

                return RedirectToAction("UnitList", "CourseUnit", new { id = courseId });

            }

            return View(courseUnit);

        }


        //GET - DELETE

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var unit = await _db.CourseUnits.FindAsync(id);

            if (unit == null)
            {
                return NotFound();
            }

            return View(unit);

        }


        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == id);

            // !! This only addresses a single occurence of a Unit being assigned !! If the same unit is assigned / copied (possible later functionality) this should be altered to check for which course it is assigned to

            var courseAssignment = await _db.CourseUnitAssignments.SingleOrDefaultAsync(p => p.CourseUnit.Id == id);
            var courseId = courseAssignment.CourseId;
            var moduleId = courseAssignment.CourseModuleId;

            var currentModule = await _db.CourseModules.SingleOrDefaultAsync(c => c.Id == moduleId);
            var currentUnitOrder = currentModule.CourseUnitOrder;

            if (currentUnitOrder == null || currentUnitOrder == "" || !OrderList.ItemIdOrder(currentUnitOrder).Contains(id))
            {
                return NotFound();
            }

            currentModule.CourseUnitOrder = RePosition.Delete(currentUnitOrder, id);

            _db.CourseUnitAssignments.Remove(courseAssignment);
            _db.CourseUnits.Remove(currentUnit);
            _db.Update(currentModule);

            await _db.SaveChangesAsync();

            return RedirectToAction("UnitList", "CourseUnit", new { id = courseId });


        }


        public async Task<IActionResult> MoveUnitUp(int Id)
        {
            var currentUnit = await _db.CourseUnits.SingleOrDefaultAsync(c => c.Id == Id);

            // !! This only addresses a single occurence of a Unit being assigned !! If the same unit is assigned / copied (possible later functionality) this should be altered to check for which course it is assigned to

            var courseAssignment = await _db.CourseUnitAssignments.SingleOrDefaultAsync(p => p.CourseUnit.Id == Id);
            var courseId = courseAssignment.CourseId;

            var currentCourse = await _db.Courses.SingleOrDefaultAsync(c => c.Id == courseId);

            var currentModule = await _db.CourseModules.SingleOrDefaultAsync(p => p.Id == courseAssignment.CourseModuleId);
            var currentUnitOrder = currentModule.CourseUnitOrder;


            if (RePosition.CanMoveUpCheck(currentUnitOrder, Id))
            {
                currentModule.CourseUnitOrder = RePosition.MoveUp(currentUnitOrder, Id);
                _db.Update(currentModule);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("UnitList", "CourseUnit", new { id = courseId });
        }

        public async Task<IActionResult> MoveUnitDown(int Id)
        {
            var courseAssignment = await _db.CourseUnitAssignments.SingleOrDefaultAsync(p => p.CourseUnit.Id == Id);
            var courseId = courseAssignment.CourseId;

            var currentCourse = await _db.Courses.SingleOrDefaultAsync(c => c.Id == courseId);

            var currentModule = await _db.CourseModules.SingleOrDefaultAsync(p => p.Id == courseAssignment.CourseModuleId);
            var currentUnitOrder = currentModule.CourseUnitOrder;

            if (RePosition.CanMoveDownCheck(currentUnitOrder, Id))
            {
                currentModule.CourseUnitOrder = RePosition.MoveDown(currentUnitOrder, Id);
                _db.Update(currentModule);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("UnitList", "CourseUnit", new { id = courseId });
        }

        // Selecting the Course
        public async Task<IActionResult> CopySelectCourse(int sourceCourseId)
        {

            var sourceCourse = await _db.Courses.SingleOrDefaultAsync(p => p.Id == sourceCourseId);

            //List of all Courses except destcourse
            var allCourses = await _db.Courses.Where(p => p.Id != sourceCourseId).ToListAsync();

            CourseUnitViewModel model = new CourseUnitViewModel
            {
                CourseList = allCourses,
                CopyToCourseId = sourceCourseId,
                Courses = new SelectList(allCourses, "Id", "Name") { }
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertUnit(int courseId, int destModuleId, int unitId)
        {

            //If anay parameter = 0, return to form
            if (courseId == 0 || destModuleId == 0 || unitId == 0)
            {

                ModelState.AddModelError("sourceCourseId", "PLease select from all the required options");
                return RedirectToAction("CopySelectCourse", "CourseUnit", new { sourceCourseId = courseId });

            }

            var destCourse = await _db.Courses.SingleOrDefaultAsync(p => p.Id == courseId);
            var destModule = await _db.CourseModules.SingleOrDefaultAsync(p => p.Id == destModuleId);
            var unit = await _db.CourseUnits.SingleOrDefaultAsync(p => p.Id == unitId);

            var topicForm = new Dictionary<CourseTopic, List<Formative>>();

            //Creating a Dictionary of Pairs per: Topic|FormativeList
            if (unit.CourseTopicIds != null && unit.CourseTopicIds != "")
            {
                var unitTopicsInOrder = OrderList.ItemIdOrder(unit.CourseTopicIds);

                foreach (var topic in unitTopicsInOrder)
                {
                    var addTopic = await _db.CourseTopics.SingleOrDefaultAsync(p => p.Id == topic);
                    var formList = await _db.Formatives
                        .Include(p => p.MultipleChoice)
                        .Include(p => p.TrueFalse)
                        .Where(p => p.CourseTopicId == topic).ToListAsync();

                    topicForm.Add(addTopic, formList);
                }
            }

            //List of Summatives In Order                   
            var SaList = new List<Summative>();
            if (unit.SAOrder != null && unit.SAOrder != "")
            {

                var unitSAInOrder = OrderList.ItemIdOrder(unit.SAOrder);


                foreach (var sum in unitSAInOrder)
                {
                    var unitSummative = await _db.Summatives
                    .Include(p => p.DirectQuestion)
                    .Include(p => p.MultipleChoice)
                    .Include(p => p.TrueFalse)
                    .Include(p => p.Practical)
                    .Include(p => p.Assignment)
                    .Where(p => p.Id == sum).SingleOrDefaultAsync();

                    SaList.Add(unitSummative);

                }
            }
            
            // 1 - Adding Unit
            var newUnit = new CourseUnit()
            {
                CourseTopicIds = unit.CourseTopicIds,
                Credits = unit.Credits,
                Level = unit.Level,
                Name = unit.Name,
                Reference = unit.Reference,
                SAOrder = unit.SAOrder
            };
            _db.CourseUnits.Add(newUnit);
            await _db.SaveChangesAsync();


            // 2- Add CourseUnitAssignment
            var newUnitAss = new CourseUnitAssignment()
            {
                CourseId = courseId,
                CourseModuleId = destModuleId,
                CourseUnitId = newUnit.Id
            };

            _db.CourseUnitAssignments.Add(newUnitAss);

            // 3 - Add Unit to Module Unit Order
            var currentUnitOrder = destModule.CourseUnitOrder;

            if (currentUnitOrder == null || currentUnitOrder == "")
            {
                destModule.CourseUnitOrder = newUnit.Id.ToString();
                _db.Update(destModule);
                await _db.SaveChangesAsync();

            }
            else if (currentUnitOrder != null)
            {
                var finalOrder = String.Concat(currentUnitOrder + "," + newUnit.Id.ToString());
                destModule.CourseUnitOrder = finalOrder;
                _db.Update(destModule);
            }

            // Preparing for File Copying

            var practical = new List<Practical>();
            var assignment = new List<Assignment>();
            string webRootPath = _hostingEnvironment.WebRootPath;

            // 4 - Add the Summatives for Unit
            var newSaOrder = "";

            if (SaList != null && SaList.Count > 0)
            {
                foreach (var summ in SaList)
                {
                    var newSum = new Summative() { Title = summ.Title, Reference = summ.Reference, CourseUnitId = newUnit.Id, Weight = summ.Weight };

                    switch (summ.AssessmentType)
                    {
                        case SD.DirectQuestion:
                            var dir = await _db.DirectQuestions.SingleOrDefaultAsync(p => p.Id == summ.DirectQuestionId);
                            var newDir = new DirectQuestion() { Question = dir.Question, AnswerGuide = dir.AnswerGuide };
                            _db.DirectQuestions.Add(newDir);

                            newSum.AssessmentType = SD.DirectQuestion;
                            newSum.DirectQuestionId = newDir.Id;
                            _db.Summatives.Add(newSum);
                            await _db.SaveChangesAsync();

                            if (newSaOrder != "" && newSaOrder != null)
                            {
                                newSaOrder = newSaOrder + "," + newSum.Id;
                            }
                            else
                            {
                                newSaOrder = newSum.Id.ToString();
                            }
                            break;

                        case SD.MultipleChoice:
                            var mc = await _db.MultipleChoices.SingleOrDefaultAsync(p => p.Id == summ.MultipleChoiceId);
                            var newMc = new MultipleChoice()
                            {
                                Question = mc.Question,
                                AnswerA = mc.AnswerA,
                                AnswerB = mc.AnswerB,
                                AnswerC = mc.AnswerC,
                                AnswerD = mc.AnswerD,
                                CorrectAnswer = mc.CorrectAnswer
                            };
                            _db.MultipleChoices.Add(newMc);

                            newSum.AssessmentType = SD.MultipleChoice;
                            newSum.MultipleChoiceId = newMc.Id;
                            _db.Summatives.Add(newSum);

                            await _db.SaveChangesAsync();

                            if (newSaOrder != "" && newSaOrder != null)
                            {
                                newSaOrder = newSaOrder + "," + newSum.Id;
                            }
                            else
                            {
                                newSaOrder = newSum.Id.ToString();
                            }
                            break;

                        case SD.TrueFalse:
                            var truef = await _db.TrueFalses.SingleOrDefaultAsync(p => p.Id == summ.TrueFalseId);
                            var newTf = new TrueFalse() { Question = truef.Question, CorrectAnswer = truef.CorrectAnswer };
                            _db.TrueFalses.Add(newTf);

                            newSum.AssessmentType = SD.TrueFalse;
                            newSum.TrueFalseId = newTf.Id;
                            _db.Summatives.Add(newSum);
                            await _db.SaveChangesAsync();

                            if (newSaOrder != "" && newSaOrder != null)
                            {
                                newSaOrder = newSaOrder + "," + newSum.Id;
                            }
                            else
                            {
                                newSaOrder = newSum.Id.ToString();
                            }
                            break;

                        case SD.Assignment:
                            var ass = await _db.Assignments.SingleOrDefaultAsync(p => p.Id == summ.AssignmentId);
                            var newAss = new Assignment()
                            {
                                AssignmentRequest = ass.AssignmentRequest,
                                Description = ass.Description,
                                SubmissionRequirements = ass.SubmissionRequirements,
                                SummId = ass.SummId
                            };
                            assignment.Add(newAss);
                            _db.Assignments.Add(newAss);
                            await _db.SaveChangesAsync();

                            newSum.AssessmentType = SD.Assignment;
                            newSum.AssignmentId = newAss.Id;
                            _db.Summatives.Add(newSum);
                            await _db.SaveChangesAsync();

                            newAss.SummId = newSum.Id;
                            _db.Update(newAss);


                            if (newSaOrder != "" && newSaOrder != null)
                            {
                                newSaOrder = newSaOrder + "," + newSum.Id;
                            }
                            else
                            {
                                newSaOrder = newSum.Id.ToString();
                            }
                            await _db.SaveChangesAsync();

                            //Copy File
                            if (newAss.AssignmentRequest != null && newAss.AssignmentRequest != "")
                            {
                                var fileLoc = webRootPath + summ.Assignment.AssignmentRequest;
                                var fileDes = webRootPath + @"\assignmentfiles\" + newSum.Id + ".pdf";

                                if (System.IO.File.Exists(fileLoc))
                                {
                                    FileCopy.Copy(fileLoc, fileDes);
                                }

                                newAss.AssignmentRequest = @"\assignmentfiles\" + newSum.Id + ".pdf";
                                _db.Update(newAss);
                                await _db.SaveChangesAsync();
                            }

                            break;

                        case SD.Practical:
                            var pract = await _db.Practicals.SingleOrDefaultAsync(p => p.Id == summ.PracticalId);
                            var newPrac = new Practical()
                            {
                                Description = pract.Description,
                                PracticalRequest = pract.PracticalRequest,
                                Requirements = pract.Requirements,
                                SummId = pract.SummId
                            };
                            practical.Add(newPrac);
                            _db.Practicals.Add(newPrac);
                            await _db.SaveChangesAsync();

                            newSum.AssessmentType = SD.Practical;
                            newSum.PracticalId = newPrac.Id;

                            _db.Summatives.Add(newSum);
                            await _db.SaveChangesAsync();

                            newPrac.SummId = newSum.Id;
                            _db.Update(newPrac);



                            if (newSaOrder != "" && newSaOrder != null)
                            {
                                newSaOrder = newSaOrder + "," + newSum.Id;
                            }
                            else
                            {
                                newSaOrder = newSum.Id.ToString();
                            }
                            //Copy File
                            if (newPrac.PracticalRequest != null && newPrac.PracticalRequest != "")
                            {
                                var fileLoc = webRootPath + summ.Practical.PracticalRequest;
                                var fileDes = webRootPath + @"\practicalfiles\" + newSum.Id + ".pdf";

                                if (System.IO.File.Exists(fileLoc))
                                {
                                    FileCopy.Copy(fileLoc, fileDes);
                                }

                                newPrac.PracticalRequest = @"\practicalfiles\" + newSum.Id + ".pdf";
                                _db.Update(newPrac);
                                await _db.SaveChangesAsync();
                            }

                            break;
                    }
                }

                newUnit.SAOrder = newSaOrder;
                _db.Update(newUnit);
                await _db.SaveChangesAsync();

            }


            // 5 - Add the Formatives of the Unit

            var formProcessedList = new List<int>();
            var newTopicOrder = "";

            if (topicForm != null && topicForm.Count > 0)
            {
                foreach (var set in topicForm)
                {
                    var newFaOrder = "";
                    var sourceTopic = set.Key;
                    var newTopic = new CourseTopic()
                    {
                        ContentType = sourceTopic.ContentType,
                        CourseUnitId = newUnit.Id,
                        Duration = sourceTopic.Duration,
                         FAOrder = sourceTopic.FAOrder,
                        CustomContent = sourceTopic.CustomContent,
                        Name = sourceTopic.Name,
                        PDFContent = sourceTopic.PDFContent,
                        Reference = sourceTopic.Reference
                    };

                    _db.CourseTopics.Add(newTopic);

                    await _db.SaveChangesAsync();

                    if (newTopicOrder != "" && newTopicOrder != null)
                    {
                        newTopicOrder = newTopicOrder + "," + newTopic.Id;
                    }
                    else
                    {
                        newTopicOrder = newTopic.Id.ToString();
                    }


                    if (newTopic.PDFContent != null && newTopic.PDFContent != "")
                    {
                        var fileLoc = webRootPath + newTopic.PDFContent;
                        var fileDes = webRootPath + @"\topicfiles\" + newTopic.Id + ".pdf";

                        if (System.IO.File.Exists(fileLoc))
                        {
                            System.IO.File.Copy(fileLoc, fileDes, true);
                        }

                        newTopic.PDFContent = @"\topicfiles\" + newTopic.Id + ".pdf";
                        _db.Update(newTopic);

                    }


                    newUnit.CourseTopicIds = newTopicOrder;
                    _db.Update(newUnit);
                    await _db.SaveChangesAsync();



                    foreach (var formList in topicForm.Values)
                    {

                        foreach (var formative in formList)
                        {

                            var newFor = new Formative() { Title = formative.Title, Reference = formative.Reference, CourseTopicId = set.Key.Id };



                            if (newTopic.FAOrder != null && newTopic.FAOrder != "" && OrderList.ItemIdOrder(set.Key.FAOrder).Contains(formative.Id))
                            {

                                switch (formative.QuestionType)
                                {

                                    case SD.MultipleChoice:
                                        var mc = await _db.MultipleChoices.SingleOrDefaultAsync(p => p.Id == formative.MultipleChoiceId);
                                        var newMc = new MultipleChoice()
                                        {
                                            Question = mc.Question,
                                            AnswerA = mc.AnswerA,
                                            AnswerB = mc.AnswerB,
                                            AnswerC = mc.AnswerC,
                                            AnswerD = mc.AnswerD,
                                            CorrectAnswer = mc.CorrectAnswer
                                        };
                                        _db.MultipleChoices.Add(newMc);

                                        newFor.QuestionType = SD.MultipleChoice;
                                        newFor.MultipleChoiceId = newMc.Id;
                                        newFor.CourseTopicId = newTopic.Id;
                                        _db.Formatives.Add(newFor);
                                        await _db.SaveChangesAsync();
                                        if (newFaOrder != "" && newFaOrder != null)
                                        {
                                            newFaOrder = newFaOrder + "," + newFor.Id;
                                        }
                                        else
                                        {
                                            newFaOrder = newFor.Id.ToString();
                                        }
                                        newTopic.FAOrder = newFaOrder;
                                        _db.Update(newTopic);
                                        await _db.SaveChangesAsync();


                                        break;

                                    case SD.TrueFalse:
                                        var truef = await _db.TrueFalses.SingleOrDefaultAsync(p => p.Id == formative.TrueFalseId);
                                        var newTf = new TrueFalse() { Question = truef.Question, CorrectAnswer = truef.CorrectAnswer };
                                        _db.TrueFalses.Add(newTf);

                                        newFor.CourseTopicId = newTopic.Id;
                                        newFor.QuestionType = SD.TrueFalse;
                                        newFor.TrueFalseId = newTf.Id;
                                        _db.Formatives.Add(newFor);
                                        await _db.SaveChangesAsync();
                                        if (newFaOrder != "" && newFaOrder != null)
                                        {
                                            newFaOrder = newFaOrder + "," + newFor.Id;
                                        }
                                        else
                                        {
                                            newFaOrder = newFor.Id.ToString();
                                        }
                                     
                                            newTopic.FAOrder = newFaOrder;
                                            _db.Update(newTopic);
                                        await _db.SaveChangesAsync();

                                        break;

                                }
                                formProcessedList.Add(formative.Id);

                            }

                        }

                    }


                  



                }
            }
            return RedirectToAction("UnitList", "CourseUnit", new { id = courseId });
        }
    }
}