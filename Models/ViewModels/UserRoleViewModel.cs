using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.ViewModels
{
    public class UserRoleViewModel
    {

        public IEnumerable <ApplicationUser> Assessors{ get; set; }
        public IEnumerable <ApplicationUser> Administrators{ get; set; }
        public IEnumerable <ApplicationUser> Facilitators { get; set; }
        public IEnumerable<ApplicationUser> Designers { get; set; }
        public IEnumerable<ApplicationUser> Learners { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public bool AdminSelected { get; set; }
        public bool FacilitatorSelected { get; set; }
        public bool AssessorSelected { get; set; }
        public bool DesignerSelected { get; set; }
        public string DisplayRole { get; set; }
        public bool EditorIsAdmin { get; set; }
    }
}
