using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Learning
{
    public class SummativeSession
    {

        public int Id { get; set; }

        public string UserId { get; set; }

        public int ClassEnrolId { get; set; }

        public int UnitId { get; set; }

        public string SAIdsInOrder { get; set; }
        public string SASubmissionInOrder { get; set; }

        public int AttemptNumber { get; set; }
        [Display(Name = "Opens")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy HH:mm}")]
        public DateTime SAStart { get; set; }
        [Display(Name = "Closes")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy HH:mm}")]
        public DateTime? SAEnd { get; set; }
        
        public int? QuestionInProgressGrade { get; set; }
        public int? QuestionGradeTotal { get; set; }
        public int? AssignmentGradeTotal { get; set; }
        public int? PracticalGradeTotal { get; set; }

        public int FinalPercentageAchieved { get; set; }
        public string FinalResult { get; set; }
        public string SAFeedback { get; set; }


    }


}

