using LMS.Models.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class LessonPlanViewModel
    {

        public IEnumerable<CourseTopic> Topics { get; set; }
        public int MyProperty { get; set; }



    }
}
