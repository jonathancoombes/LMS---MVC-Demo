using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Assessment
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string SubmissionRequirements { get; set; }
        public string AssignmentRequest { get; set; }
        public int SummId { get; set; }

    }
}
