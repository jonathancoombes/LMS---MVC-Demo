using System;
using System.Collections.Generic;
using System.Text;
using LMS.Models;
using LMS.Models.Course;
using LMS.Models.Assessment;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LMS.Models.Support;
using LMS.Models.Learning;
using LMS.LMSConfig;

namespace LMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseModule> CourseModules { get; set; }
        public DbSet <CourseUnitAssignment> CourseUnitAssignments { get; set; }
        public DbSet<CourseUnit> CourseUnits { get; set; }
        public DbSet<CourseTopic> CourseTopics { get; set; }

        public DbSet<Formative> Formatives { get; set; }
        public DbSet<MultipleChoice> MultipleChoices { get; set; }
        public DbSet<TrueFalse> TrueFalses { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<DirectQuestion> DirectQuestions { get; set; }
        public DbSet<Summative> Summatives { get; set; }
        public DbSet<Practical> Practicals { get; set; }

        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassEnrolment> ClassEnrolments { get; set; }

        public DbSet<SupportRequest> SupportRequests { get; set; }

        public DbSet<SupportResponse> SupportResponse { get; set; }

        public DbSet<FormativeSession> FormativeSessions { get; set; }
        public DbSet<FormativeSubmission> FormativeSubmissions { get; set; }

        public DbSet<SummativeSession> SummativeSessions { get; set; }
        public DbSet<SummativeSubmission> SummativeSubmissions { get; set; }

        public DbSet<SubmissionValidity> SubmissionValidities { get; set; }

        public DbSet<ConfigOptions> ConfigOptions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
