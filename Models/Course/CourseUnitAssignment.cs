using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Course
{
    public class CourseUnitAssignment
    {
        public int Id { get; set; }

       
        public int CourseUnitId { get; set; }
        [ForeignKey("CourseUnitId")]
        public CourseUnit CourseUnit { get; set; }

     
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        
        public int CourseModuleId { get; set; }
        [ForeignKey("CourseModuleId")]
        public CourseModule CourseModule { get; set; }

    }
}
