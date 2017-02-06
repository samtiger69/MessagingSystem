using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Models
{
    public class CreateOrganization
    {
        public CreateOrganization()
        {
            Organizations = new List<SelectListItem>();
        }
        
        [Required]
        [Display(Name="Organization Name")]
        public string Name { get; set; }

        [Display(Name="Parent Organization")]
        public int ParentOrganizationId { get; set; }

        public virtual IList<SelectListItem> Organizations { get; set; }
    }
}