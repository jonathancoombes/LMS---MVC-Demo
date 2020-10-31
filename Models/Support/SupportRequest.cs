using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Support
{
    public class SupportRequest
    {
        public int Id { get; set; }
        [Display(Name = "Heading")]
        public string RequestHeading { get; set; }
        [Display(Name = "Body")]
        public string RequestBody { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy HH:mm}")]
        [Display(Name = "Date Created")]
        public DateTime Open { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy HH:mm}")]
        public DateTime Closed { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        public int ClassId { get; set; }
        [Display(Name = "Responses")]
        public string ResponseIds { get; set; }
    }
}
