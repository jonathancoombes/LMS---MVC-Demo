using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "SubCategory Name")]
        [Required]
        public string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }

        //Linking the foreign key "CategoryId" above to the Category table:

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public bool? CanDelete { get; set; }
    }
}
