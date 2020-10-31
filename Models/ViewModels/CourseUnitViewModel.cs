using LMS.Models.Course;
using LMS.Models.Learning;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class CourseUnitViewModel
    {
        public CourseUnit CourseUnit { get; set; }
        public Course.Course Course { get; set; }
        [Display(Name = "Module")]
        public IEnumerable<CourseModule> CourseModuleList { get; set; }
        [Display(Name = "Unit")]
        public IEnumerable<CourseUnit> CourseUnitList { get; set; }
        public CourseUnitAssignment CourseUnitAssignment { get; set; }
        public IEnumerable<CourseUnitAssignment> CourseUnitAssignments { get; set; }
        public Dictionary<int, List<SubmissionValidity>> UnitValidityPairs { get; set; }
        public SubmissionValidity SubmissionValidity { get; set; }
        public List<int> ModsWithVals { get; set; }

        [Display(Name = "Course")]
        public IEnumerable<Course.Course> CourseList { get; set; }
        public int CopyFromCourseId { get; set; }
        [Display(Name = "Module")]
        public int CopyFromModuleId { get; set; }
        [Display(Name="Module")]
        public int CopyToModuleId { get; set; }
        public int CopyToCourseId { get; set; }
        [Display(Name = "Unit")]

        public int CopyUnitId { get; set; }
        public SelectList Courses { get; set; }
        public ClassEnrolment ClassEnrol { get; set; }
        public List<int> AssPracInComplete { get; set; }
        public List<SummativeSession> SummSubs { get; set; }

    }
}
