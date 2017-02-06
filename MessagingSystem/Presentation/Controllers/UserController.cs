using Presentation.Models;
using Presentation.ViewModel;
using System;
using System.Web.Mvc;
using System.Web.Security;
using Domain.Entities;
using Presentation.Helpers;

namespace Presentation.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        #region Fields
        private UserHelper _helper;
        private Notifications _notifications;
        #endregion

        public UserController()
        {
            _helper = new UserHelper();
            _notifications = new Notifications(TempData);
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserModel user)
        {
            if (ModelState.IsValid)
            {
                if (_helper.Login(user.UserName, user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, user.RememberMe);
                    _notifications.AddSuccessNotification("Logged in successfully");
                    return RedirectToAction("Index", "Home");
                }
                _notifications.AddErrorNotification("Login data is incorrect!");
                ModelState.AddModelError("", "Login data is incorrect!");
            }
            return View(user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UserList(int? pageNumber)
        {
            var admin = _helper.GetUser(User.Identity.Name);

            if (_helper.IsAdmin(admin.Id))
            {
                var model = _helper.GetUserList(admin, pageNumber);
                if (model.Users.Count == 0)
                {
                    _notifications.AddInfoNotification("No users to display");
                }
                return View(model);
            }
            _notifications.AddErrorNotification("You don't have access to this page.");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult UserList(int[] Ids)
        {
            var admin = _helper.GetUser(User.Identity.Name);
            if (_helper.IsAdmin(admin.Id))
            {
                if (Ids != null)
                {
                    var userList = _helper.GetUserList(admin, null);
                    var deletedCounter = _helper.DeleteUsers(admin, Ids);
                    if (deletedCounter == 1)
                    {
                        _notifications.AddSuccessNotification(String.Format("{0} user has been deleted successfully.", deletedCounter));
                    }
                    if (deletedCounter > 1)
                    {
                        _notifications.AddSuccessNotification(String.Format("{0} users have been deleted successfully.", deletedCounter));
                    }
                    if (deletedCounter != Ids.Length)
                    {
                        _notifications.AddErrorNotification(String.Format("{0} users weren't deleted", Ids.Length - deletedCounter));
                    }
                }
                else
                {
                    _notifications.AddErrorNotification("Select users to delete");
                }
                return RedirectToAction("UserList");
            }
            _notifications.AddErrorNotification("You don't have access to this page.");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AdminList(int? pageNumber)
        {
            var admin = _helper.GetUser(User.Identity.Name);
            if (admin.UserTypeId == (int)UserTypes.SuperAdmin)
            {
                var model = _helper.GetAdminList(null);
                if (model.Users.Count == 0)
                {
                    _notifications.AddInfoNotification("No admins to display");
                }
                return View(model);
            }
            _notifications.AddErrorNotification("You don't have access to this page.");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult AdminList(int[] Ids)
        {
            var user = _helper.GetUser(User.Identity.Name);
            if (user.UserTypeId == (int)UserTypes.SuperAdmin)
            {
                if (Ids != null)
                {
                    var deletedCounter = _helper.DeleteAdmins(Ids);
                    if (deletedCounter == 1)
                    {
                        _notifications.AddSuccessNotification(String.Format("{0} admin has been deleted successfully.", deletedCounter));
                    }
                    if (deletedCounter > 1)
                    {
                        _notifications.AddSuccessNotification(String.Format("{0} admins have been deleted successfully.", deletedCounter));
                    }
                    if (deletedCounter != Ids.Length)
                    {
                        _notifications.AddErrorNotification(String.Format("{0} admins weren't deleted", Ids.Length - deletedCounter));
                    }
                }
                return RedirectToAction("AdminList");
            }
            _notifications.AddErrorNotification("You don't have access to this page.");
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EditUser(int Id)
        {
            var admin = _helper.GetUser(User.Identity.Name);
            // Checks if the logged in user is an admin.
            if (!_helper.IsAdmin(admin.Id))
            {
                _notifications.AddErrorNotification("You can't access this page");
                return RedirectToAction("Index", "Home");
            }

            // Checks if the user to be edited does exist.
            var user = _helper.GetUserById(Id);
            if (user == null)
            {
                _notifications.AddErrorNotification("User not found");
                return RedirectToAction("UserList");
            }

            // Checks if the admin has permission to edit this user.
            if (!_helper.CanEditUser(admin.Id, user.Id))
            {
                _notifications.AddErrorNotification("You can't edit this user");
                return RedirectToAction("UserList");
            }

            var model = _helper.PrepareEditUserModel(admin, user);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditUser(EditUser model, int[] LocalSendingIds, int[] GlobalSendingIds)
        {
            var admin = _helper.GetUser(User.Identity.Name);
            // Checks if the logged user isn't an admin.
            if (!_helper.IsAdmin(admin.Id))
            {
                _notifications.AddErrorNotification("You can't access this page");
                return RedirectToAction("Index", "Home");
            }

            // Checks if the user to be edited doesn't exist.
            var user = _helper.GetUserById(model.Id);
            if (user == null)
            {
                _notifications.AddErrorNotification("User not found");
                return RedirectToAction("UserList");
            }

            // Checks if the admin can't edit the user.
            if (!_helper.CanEditUser(admin.Id, user.Id))
            {
                _notifications.AddErrorNotification("You can't edit this user");
                return RedirectToAction("UserList");
            }

            // Checks if the model isn't valid.
            if (!ModelState.IsValid)
            {
                model = _helper.PrepareEditUserModel(admin, user);
                return View(model);
            }

            // Update the user.
            _helper.UpdateUser(admin, user, model, LocalSendingIds, GlobalSendingIds);
            _notifications.AddSuccessNotification("User has been updated");
            return RedirectToAction("UserList");
        }

        public ActionResult CreateUser()
        {
            var admin = _helper.GetUser(User.Identity.Name);
            var model = new CreateUserViewModel();
            if (_helper.IsAdmin(admin.Id))
            {
                _helper.PrepareCreateUserModel(model, admin);
                return View(model);
            }
            _notifications.AddErrorNotification("You don't have access to this page");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult CreateUser(CreateUserViewModel model, int[] Ids, int[] isGlobals)
        {
            var user = _helper.GetUser(User.Identity.Name);

            // Checks if the logged in user isn't an admin.
            if (!_helper.IsAdmin(user.Id))
            {
                _notifications.AddErrorNotification("You can't access this page");
                return RedirectToAction("Index", "Home");
            }

            // Checks if the model isn't valid.
            if (!ModelState.IsValid)
            {
                _helper.PrepareCreateUserModel(model, user);
                return View(model);
            }

            // Create a user.
            _helper.CreateUser(model, Ids, isGlobals);
            _notifications.AddSuccessNotification("User has been added successfully");
            return RedirectToAction("UserList");
        }

        public ActionResult CreateAdmin()
        {
            var user = _helper.GetUser(User.Identity.Name);
            if (user.UserTypeId != (int)UserTypes.SuperAdmin)
            {
                _notifications.AddErrorNotification("You don't have access to this page");
                return RedirectToAction("Index", "Home");
            }
            var model = new CreateUserViewModel();
            _helper.PrepareCreateUserModel(model, user);
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateAdmin(CreateUserViewModel model, int[] SendingLocalIds, int[] SendingGlobalIds, int[] AdminLocalId, int[] AdminGlobalIds)
        {
            var user = _helper.GetUser(User.Identity.Name);

            // Check if the logged in user isn't a super admin.
            if (user.UserTypeId != (int)UserTypes.SuperAdmin)
            {
                _notifications.AddErrorNotification("You don't have access to this page");
                return RedirectToAction("Index", "Home");
            }

            // Checks if the model isn't valid.
            if (!ModelState.IsValid)
            {
                _helper.PrepareCreateUserModel(model, user);
                return View(model);
            }

            // Create an admin.
            _helper.CreateAdmin(model, SendingLocalIds, SendingGlobalIds, AdminLocalId, AdminGlobalIds);

            _notifications.AddSuccessNotification("Admin has been created successfully");
            return RedirectToAction("AdminList");
        }

        [HttpGet]
        public ActionResult EditAdmin(int Id)
        {
            var superAdmin = _helper.GetUser(User.Identity.Name);

            // Checks if the user isn't a super admin.
            if (superAdmin.UserTypeId != (int)UserTypes.SuperAdmin)
            {
                _notifications.AddErrorNotification("You don't have access to this page");
                return RedirectToAction("Index", "Home");
            }

            var admin = _helper.GetUserById(Id);

            // Checks if the user doesn't exist.
            if (admin == null)
            {
                _notifications.AddErrorNotification("User not found");
                return RedirectToAction("AdminList");
            }

            // Checks if the user isn't an admin.
            if (admin.UserTypeId != (int)UserTypes.Admin)
            {
                _notifications.AddErrorNotification("The user you want to edit is not an admin");
                return RedirectToAction("AdminList");
            }
            var model = _helper.PrepareEditAdminModel(admin);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditAdmin(EditAdmin model, int[] SendingLocalIds, int[] SendingGlobalIds, int[] AdminLocalIds, int[] AdminGlobalIds)
        {
            var superAdmin = _helper.GetUser(User.Identity.Name);

            // Checks if the user isn't a super admin.
            if (superAdmin.UserTypeId != (int)UserTypes.SuperAdmin)
            {
                _notifications.AddErrorNotification("You don't have access to this page");
                return RedirectToAction("Index", "Home");
            }

            // Checks if the user to be edited doesn't exist.
            var admin = _helper.GetUserById(model.Id);
            if (admin == null)
            {
                _notifications.AddErrorNotification("User not found");
                return RedirectToAction("AdminList");
            }

            // Checks if the user to be edited isn't an admin.
            if (admin.UserTypeId != (int)UserTypes.Admin)
            {
                _notifications.AddErrorNotification("The user you want to edit is not an admin");
                return RedirectToAction("AdminList");
            }

            // Checks if the model isn't valid.
            if (!ModelState.IsValid)
            {
                model = _helper.PrepareEditAdminModel(admin);
                return View(model);
            }

            _helper.UpdateAdmin(model, admin, SendingLocalIds, SendingGlobalIds, AdminLocalIds, AdminGlobalIds);

            _notifications.AddSuccessNotification("Admin has been updated successfully");
            return RedirectToAction("AdminList");
        }
    }
}