using Core.Abstract;
using Core.Implementation;
using Domain.Entities;
using Presentation.Models;
using Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Helpers
{
    public class UserHelper : BaseHelper
    {
        #region Fields
        private IOrganizationService _organizationService;
        private IUserPermissionService _userPermissionService;
        private IUserService _userService;
        #endregion

        public UserHelper()
            :base()
        {
            _organizationService = new OrganizationService();
            _userPermissionService = new UserPermissionService();
            _userService = new UserService();
        }


        public bool CanEditUser(int admin, int user)
        {
            var users = _userService.GetUsersByAdminId(admin);
            return (users.FirstOrDefault(m => m.Id == user) != null) && IsAdmin(admin);
        }

        public int DeleteUsers(User admin, int[] Ids)
        {
            var deletedCounter = 0;
            foreach (var userId in Ids)
            {
                var toBeDeletedUser = _userService.GetUserById(userId);
                if(toBeDeletedUser != null)
                {
                    if(CanEditUser(admin.Id,toBeDeletedUser.Id))
                    {
                        _userService.DeleteUser(toBeDeletedUser);
                        deletedCounter++;
                    }
                }
            }
            return deletedCounter;
        }

        internal bool Login(string userName, string password)
        {
            return _userService.Login(userName, password);
        }

        public bool IsAdmin(int adminId)
        {
            var admin = _userService.GetUserById(adminId);
            return (admin.UserTypeId == (int)UserTypes.Admin || (admin.UserTypeId == (int)UserTypes.SuperAdmin));
        }

        public int DeleteAdmins(int[] Ids)
        {
            var deletedCounter = 0;
            foreach (var userId in Ids)
            {
                var ToBeDeletedUser = _userService.GetUserById(userId);
                if (ToBeDeletedUser != null && ToBeDeletedUser.UserTypeId == (int)UserTypes.Admin)
                {
                    _userService.DeleteUser(ToBeDeletedUser);
                    deletedCounter++;
                }
            }
            return deletedCounter;
        }

        public UserList GetAdminList(int? pageNumber)
        {
            var model = new UserList();
            var users = _userService.GetAllUsersByTypeId((int)UserTypes.Admin);
            model.PageNumber = (pageNumber == null ? 1 : Convert.ToInt32(pageNumber));
            model.PageSize = 10;
            if (users != null)
            {
                model.Users = users.OrderBy(x => x.Id)
                         .Skip(model.PageSize * (model.PageNumber - 1))
                         .Take(model.PageSize).ToList();

                model.TotalCount = users.Count();
                var page = (model.TotalCount / model.PageSize) - (model.TotalCount % model.PageSize == 0 ? 1 : 0);
                model.PagerCount = page + 1;
            }
            return model;
        }

        public UserList GetUserList(User admin, int? pageNumber)
        {
            var users = _userService.GetUsersByAdminId(admin.Id);
            var model = new UserList();
            model.PageNumber = (pageNumber == null ? 1 : Convert.ToInt32(pageNumber));
            model.PageSize = 10;
            if (users != null)
            {
                model.Users = users.OrderBy(x => x.Id)
                         .Skip(model.PageSize * (model.PageNumber - 1))
                         .Take(model.PageSize).ToList();

                model.TotalCount = users.Count();
                var page = (model.TotalCount / model.PageSize) - (model.TotalCount % model.PageSize == 0 ? 1 : 0);
                model.PagerCount = page + 1;
            }
            return model;
        }

        public void MapPermissionsToOrganization(Organization organization, IList<UserPermission> userPermissions, IList<UserPermission> hasAccessTo)
        {
            if (organization != null)
            {
                var permission = userPermissions.FirstOrDefault(m => m.OrganizationId == organization.Id);
                if (permission != null)
                {
                    if ((permission.PermissionType & PermissionType.AdminGlobal) == PermissionType.AdminGlobal)
                    {
                        hasAccessTo.Add(new UserPermission { OrganizationId = organization.Id, PermissionType = PermissionType.AdminGlobal });
                        DFS(organization, hasAccessTo);
                    }
                    else
                    {
                        hasAccessTo.Add(new UserPermission { OrganizationId = organization.Id, PermissionType = PermissionType.AdminLocal });
                    }
                }
                foreach (var child in organization.Children)
                {
                    MapPermissionsToOrganization(child, userPermissions, hasAccessTo);
                }
            }
        }

        private void DFS(Organization organization, IList<UserPermission> hasAccessTo)
        {
            if (organization != null)
            {
                hasAccessTo.Add(new UserPermission { OrganizationId = organization.Id, PermissionType = PermissionType.AdminGlobal });
                foreach (var child in organization.Children)
                {
                    DFS(child, hasAccessTo);
                }
            }
        }

        public void PrepareCreateUserModel(CreateUserViewModel model, User user)
        {
            var Organizations = _organizationService.GetOrganizationById(1);
            var UserPermissions = _userPermissionService.GetAllUserPermissionsByUserId(user.Id, PermissionType.AdminLocal);
            var hasAccessTo = new List<UserPermission>();
            MapPermissionsToOrganization(Organizations, UserPermissions, hasAccessTo);
            var userOrganizations = _organizationService.GetAllOrganizationsByUserId(user.Id, PermissionType.AdminLocal);
            foreach (var organization in userOrganizations)
                model.Organizations.Add(new SelectListItem
                {
                    Value = organization.Id.ToString(),
                    Text = organization.Name
                });
            model.OrganizationWithAccessTo = hasAccessTo;
            model.root = Organizations;
        }

        public void GrantUserPermissions(int[] organizationIds, PermissionType permissionsType, int userId)
        {
            if (organizationIds != null)
            {
                foreach (var organizationId in organizationIds)
                {
                    var permission = _userPermissionService.GetUserPermissionsByUserIdAndOrganizationId(userId, organizationId);
                    if (permission != null)
                    {
                        permission.PermissionType = permission.PermissionType | permissionsType;
                        _userPermissionService.UpdateUserPermission(permission);
                    }
                    else
                    {
                        var userPermission = new UserPermission
                        {
                            OrganizationId = organizationId,
                            UserId = userId,
                            PermissionType = permissionsType
                        };
                        _userPermissionService.InsertUserPermission(userPermission);
                    }
                }
            }
        }

        public void RemoveAdminPermissions(int[] Permissions, User admin, PermissionType permissionType)
        {
            if (Permissions != null)
            {
                var userPermissions = _userPermissionService.GetAllUserPermissionsByUserId(admin.Id, permissionType);
                foreach (var permission in userPermissions)
                {
                    if (Permissions.FirstOrDefault(m => m == permission.OrganizationId) == 0)
                    {
                        permission.PermissionType -= permissionType;
                        _userPermissionService.UpdateUserPermission(permission);
                    }
                }
            }
            else
            {
                var userPermissions = _userPermissionService.GetAllUserPermissionsByUserId(admin.Id, permissionType);
                foreach (var permission in userPermissions)
                {
                    permission.PermissionType -= permissionType;
                    _userPermissionService.UpdateUserPermission(permission);
                }
            }
        }

        public EditAdmin PrepareEditAdminModel(User admin)
        {
            var model = new EditAdmin();
            model.Name = admin.Name;
            model.Username = admin.Username;
            model.IsActive = admin.IsActive;
            model.Password = admin.Password;
            model.OrganizationId = admin.OrganizationId;
            model.Id = admin.Id;


            model.AdminOrganizations = _organizationService.GetAllOrganizations();
            model.AdminPermissions = _userPermissionService.GetUserAdminPermissions(admin.Id);
            model.UserPermissions = _userPermissionService.GetUserSendingPermissions(admin.Id);
            model.Root = _organizationService.GetOrganizationById(1);
            foreach (var organization in model.AdminOrganizations)
            {
                model.OrganizationList.Add(new SelectListItem
                {
                    Value = organization.Id.ToString(),
                    Text = organization.Name
                });
            }
            return model;
        }

        public EditUser PrepareEditUserModel(User admin, User user)
        {
            var model = new EditUser();
            model.Id = user.Id;
            model.Name = user.Name;
            model.Username = user.Username;
            model.OrganizationId = user.OrganizationId;
            model.IsActive = user.IsActive;
            model.Password = user.Password;

            model.AdminOrganizations = _organizationService.GetAllOrganizationsByUserId(admin.Id, PermissionType.AdminLocal);
            MapPermissionsToOrganization(_organizationService.GetOrganizationById(1), _userPermissionService.GetUserAdminPermissions(admin.Id), model.AdminPermissions);
            model.UserPermissions = _userPermissionService.GetUserSendingPermissions(user.Id);
            var userOrganizations = _organizationService.GetAllOrganizationsByUserId(user.Id, PermissionType.SendLocal).Where(m => model.UserPermissions.FirstOrDefault(x => x.OrganizationId == m.Id) != null);
            model.AdminOrganizations = model.AdminOrganizations.Union(userOrganizations).ToList();

            foreach (var org in model.AdminOrganizations)
            {
                model.OrganizationList.Add(new SelectListItem
                {
                    Value = org.Id.ToString(),
                    Text = org.Name
                });
            }
            return model;
        }

        public void UpdateUser(User admin, User user, EditUser model, int[] localSendingIds, int[] globalSendingIds)
        {
            user.Name = model.Name;
            user.Username = model.Username;
            user.OrganizationId = model.OrganizationId;
            user.IsActive = user.IsActive;
            user.UpdateDate = DateTime.Now;
            if (model.Password != null)
            {
                user.Password = model.Password;
            }

            _userService.UpdateUser(user);

            // Remove Global sending permissions.
            RemoveAdminPermissions(globalSendingIds, user, PermissionType.SendGlobal);

            // Remove Local sending permissions.
            RemoveAdminPermissions(localSendingIds, user, PermissionType.SendLocal);

            // Grant local sending permissions.
            GrantUserPermissions(localSendingIds, PermissionType.SendLocal, user.Id);

            // Grant global sending permissions.
            GrantUserPermissions(globalSendingIds, PermissionType.SendGlobal, user.Id);
        }

        public void CreateUser(CreateUserViewModel model, int[] ids, int[] isGlobals)
        {
            var NewUser = new User
            {
                Name = model.Name,
                Password = model.Password,
                Username = model.Username,
                OrganizationId = model.OrganizationId,
                IsActive = model.IsActive,
                UserTypeId = (int)UserTypes.User
            };
            var userId = _userService.InsertUser(NewUser);
            var localPermission = new UserPermission { UserId = userId, OrganizationId = model.OrganizationId, PermissionType = PermissionType.SendLocal };
            _userPermissionService.InsertUserPermission(localPermission);
            // Grant user Local sending permissions
            GrantUserPermissions(ids, PermissionType.SendLocal, userId);

            // Grant user Global sending permissions
            GrantUserPermissions(isGlobals, PermissionType.SendGlobal, userId);
        }

        public void CreateAdmin(CreateUserViewModel model, int[] sendingLocalIds, int[] sendingGlobalIds, int[] adminLocalId, int[] adminGlobalIds)
        {
            var NewUser = new User
            {
                Name = model.Name,
                Password = model.Password,
                Username = model.Username,
                OrganizationId = model.OrganizationId,
                IsActive = model.IsActive,
                UserTypeId = (int)UserTypes.Admin
            };
            var userId = _userService.InsertUser(NewUser);

            // local sending permission on the permission
            var localPermission = new UserPermission { UserId = userId, OrganizationId = model.OrganizationId, PermissionType = PermissionType.SendLocal };

            _userPermissionService.InsertUserPermission(localPermission);

            // Giving the admin sending permissions
            GrantUserPermissions(sendingLocalIds, PermissionType.SendLocal, userId);

            // Giving the admin global sending permissions
            GrantUserPermissions(sendingGlobalIds, PermissionType.SendGlobal, userId);

            // Giving the admin local admin permissions
            GrantUserPermissions(adminLocalId, PermissionType.AdminLocal, userId);

            // Giving the admin global admin permissions
            GrantUserPermissions(adminGlobalIds, PermissionType.AdminGlobal, userId);
        }

        public void UpdateAdmin(EditAdmin model, User admin, int[] sendingLocalIds, int[] sendingGlobalIds, int[] adminLocalIds, int[] adminGlobalIds)
        {
            admin.Name = model.Name;
            admin.Username = model.Username;
            admin.IsActive = model.IsActive;
            admin.UpdateDate = DateTime.Now;
            admin.OrganizationId = model.OrganizationId;
            if (model.Password != null)
            {
                admin.Password = model.Password;
            }
            // remove local sending permissions
            RemoveAdminPermissions(sendingLocalIds, admin, PermissionType.SendLocal);

            // remove global sending permissions
            RemoveAdminPermissions(sendingGlobalIds, admin, PermissionType.SendGlobal);

            // remove local admin permissions
            RemoveAdminPermissions(adminLocalIds, admin, PermissionType.AdminLocal);

            // remove global admin permissions
            RemoveAdminPermissions(adminGlobalIds, admin, PermissionType.AdminGlobal);

            // local sending Permissions.
            GrantUserPermissions(sendingLocalIds, PermissionType.SendLocal, admin.Id);

            // global sending permissions.
            GrantUserPermissions(sendingGlobalIds, PermissionType.SendGlobal, admin.Id);

            // local admin permissions.
            GrantUserPermissions(adminLocalIds, PermissionType.AdminLocal, admin.Id);

            // global admin permissions.
            GrantUserPermissions(adminGlobalIds, PermissionType.AdminGlobal, admin.Id);
        }
    }
}