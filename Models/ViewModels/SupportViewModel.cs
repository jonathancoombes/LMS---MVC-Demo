using LMS.Models.Support;
using LMS.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class SupportViewModel
    {
        public IEnumerable<SupportRequest> SupportRequestList { get; set; }
        public SupportRequest SupportRequest { get; set; }
        public SupportResponse SupportResponse { get; set; }
        public IEnumerable<SupportResponse> SupportResponseList { get; set; }
        [Display(Name ="From")]
        public ApplicationUser ApplicationUser  { get; set; }
        public ApplicationUser Sender { get; set; }
        public IEnumerable<Class> Classes { get; set; }
        public List<Course.Course> Courses { get; set; }
        public Class Class { get; set; }
        public int ClassId { get; set; }
        public List<string> Statuses { get; set; }
        [Display(Name = "Status")]
        public SelectList RequestStatus { get; set; }
        [Display(Name="Class")]
        public SelectList UserClasses { get; set; }
        public IEnumerable<List<SupportResponse>> SupportResponseListofLists { get; set; }
    }
}
