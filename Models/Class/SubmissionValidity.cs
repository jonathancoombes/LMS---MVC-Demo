using LMS.Models.Assessment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models
{
    public class SubmissionValidity
    {

        public int Id { get; set; }

        public DateTime Open { get; set; }
        public DateTime Close { get; set; }

        public int SummativeId { get; set; }
        public virtual Summative Summative { get; set; }
        public int ClassId { get; set; }
        public virtual Class Class { get; set; }
    }
}
