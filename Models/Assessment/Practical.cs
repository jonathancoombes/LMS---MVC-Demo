using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Assessment
{
    public class Practical
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string PracticalRequest { get; set; }
        public int SummId { get; set; }

    }
}
