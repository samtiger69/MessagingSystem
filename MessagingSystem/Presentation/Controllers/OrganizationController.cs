using Domain.Entities;
using Presentation.Helpers;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    [Authorize]
    public class OrganizationController : Controller
    {
        #region Fields
        private OrganizationHelper _helper;
        private Notifications _notifications;
        #endregion

        public OrganizationController()
        {
            _notifications = new Notifications(TempData);
            _helper = new OrganizationHelper();
        }
        public ActionResult Index()
        {
            var user = _helper.GetUser(User.Identity.Name);
            if (user.UserTypeId != (int)UserTypes.SuperAdmin)
            {
                _notifications.AddErrorNotification("You don't have access to this page.");
                return RedirectToAction("Index", "Home");
            }
            var organization = _helper.GetAllOrganizations();
            return View(organization);
        }

        public ActionResult Edit(int organizationId)
        {
            var user = _helper.GetUser(User.Identity.Name);
            if (user.UserTypeId != (int)UserTypes.SuperAdmin)
            {
                _notifications.AddErrorNotification("You don't have access to this page.");
                return RedirectToAction("Index", "Home");
            }

            var organization = _helper.GetOrganizationById(organizationId);
            if (organization == null)
            {
                _notifications.AddErrorNotification("Organization not found.");
                return RedirectToAction("Index");
            }

            var model = _helper.PrepareEditOrganizationModel(organizationId);       
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EditOrganization model)
        {
            var admin = _helper.GetUser(User.Identity.Name);

            // Check if the user isn't a super admin.
            if ((admin.UserTypeId != (int)UserTypes.SuperAdmin))
            {
                _notifications.AddErrorNotification("You don't have access to this page.");
                return RedirectToAction("Index", "Home");
            }

            // Checks if the model isn't valid.
            if (!ModelState.IsValid)
            {
                model = _helper.PrepareEditOrganizationModel(model.Id);
                return View(model);
            }

            var orgainzation = _helper.GetOrganizationById(model.Id);

            // Checks the existance of the organization.
            if(orgainzation == null)
            {
                _notifications.AddErrorNotification("Organization not found.");
                return RedirectToAction("Index");
            }

            // checks if the organization is added as a child to one of it's own children.
            if (_helper.IsOrganizationChild(_helper.GetOrganizationById(model.ParentOrganizationId), model.Id))
            {
                model = _helper.PrepareEditOrganizationModel(model.Id);
                _notifications.AddErrorNotification("you can assign the parent as a child of one of his children");
                return View(model);
            }

            _helper.UpdateOrganization(orgainzation, model);

            _notifications.AddSuccessNotification("Organization has been edited successfully.");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            var admin = _helper.GetUser(User.Identity.Name);

            // Check if the user isn't a super admin.
            if (admin.UserTypeId != (int)UserTypes.SuperAdmin)
            {
                _notifications.AddErrorNotification("You don't have access to this page.");
                return RedirectToAction("Index", "Home");
            }

            var model = _helper.PrepareCreateOrganizationModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CreateOrganization model)
        {
            var admin = _helper.GetUser(User.Identity.Name);
            
            // Check if the user isn't a super admin.
            if (admin.UserTypeId != (int)UserTypes.SuperAdmin)
            {
                _notifications.AddErrorNotification("You don't have access to this page.");
                return RedirectToAction("Index", "Home");
            }

            // Checks if the model isn't valid.
            if (!ModelState.IsValid)
            {
                model = _helper.PrepareCreateOrganizationModel();
                return View(model);
            }

            // Insert the organization in the database.
            _helper.AddOrganization(model);

            _notifications.AddSuccessNotification("Organization has been created successfully.");
            return RedirectToAction("Index");
        }
    }
}