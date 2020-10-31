using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Course
{
    public class CourseUnit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Reference{ get; set; }
        public int? Level { get; set; }
        public int? Credits { get; set; }
  
        public string CourseTopicIds { get; set; }
        public string SAOrder { get; set; }
    }
}