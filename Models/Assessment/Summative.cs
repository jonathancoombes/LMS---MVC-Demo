using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Assessment
{
    public class Summative
    {
        
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        public string Reference { get; set; }

        public int CourseUnitId { get; set; }

        public int? MultipleChoiceId { get; set; }
        public virtual MultipleChoice MultipleChoice { get; set; }
        public int? TrueFalseId { get; set; }
        public virtual TrueFalse TrueFalse { get; set; }

        public int? DirectQuestionId { get; set; }
        public virtual DirectQuestion DirectQuestion { get; set; }

        public int? AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }

        public int? PracticalId { get; set; }
        public virtual Practical Practical { get; set; }

        [Display(Name = "Type")]
        public string AssessmentType { get; set; }

        public int? Weight { get; set; }



    }
}
