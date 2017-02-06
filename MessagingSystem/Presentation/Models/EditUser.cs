using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Models
{
    public class EditUser
    {
        public EditUser()
        {
            AdminOrganizations = new List<Organization>();
            AdminPermissions = new List<UserPermission>();
            UserPermissions = new List<UserPermission>();
            OrganizationList = new List<SelectListItem>();
            AdminUserOrganizations = new List<Organization>();
        }

        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public virtual IList<Organization> AdminOrganizations { get; set; }

        public virtual IList<Organization> AdminUserOrganizations { get; set; }

        public virtual IList<SelectListItem> OrganizationList { get; set; }

        public virtual IList<UserPermission> AdminPermissions { get; set; }

        public virtual IList<UserPermission> UserPermissions { get; set; }

        public virtual Organization Root { get; set; }
    }
}