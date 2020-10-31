using LMS.Models.Assessment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.Learning
{
    public class FormativeSubmission
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int FormativeId { get; set; }
        public virtual Formative Formative { get; set; }
    
        public string MCAnswer { get; set; }
        public string TFAnswer { get; set; }
        public string Result { get; set; }

        public int FormativeSessionId { get; set; }
        public virtual FormativeSession FormativeSession { get; set; }
        

    }
}
