using Domain.Entities;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.ViewModel
{
    public class CreateUserViewModel
    {
        public CreateUserViewModel()
        {
            OrganizationWithAccessTo = new List<UserPermission>();
            Organizations = new List<SelectListItem>();
        }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public virtual IList<SelectListItem> Organizations { get; set; }
        public virtual IList<UserPermission> OrganizationWithAccessTo { get; set; }
        public virtual Organization root { get; set; }
    }
}