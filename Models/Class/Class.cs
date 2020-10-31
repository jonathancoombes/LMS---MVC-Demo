using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.Course;

namespace LMS.Models
{
    public class Class
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }
        public virtual Course.Course Course { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy HH:mm}")]
        [Display(Name="Start")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy HH:mm}")]
        [Display(Name = "End")]
        public DateTime EndDate { get; set; }
        public string EnrolIds { get; set; }


    }
}
