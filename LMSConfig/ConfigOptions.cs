using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.LMSConfig
{
    public class ConfigOptions
    {
        public int Id { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Minutes Per Question (Range 1-10)")]
        [Range(1, 10)]
        public int MinutesPerQuest { get; set; } 
        [Display(Name = "Completed Course Message")]
        public string CompletedCourseMessage { get; set; }
        [Display(Name = "Multiple User Roles")]
        public bool MultipleUserRolePerClassEnrol { get; set; } 
        [Display(Name = "Assessor Support")]
        public bool AssessorCanSupport { get; set; } 
        [Display(Name = "Facilitator Support")]
        public bool FacilitatorCanSupport { get; set; }
    }
}
