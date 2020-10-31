using LMS.Models.Assessment;
using LMS.Models.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class ClassroomViewModel
    {

        public CourseTopic CourseTopic { get; set; }
        public int ClassId { get; set; }
        public bool? ContainsFormative { get; set; }
        public bool? IsLastTopicInUnit { get; set; }
        public bool TopicFANotComplete { get; set; }

        public bool? IsLastTopicOfCourse { get; set; }

    }
}
