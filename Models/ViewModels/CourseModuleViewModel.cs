using LMS.Models.Course;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class CourseModuleViewModel
    {
        public Course.Course Course { get; set; }

        public CourseModule CourseModule { get; set; }
        [Display(Name="Module")]
        public List<CourseModule> CourseModuleList { get; set; }

    }
}
