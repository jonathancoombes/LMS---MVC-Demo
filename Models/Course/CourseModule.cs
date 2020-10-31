using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Course
{
    public class CourseModule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int CourseId { get; set; }

        //Linking the foreign key "CourseId" above to the Course table:

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public string CourseUnitOrder { get; set; }
    }
}
