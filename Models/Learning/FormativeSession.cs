using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Learning
{
    public class FormativeSession
    {

        public int Id { get; set; }

        public string UserId { get; set; }

        public int ClassEnrolId { get; set; }

        public int TopicId { get; set; }

        public string FAQuestionIdsInOrder { get; set; }
        public string FASubmissionInOrder { get; set; }
          
        public DateTime FAStart { get; set; }
        public DateTime? FAEnd { get; set; }
               
        public string FAGradesInOrder { get; set; }
        public int? PercentageAchieved { get; set; }

        public string FAFeedback { get; set; }

        public string Result { get; set; }
    }
}
