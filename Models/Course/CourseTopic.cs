using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Course
{
    public class CourseTopic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        public int? Duration { get; set; }
        public string ContentType { get; set; }
        public string CustomContent { get; set; }
        public string PDFContent { get; set; }

        public int CourseUnitId { get; set; }
        [ForeignKey("CourseUnitId")]
        public CourseUnit CourseUnit { get; set; }

        public string FAOrder { get; set; }
    }
}
