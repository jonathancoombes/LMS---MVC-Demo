using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Assessment
{
    public class TrueFalse
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public bool CorrectAnswer { get; set; }

    }
}
