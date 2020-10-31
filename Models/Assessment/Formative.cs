using LMS.Models.Course;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Assessment
{
  
    public class Formative
    {        
        
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        public string Reference { get; set; }

        public int CourseTopicId { get; set; }

        public int? MultipleChoiceId { get; set; }
        public virtual MultipleChoice MultipleChoice { get; set; }
        public int? TrueFalseId { get; set; }
        public virtual TrueFalse TrueFalse { get; set; }
        [Display(Name = "Type")]
        public string QuestionType { get; set; }
    }
}
