using LMS.Models.Assessment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Learning
{
    public class SummativeSubmission
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int SummativeId { get; set; }
        public virtual Summative Summative { get; set; }

        public string MCAnswer { get; set; }
        public string TFAnswer { get; set; }
        public string DirectAnswer { get; set; }
        public string PracticalSubmission { get; set; }
        public string AssignmentSubmission { get; set; }
        public string AssignmentGraded { get; set; }
        public string PracticalGraded { get; set; }

        [Display(Name = "Graded On")]
        public DateTime? GradingDate { get; set; }

        public int? GradePercentage { get; set; }

        public int SummativeSessionId { get; set; }
        public virtual SummativeSession SummativeSession { get; set; }
        public string AssessorId { get; set; }

    }
}
