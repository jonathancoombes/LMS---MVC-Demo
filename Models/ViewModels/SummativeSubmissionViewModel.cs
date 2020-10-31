using LMS.Models.Assessment;
using LMS.Models.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class SummativeSubmissionViewModel
    {
        public MultipleChoice MultipleChoice { get; set; }
        public TrueFalse TrueFalse { get; set; }
        public DirectQuestion DirectQuestion { get; set; }
        public Practical Practical { get; set; }
        public Assignment Assignment { get; set; }
        public IEnumerable<MultipleChoice> MCList { get; set; }
        public IEnumerable<TrueFalse> TFList { get; set; }
        public IEnumerable<DirectQuestion> DirectList { get; set; }
        public IEnumerable<Assignment> AssList { get; set; }
        public IEnumerable<Practical> PractList { get; set; }

        public Summative Summative { get; set; }
        public SummativeSubmission SummativeSubmission { get; set; }

        public int QuestionCount { get; set; }
        public int QuestionCountCompleted { get; set; }
        public int AssignmentCountCompleted { get; set; }
        public int PracticalCountCompleted { get; set; }
        public int ClassEnrolId { get; set; }
        public string QuestionType { get; set; }
        public IEnumerable<Summative> SumList { get; set; }
        public IEnumerable<SummativeSubmission> SubList { get; set; }
        public int UnitId { get; set; }
        public string UnitTitle { get; set; }
        public SummativeSession SummativeSession { get; set; }
        public int? FinalGrade { get; set; }
        public int? ProvisionalGrade { get; set; }
        public Dictionary<Assignment, SummativeSubmission> AssSub { get; set; }
        public Dictionary<Practical, SummativeSubmission> PracSub { get; set; }
        public SubmissionValidity SubmissionValidity { get; set; }
        public IEnumerable<SubmissionValidity> SubValList { get; set; }
        public int QuestionMinutes { get; set; }
    }
}
