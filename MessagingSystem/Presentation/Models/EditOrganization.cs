using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Models
{
    public class EditOrganization
    {
        public EditOrganization()
        {
            Organizations = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name ="Parent Organization")]
        public int ParentOrganizationId { get; set; }
        public virtual IList<SelectListItem> Organizations { get; set; }
    }
}