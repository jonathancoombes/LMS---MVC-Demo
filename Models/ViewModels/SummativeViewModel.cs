using LMS.Models.Assessment;
using LMS.Models.Course;
using LMS.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class SummativeViewModel
    {

        public Summative Summative { get; set; }

        [Display(Name = "Summative Assessment")]
        public IEnumerable<Summative> SummativeList { get; set; }

        [Display(Name = "Multiple Choice")]
        public IEnumerable<MultipleChoice> MultipleChoiceList { get; set; }

        [Display(Name = "True or False")]
        public IEnumerable<TrueFalse> TrueFalseList { get; set; }
        [Display(Name = "Assignment")]
        public IEnumerable<Assignment> AssignmentList { get; set; }
        [Display(Name = "Practical")]
        public IEnumerable<Practical> PracticalList { get; set; }
        [Display(Name = "Question")]
        public IEnumerable<DirectQuestion> DirectQuestionList { get; set; }

        public List<string> AssessmentType { get; set; }

        public CourseUnit CourseUnit { get; set; }
        public SubmissionValidity SubmissionValidity { get; set; }
        public bool CanDelete { get; set; }

    }
}
