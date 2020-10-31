using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class LearnerProgressViewModel
    {

        public ClassEnrolment ClassEnrolment { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int TotalUnits { get; set; }
        public int TotalUnitsCompleted { get; set; }
        public int TotalTopics { get; set; }
        public int TotalTopicsCompleted { get; set; }
        public int TotalFA { get; set; }
        public int TotalFACompleted { get; set; }
        public int TotalSA { get; set; }
        public int TotalSACompleted { get; set; }
        public int TotalProgressPercentage { get; set; }
        public int TotalSupportRequests { get; set; }
        public TimeSpan? EnrolledFor { get; set; }

    }
}
