using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.Course;

namespace LMS.Models.Learning
{
    public class Lesson
    {

        public int Id { get; set; }

        public string LessonType { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public TimeSpan Duration { get; set; }

        public string UserId { get; set; }




    }
}
