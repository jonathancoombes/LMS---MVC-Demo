using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class ClassEnrolmentViewModel
    {
        public ClassEnrolment ClassEnrolment { get; set; }
        public IEnumerable<ClassEnrolment> ClassEnrolments { get; set; }
        public IEnumerable<Course.Course> Courses { get; set; }
        [Display(Name="User to Enrol")]
        public IEnumerable<ApplicationUser> ApplicationUser { get; set; }
        public Class Class { get; set; }
        public SelectList UsersInRole { get; set; }
        public ApplicationUser User { get; set; }
    }
}
