﻿// <auto-generated />
using System;
using LMS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LMS.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190907200348_UpdateSummativeSubmissionModelForGrading")]
    partial class UpdateSummativeSubmissionModelForGrading
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LMS.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<int?>("ActiveClassId");

                    b.Property<string>("City");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("ContactNumber");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("IdentityNumber");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("PostalCode");

                    b.Property<string>("Province");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("StreetAddress");

                    b.Property<string>("Surname");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("LMS.Models.Assessment.Assignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AssignmentRequest");

                    b.Property<string>("AssignmentSubmission");

                    b.Property<string>("Description");

                    b.Property<string>("SubmissionRequirements");

                    b.Property<int>("SummId");

                    b.HasKey("Id");

                    b.ToTable("Assignments");
                });

            modelBuilder.Entity("LMS.Models.Assessment.DirectQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AnswerGuide");

                    b.Property<string>("Question");

                    b.HasKey("Id");

                    b.ToTable("DirectQuestions");
                });

            modelBuilder.Entity("LMS.Models.Assessment.Formative", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CourseTopicId");

                    b.Property<int?>("MultipleChoiceId");

                    b.Property<string>("QuestionType");

                    b.Property<string>("Reference");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int?>("TrueFalseId");

                    b.HasKey("Id");

                    b.HasIndex("MultipleChoiceId");

                    b.HasIndex("TrueFalseId");

                    b.ToTable("Formatives");
                });

            modelBuilder.Entity("LMS.Models.Assessment.MultipleChoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AnswerA");

                    b.Property<string>("AnswerB");

                    b.Property<string>("AnswerC");

                    b.Property<string>("AnswerD");

                    b.Property<string>("CorrectAnswer")
                        .IsRequired()
                        .HasConversion(new ValueConverter<string, string>(v => default(string), v => default(string), new ConverterMappingHints(size: 1)));

                    b.Property<string>("Question");

                    b.HasKey("Id");

                    b.ToTable("MultipleChoices");
                });

            modelBuilder.Entity("LMS.Models.Assessment.Practical", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("PracticalRequest");

                    b.Property<string>("PracticalSubmission");

                    b.Property<string>("Requirements");

                    b.Property<int>("SummId");

                    b.HasKey("Id");

                    b.ToTable("Practicals");
                });

            modelBuilder.Entity("LMS.Models.Assessment.Summative", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AssessmentType");

                    b.Property<int?>("AssignmentId");

                    b.Property<int>("CourseUnitId");

                    b.Property<int?>("DirectQuestionId");

                    b.Property<int?>("MultipleChoiceId");

                    b.Property<int?>("PracticalId");

                    b.Property<string>("Reference");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int?>("TrueFalseId");

                    b.Property<int?>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentId");

                    b.HasIndex("DirectQuestionId");

                    b.HasIndex("MultipleChoiceId");

                    b.HasIndex("PracticalId");

                    b.HasIndex("TrueFalseId");

                    b.ToTable("Summatives");
                });

            modelBuilder.Entity("LMS.Models.Assessment.TrueFalse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("CorrectAnswer");

                    b.Property<string>("Question");

                    b.HasKey("Id");

                    b.ToTable("TrueFalses");
                });

            modelBuilder.Entity("LMS.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("CanDelete");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("LMS.Models.Class", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CourseId");

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("EnrolIds");

                    b.Property<string>("Name");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("Classes");
                });

            modelBuilder.Entity("LMS.Models.ClassEnrolment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ClassId");

                    b.Property<string>("CompeletedSas");

                    b.Property<string>("CompletedFas");

                    b.Property<string>("CompletedTopics");

                    b.Property<string>("CompletedUnits");

                    b.Property<int?>("CurrentPage");

                    b.Property<int?>("CurrentTopicId");

                    b.Property<int?>("EnrolledByUserId");

                    b.Property<DateTime?>("EnrolmentDate");

                    b.Property<string>("Identity");

                    b.Property<string>("Status");

                    b.Property<string>("StatusMessage");

                    b.Property<string>("UserId");

                    b.Property<string>("UserName");

                    b.Property<string>("UserRole");

                    b.Property<string>("UserSurname");

                    b.HasKey("Id");

                    b.HasIndex("ClassId");

                    b.ToTable("ClassEnrolments");
                });

            modelBuilder.Entity("LMS.Models.Course.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId");

                    b.Property<string>("CourseModuleOrder");

                    b.Property<string>("Description");

                    b.Property<string>("Duration");

                    b.Property<string>("Image");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Reference");

                    b.Property<int>("SubCategoryId");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("SubCategoryId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("LMS.Models.Course.CourseModule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CourseId");

                    b.Property<string>("CourseUnitOrder");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("CourseModules");
                });

            modelBuilder.Entity("LMS.Models.Course.CourseTopic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ContentType");

                    b.Property<int>("CourseUnitId");

                    b.Property<string>("CustomContent");

                    b.Property<int?>("Duration");

                    b.Property<string>("FAOrder");

                    b.Property<string>("Name");

                    b.Property<string>("PDFContent");

                    b.Property<string>("Reference");

                    b.HasKey("Id");

                    b.HasIndex("CourseUnitId");

                    b.ToTable("CourseTopics");
                });

            modelBuilder.Entity("LMS.Models.Course.CourseUnit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CourseTopicIds");

                    b.Property<int?>("Credits");

                    b.Property<int?>("Level");

                    b.Property<string>("Name");

                    b.Property<string>("Reference");

                    b.Property<string>("SAOrder");

                    b.HasKey("Id");

                    b.ToTable("CourseUnits");
                });

            modelBuilder.Entity("LMS.Models.Course.CourseUnitAssignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CourseId");

                    b.Property<int>("CourseModuleId");

                    b.Property<int>("CourseUnitId");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("CourseModuleId");

                    b.HasIndex("CourseUnitId");

                    b.ToTable("CourseUnitAssignments");
                });

            modelBuilder.Entity("LMS.Models.Learning.FormativeSession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClassEnrolId");

                    b.Property<DateTime?>("FAEnd");

                    b.Property<string>("FAFeedback");

                    b.Property<string>("FAGradesInOrder");

                    b.Property<string>("FAQuestionIdsInOrder");

                    b.Property<DateTime>("FAStart");

                    b.Property<string>("FASubmissionInOrder");

                    b.Property<int?>("PercentageAchieved");

                    b.Property<string>("Result");

                    b.Property<int>("TopicId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("FormativeSessions");
                });

            modelBuilder.Entity("LMS.Models.Learning.FormativeSubmission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FormativeId");

                    b.Property<int>("FormativeSessionId");

                    b.Property<string>("MCAnswer");

                    b.Property<string>("Result");

                    b.Property<string>("TFAnswer");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("FormativeId");

                    b.HasIndex("FormativeSessionId");

                    b.ToTable("FormativeSubmissions");
                });

            modelBuilder.Entity("LMS.Models.Learning.SummativeSession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AssignmentGradeTotal");

                    b.Property<int>("AttemptNumber");

                    b.Property<int>("ClassEnrolId");

                    b.Property<int>("FinalPercentageAchieved");

                    b.Property<string>("FinalResult");

                    b.Property<int?>("PracticalGradeTotal");

                    b.Property<int?>("QuestionGradeTotal");

                    b.Property<int?>("QuestionInProgressGrade");

                    b.Property<DateTime?>("SAEnd");

                    b.Property<string>("SAFeedback");

                    b.Property<string>("SAIdsInOrder");

                    b.Property<DateTime>("SAStart");

                    b.Property<string>("SASubmissionInOrder");

                    b.Property<int>("UnitId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("SummativeSessions");
                });

            modelBuilder.Entity("LMS.Models.Learning.SummativeSubmission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ApplicationUserId");

                    b.Property<string>("ApplicationUserId1");

                    b.Property<string>("AssignmentGraded");

                    b.Property<string>("AssignmentSubmission");

                    b.Property<string>("DirectAnswer");

                    b.Property<int?>("GradePercentage");

                    b.Property<DateTime?>("GradingDate");

                    b.Property<string>("MCAnswer");

                    b.Property<string>("PracticalGraded");

                    b.Property<string>("PracticalSubmission");

                    b.Property<int>("SummativeId");

                    b.Property<int>("SummativeSessionId");

                    b.Property<string>("TFAnswer");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId1");

                    b.HasIndex("SummativeId");

                    b.HasIndex("SummativeSessionId");

                    b.ToTable("SummativeSubmissions");
                });

            modelBuilder.Entity("LMS.Models.SubCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("CanDelete");

                    b.Property<int>("CategoryId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("SubCategories");
                });

            modelBuilder.Entity("LMS.Models.SubmissionValidity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClassId");

                    b.Property<DateTime>("Close");

                    b.Property<DateTime>("Open");

                    b.Property<int>("SummativeId");

                    b.HasKey("Id");

                    b.HasIndex("ClassId");

                    b.HasIndex("SummativeId");

                    b.ToTable("SubmissionValidities");
                });

            modelBuilder.Entity("LMS.Models.Support.SupportRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClassId");

                    b.Property<DateTime>("Closed");

                    b.Property<DateTime>("Open");

                    b.Property<string>("RequestBody");

                    b.Property<string>("RequestHeading");

                    b.Property<string>("ResponseIds");

                    b.Property<string>("Status");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("SupportRequests");
                });

            modelBuilder.Entity("LMS.Models.Support.SupportResponse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ResponseBody");

                    b.Property<DateTime>("ResponseDate");

                    b.Property<int>("SupportRequestId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("SupportRequestId");

                    b.ToTable("SupportResponse");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("LMS.Models.Assessment.Formative", b =>
                {
                    b.HasOne("LMS.Models.Assessment.MultipleChoice", "MultipleChoice")
                        .WithMany()
                        .HasForeignKey("MultipleChoiceId");

                    b.HasOne("LMS.Models.Assessment.TrueFalse", "TrueFalse")
                        .WithMany()
                        .HasForeignKey("TrueFalseId");
                });

            modelBuilder.Entity("LMS.Models.Assessment.Summative", b =>
                {
                    b.HasOne("LMS.Models.Assessment.Assignment", "Assignment")
                        .WithMany()
                        .HasForeignKey("AssignmentId");

                    b.HasOne("LMS.Models.Assessment.DirectQuestion", "DirectQuestion")
                        .WithMany()
                        .HasForeignKey("DirectQuestionId");

                    b.HasOne("LMS.Models.Assessment.MultipleChoice", "MultipleChoice")
                        .WithMany()
                        .HasForeignKey("MultipleChoiceId");

                    b.HasOne("LMS.Models.Assessment.Practical", "Practical")
                        .WithMany()
                        .HasForeignKey("PracticalId");

                    b.HasOne("LMS.Models.Assessment.TrueFalse", "TrueFalse")
                        .WithMany()
                        .HasForeignKey("TrueFalseId");
                });

            modelBuilder.Entity("LMS.Models.Class", b =>
                {
                    b.HasOne("LMS.Models.Course.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS.Models.ClassEnrolment", b =>
                {
                    b.HasOne("LMS.Models.Class", "Class")
                        .WithMany()
                        .HasForeignKey("ClassId");
                });

            modelBuilder.Entity("LMS.Models.Course.Course", b =>
                {
                    b.HasOne("LMS.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS.Models.SubCategory", "SubCategory")
                        .WithMany()
                        .HasForeignKey("SubCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS.Models.Course.CourseModule", b =>
                {
                    b.HasOne("LMS.Models.Course.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS.Models.Course.CourseTopic", b =>
                {
                    b.HasOne("LMS.Models.Course.CourseUnit", "CourseUnit")
                        .WithMany()
                        .HasForeignKey("CourseUnitId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS.Models.Course.CourseUnitAssignment", b =>
                {
                    b.HasOne("LMS.Models.Course.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS.Models.Course.CourseModule", "CourseModule")
                        .WithMany()
                        .HasForeignKey("CourseModuleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS.Models.Course.CourseUnit", "CourseUnit")
                        .WithMany()
                        .HasForeignKey("CourseUnitId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS.Models.Learning.FormativeSubmission", b =>
                {
                    b.HasOne("LMS.Models.Assessment.Formative", "Formative")
                        .WithMany()
                        .HasForeignKey("FormativeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS.Models.Learning.FormativeSession", "FormativeSession")
                        .WithMany()
                        .HasForeignKey("FormativeSessionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS.Models.Learning.SummativeSubmission", b =>
                {
                    b.HasOne("LMS.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId1");

                    b.HasOne("LMS.Models.Assessment.Summative", "Summative")
                        .WithMany()
                        .HasForeignKey("SummativeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS.Models.Learning.SummativeSession", "SummativeSession")
                        .WithMany()
                        .HasForeignKey("SummativeSessionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS.Models.SubCategory", b =>
                {
                    b.HasOne("LMS.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS.Models.SubmissionValidity", b =>
                {
                    b.HasOne("LMS.Models.Class", "Class")
                        .WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS.Models.Assessment.Summative", "Summative")
                        .WithMany()
                        .HasForeignKey("SummativeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS.Models.Support.SupportResponse", b =>
                {
                    b.HasOne("LMS.Models.Support.SupportRequest", "SupportRequest")
                        .WithMany()
                        .HasForeignKey("SupportRequestId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("LMS.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("LMS.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("LMS.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
