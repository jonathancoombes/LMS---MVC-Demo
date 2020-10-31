using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data;
using LMS.Extentions;
using LMS.Models.Assessment;
using LMS.Models.ViewModels;
using LMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LMS.Areas.Course.Controllers
{
    [Area("Course")]
    [Authorize(Roles = "Administrator,Designer")]
    public class FormativeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FormativeController(ApplicationDbContext db)
        {
            _db = db;
        }



        //GET
        public async Task<IActionResult> FormativeList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FormativeViewModel model = new FormativeViewModel
            {
                CourseTopic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == id),
                FormativeList = new List<Formative>(),
                MultipleChoiceList = new List<MultipleChoice>(),
                TrueFalseList = new List<TrueFalse>(),
                Formative = new Formative(),
                QuestionType = new List<string>() { SD.TrueFalse, SD.MultipleChoice }

            };

            var unit = await _db.CourseUnits.SingleOrDefaultAsync(p=>p.Id == model.CourseTopic.CourseUnitId);

            model.CanDelete = CanDeleteUnitOrChild.CanDel(unit, _db);

            //Preparing a List of Formative Assessments In Order

            if (model.CourseTopic.FAOrder != null && model.CourseTopic.FAOrder != "")
            {
                var formativeIds = OrderList.ItemIdOrder(model.CourseTopic.FAOrder);

                var formativeList = new List<Formative>();

                foreach (var item in formativeIds)
                {
                    formativeList.AddRange(_db.Formatives.Include(p => p.MultipleChoice).Include(p => p.TrueFalse).Where(p => p.Id == item).ToList());

                };

                model.FormativeList = formativeList;

            }

            return View(model);
        }



        public async Task<IActionResult> Create(int? id)
        {
            if (id == null) {

                return NotFound();

            }

            FormativeViewModel model = new FormativeViewModel
            {
                CourseTopic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == id),
                FormativeList = new List<Formative>(),
                MultipleChoiceList = new List<MultipleChoice>(),
                TrueFalseList = new List<TrueFalse>(),
                QuestionType = new List<string>() { SD.TrueFalse, SD.MultipleChoice }

            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormativeViewModel formativeVM, int topId) {
            
            if (ModelState.IsValid)
            {

                //Check to see if there is allready a question with the same name

                var formativeExists = _db.Formatives.Where(s =>
                    s.Title == formativeVM.Formative.Title &&
                    s.CourseTopicId == topId);

                // If it does, oops:

                if (formativeExists.Count() > 0)
                {
                    ModelState.AddModelError("Name", "Another Question with the same name allready exist in this Topic. Please choose another name.");
                    
                    FormativeViewModel model = new FormativeViewModel
                    {
                        CourseTopic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == topId),
                        Formative = formativeVM.Formative,
                        FormativeList = new List<Formative>(),
                        QuestionType = new List<string>() { SD.TrueFalse, SD.MultipleChoice }
                    };

                    return View(model);
                }

                //Otherwise, all is well so let's go:

                else
                {

                   formativeVM.Formative.CourseTopicId = topId;

                    if (formativeVM.Formative.QuestionType == SD.TrueFalse) {
                        formativeVM.Formative.MultipleChoice = null;
                    }
                    else if (formativeVM.Formative.QuestionType == SD.MultipleChoice) {
                        formativeVM.Formative.TrueFalse = null;
                    }

                    //1 - Adding the new Topic to DB

                    _db.Formatives.Add(formativeVM.Formative);

                    await _db.SaveChangesAsync();

                    
                    //2 - Update Topic Order in Unit Table

                    int formId = formativeVM.Formative.Id;

                    var currentTopic = await _db.CourseTopics.Where(c => c.Id == topId).SingleOrDefaultAsync();

                    var currentFAOrder = currentTopic.FAOrder;

                    if (currentFAOrder == null || currentFAOrder == "")
                    {
                        currentTopic.FAOrder = formId.ToString();

                        _db.Update(currentTopic);
                        await _db.SaveChangesAsync();

                    }
                    else if (currentFAOrder != null)
                    {
                        var finalOrder = String.Concat(currentFAOrder + "," + formId.ToString());
                        currentTopic.FAOrder = finalOrder;

                        _db.Update(currentTopic);
                        await _db.SaveChangesAsync();


                    }
                    return RedirectToAction("FormativeList", "Formative", new { id = topId });

                }

            }
            //If the ModelState is not valid

            FormativeViewModel modelVM = new FormativeViewModel
            {
                CourseTopic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == topId),
                Formative = formativeVM.Formative,
                FormativeList = new List<Formative>(),
                QuestionType = new List<string>() { SD.TrueFalse, SD.MultipleChoice }
            };


            return RedirectToAction("FormativeList", "Formative", new { id = topId});

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
                var formative = await _db.Formatives.Include(p => p.MultipleChoice).Include(p => p.TrueFalse).SingleOrDefaultAsync(c => c.Id == id);

                if (formative == null)
                {

                    return NotFound();
                }
                else
                {

                    FormativeViewModel formativeVM = new FormativeViewModel
                    {
                        CourseTopic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == formative.CourseTopicId),
                        Formative = formative,
                        QuestionType = new List<string>() { SD.TrueFalse, SD.MultipleChoice }
                    };
                    return View(formativeVM);
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FormativeViewModel formativeVM) {

            formativeVM.Formative.CourseTopicId = formativeVM.CourseTopic.Id;

            var formativeExists = _db.Formatives.Where(c => 
                c.Title == formativeVM.Formative.Title &&
                c.CourseTopicId == formativeVM.Formative.CourseTopicId &&
                c.Id != formativeVM.Formative.Id);

            if (formativeExists.Count() > 0) {

                ModelState.AddModelError("Title", "Another Question with the same title allready exists in this Topic");

                FormativeViewModel model = new FormativeViewModel()
                {
                    CourseTopic = formativeVM.CourseTopic,
                    Formative = formativeVM.Formative
                };

                return View(model);

                }

            else if (!ModelState.IsValid) {

                FormativeViewModel model = new FormativeViewModel()
                {
                    CourseTopic = formativeVM.CourseTopic,
                    Formative = formativeVM.Formative
                };

                return View(model);
            }

            _db.Update(formativeVM.Formative);
            await _db.SaveChangesAsync();
            
            return RedirectToAction("FormativeList", "Formative", new { id = formativeVM.Formative.CourseTopicId });
        }



        //GET - DELETE

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var formative = await _db.Formatives.FindAsync(id);

            if (formative == null)
            {
                return NotFound();
            }

            return View(formative);

        }


        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentFA = await _db.Formatives.Include(c => c.TrueFalse).Include(c => c.MultipleChoice).SingleOrDefaultAsync(c => c.Id == id);

            var topicId = currentFA.CourseTopicId;
            var currentTopic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == topicId);

            var currentFAOrder = currentTopic.FAOrder;

            if (currentFAOrder == null || currentFAOrder == "" || !OrderList.ItemIdOrder(currentFAOrder).Contains(id))
            {
                return NotFound();
            }

            currentTopic.FAOrder = RePosition.Delete(currentFAOrder, id);



            if (currentFA.QuestionType == SD.TrueFalse) {

                var trueFalse = await _db.TrueFalses.SingleOrDefaultAsync(c => c.Id == currentFA.TrueFalseId);
                _db.TrueFalses.Remove(trueFalse);

            }
            else if (currentFA.QuestionType == SD.MultipleChoice)
            {
                var mChoice = await _db.MultipleChoices.SingleOrDefaultAsync(c => c.Id == currentFA.MultipleChoiceId);
                _db.MultipleChoices.Remove(mChoice);

            }

            _db.Formatives.Remove(currentFA);
            _db.Update(currentTopic);

            await _db.SaveChangesAsync();

            return RedirectToAction("FormativeList", "Formative", new { id = topicId });


        }


        public async Task<IActionResult> MoveFAUp(int Id)
        {
            var currentFA = await _db.Formatives.SingleOrDefaultAsync(c => c.Id == Id);

            var topicId = currentFA.CourseTopicId;

            var currentTopic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == topicId);

            var currentFAOrder = currentTopic.FAOrder;

            if (RePosition.CanMoveUpCheck(currentFAOrder, Id))
            {
                currentTopic.FAOrder = RePosition.MoveUp(currentFAOrder, Id);
                _db.Update(currentTopic);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("FormativeList", "Formative", new { id = topicId });
        }

        public async Task<IActionResult> MoveFADown(int Id)
        {
            var currentFA = await _db.Formatives.SingleOrDefaultAsync(c => c.Id == Id);

            var topicId = currentFA.CourseTopicId;

            var currentTopic = await _db.CourseTopics.SingleOrDefaultAsync(c => c.Id == topicId);

            var currentFAOrder = currentTopic.FAOrder;

            if (RePosition.CanMoveDownCheck(currentFAOrder, Id))
            {
                currentTopic.FAOrder = RePosition.MoveDown(currentFAOrder, Id);
                _db.Update(currentTopic);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("FormativeList", "Formative", new { id = topicId });
        }






    }
}