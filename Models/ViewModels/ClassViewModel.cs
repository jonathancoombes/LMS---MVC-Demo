using LMS.Models.Course;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class ClassViewModel
    {

        public IEnumerable <Class> Classes { get; set; }
        public Class Class { get; set; }
        public IEnumerable<Course.Course> Courses { get; set; }
    
    }
}
