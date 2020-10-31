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
    public class FormativeViewModel
    {

        public Formative Formative { get; set; }

        [Display(Name = "Formative Question")]
        public IEnumerable<Formative> FormativeList { get; set; }

        [Display(Name = "Multiple Choice")]
        public IEnumerable<MultipleChoice> MultipleChoiceList { get; set; }

        [Display(Name = "True or False")]
        public IEnumerable<TrueFalse> TrueFalseList { get; set; }

        public List<string> QuestionType { get; set; }

        public CourseTopic CourseTopic { get; set; }
        public bool CanDelete { get; set; }
    }
}
