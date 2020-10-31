using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Support
{
    public class SupportResponse
    {
        public int Id { get; set; }

        public int SupportRequestId { get; set; }
        public virtual SupportRequest SupportRequest { get; set; }
        [Display(Name="Date")]
        public DateTime ResponseDate { get; set; }
        [Display(Name = "Message")]
        public string ResponseBody { get; set; }

        public string UserId { get; set; }
    }
}
