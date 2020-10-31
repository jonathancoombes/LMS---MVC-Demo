
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace LMS.Models.ViewModels
{
    public class CourseViewModel
    {
        public Course.Course Course { get; set; }

        public IEnumerable<SubCategory> SubCategory { get; set; }
        public IEnumerable<Category> Category { get; set; }


    }
}
