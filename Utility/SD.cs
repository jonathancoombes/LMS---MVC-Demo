using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Utility
{
    public class SD
    {
        public const string DefaultProfileImage = "default.png";
        public const string BasePath = "http://localhost";

        //Roles
        public const string Administrator = "Administrator";
        public const string Designer = "Designer";
        public const string Facilitator = "Facilitator";
        public const string Assessor = "Assessor";
        public const string Learner = "Learner";

        //Assessment Methods / Types
        public const string ContentPDF = "PDF";
        public const string ContentCustom = "Custom";
        public const string MultipleChoice = "Multiple-Choice";
        public const string TrueFalse = "TrueFalse";
        public const char AnswerOptionA = 'A';
        public const char AnswerOptionB = 'B';
        public const char AnswerOptionC = 'C';
        public const char AnswerOptionD = 'D';
        public const string AnswerOptionTrue = "True";
        public const string AnswerOptionFalse = "False";
        public const string DirectQuestion = "Question";
        public const string Assignment = "Assignment";
        public const string Practical = "Practical";

        //Class Enrolment Status
        public const string StatusActive = "Active";
        public const string StatusInactive = "Inactive";
        public const string StatusCompleted = "Completed";
        public const string StatusSuspended = "Suspended";

        //All Completed Topic / FA / SA
        public const string AllComplete = "COMPLETE";

        //SupportRequest Status
        public const string StatusOpen = "Open";
        public const string StatusClosed = "Closed";
        public const string StatusPending = "Pending";

        //Overall Pass Percentage Required
        public const int FAPassRate = 75;
        public const int SAPassRate = 75;

        //Individual Submission PassMark
        public const int SASubPassMark = 50;

        public const string Competent = "Competent";
        public const string NotYetCompetent = "NYC";

        //Config Option
        public const bool AnswerGuideVisible = true;  

    }
}