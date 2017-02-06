using Core.Abstract;
using Core.Implementation;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Presentation.Models;
using System.Web.Mvc;

namespace Presentation.Helpers
{
    public class OrganizationHelper: BaseHelper
    {
        #region fields
        private IOrganizationService _organizationService;
        private IUserService _userService;
        #endregion

        public OrganizationHelper()
            :base()
        {
            _organizationService = new OrganizationService();
            _userService = new UserService();
        }

        public IList<Organization> GetAllOrganizations()
        {
            return _organizationService.GetAllOrganizations();
        }

        public Organization GetOrganizationById(int organizationId)
        {
            return _organizationService.GetOrganizationById(organizationId);
        }

        public EditOrganization PrepareEditOrganizationModel(int organizationId)
        {
            var model = new EditOrganization();
            var organization = GetOrganizationById(organizationId);
            model.Id = organization.Id;
            model.Name = organization.Name;
            model.ParentOrganizationId = organization.ParentOrganizationId;

            foreach (var org in GetAllOrganizations())
            {
                model.Organizations.Add(new SelectListItem
                {
                    Value = org.Id.ToString(),
                    Text = org.Name
                });
            }

            return model;
        }

        public bool IsOrganizationChild(Organization parentOrganization, int childOrganizationId)
        {
            return _organizationService.IsOrganizationChild(parentOrganization, childOrganizationId);
        }

        public void UpdateOrganization(Organization orgainzation, EditOrganization model)
        {
            orgainzation.Name = model.Name;
            orgainzation.ParentOrganizationId = model.ParentOrganizationId;
            _organizationService.UpdateOrganization(orgainzation);
        }

        public CreateOrganization PrepareCreateOrganizationModel()
        {
            var model = new CreateOrganization();
            foreach (var org in _organizationService.GetAllOrganizations())
            {
                model.Organizations.Add(new SelectListItem
                {
                    Value = org.Id.ToString(),
                    Text = org.Name
                });
            }
            return model;
        }

        public void AddOrganization(CreateOrganization model)
        {
            var organization = new Organization
            {
                Name = model.Name,
                ParentOrganizationId = model.ParentOrganizationId
            };

            _organizationService.InsertOrganization(organization);
        }
    }
}