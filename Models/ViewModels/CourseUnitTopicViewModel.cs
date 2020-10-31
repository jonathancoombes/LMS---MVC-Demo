using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.Course;
using LMS.Models.Learning;
using LMS.Utility;

namespace LMS.Models.ViewModels
{
    public class CourseUnitTopicViewModel
    {
    
        public CourseTopic CourseTopic { get; set; }

        [Display(Name = "Unit Topics")]
        public IEnumerable<CourseTopic> CourseTopicList { get; set; }

        public CourseUnit CourseUnit { get; set; }
        public List<string> ContentType { get; set; }
        public List<FormativeSession> FormativeSessions { get; set; }
        public bool CanDelete { get; set; }

    }
}
