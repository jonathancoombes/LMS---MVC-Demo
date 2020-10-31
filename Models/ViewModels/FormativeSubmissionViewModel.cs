using LMS.Models.Assessment;
using LMS.Models.Learning;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class FormativeSubmissionViewModel
    {
        public MultipleChoice MultipleChoice { get; set; }

        public TrueFalse TrueFalse { get; set; }
        public Formative Formative { get; set; }
        public FormativeSubmission FormativeSubmission { get; set; }
        public int QuestionCount { get; set; }
        public int ClassEnrolId { get; set; }
        public string QuestionType { get; set; }
        public int? FinalGrade { get; set; }
        public IEnumerable<Formative> FormList { get; set; }
        public IEnumerable<FormativeSubmission> SubList { get; set; }
        public int TopicId { get; set; }
    }
}
