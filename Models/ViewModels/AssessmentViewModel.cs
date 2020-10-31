using LMS.Models.Assessment;
using LMS.Models.Course;
using LMS.Models.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class AssessmentViewModel
    {

        public DirectQuestion DirectQuestion { get; set; }
        public Assignment Assignment { get; set; }
        public Practical Practical { get; set; }
        public Summative Summative { get; set; }
        public ApplicationUser Assessor { get; set; }
        public ApplicationUser Learner { get; set; }
        public SummativeSubmission SummativeSubmission { get; set; }
        public IEnumerable <Summative> SummativeList { get; set; }
        public IEnumerable <SummativeSubmission> SubmissionList { get; set; }
        public IEnumerable <DirectQuestion> DirectList { get; set; }
        public IEnumerable <Assignment> AssignmentList { get; set; }
        public IEnumerable <Practical> PracticalList { get; set; }
        public ClassEnrolment ClassEnrolment { get; set; }
        public Dictionary<ClassEnrolment, List<SummativeSubmission>> EnrolSubList { get; set; }
        public Dictionary<SummativeSession, List<SummativeSubmission>> SessionSubList { get; set; }
        public Dictionary<DirectQuestion, SummativeSubmission> QuestSubList { get; set; }

        public IEnumerable<CourseUnit> UnitList { get; set; }
        public IEnumerable<ApplicationUser> Assessors { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdNumber { get; set; }
        public string UnitTitle { get; set; }
    }
}
