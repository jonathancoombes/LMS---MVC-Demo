using LMS.Models.Course;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models
{
    public class ClassEnrolment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Role")]
        public string UserRole { get; set; }
        public int? ClassId { get; set; }
        public virtual Class Class { get; set; }
        [Display(Name = "Enrolment Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy HH:mm}")]
        public DateTime? EnrolmentDate { get; set; }
        public int? EnrolledByUserId { get; set; }
        public string CompletedTopics { get; set; }
        public string CompletedFas { get; set; }
        public string CompeletedSas { get; set; }
        public string CompletedUnits { get; set; }
        public int? CurrentTopicId { get; set; }
        public int? CurrentPage { get; set; }
        public string Status { get; set; }
        public string StatusMessage { get; set; }
        [Display(Name="First Name")]
        public string UserName { get; set; }
        [Display(Name = "Last Name")]
        public string UserSurname { get; set; }
        [Display(Name = "Identity Number")]
        public string Identity { get; set; }
    }
}
