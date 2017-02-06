using Domain.Entities;
using System.Collections.Generic;

namespace Core.Abstract
{
    public interface IUserPermissionService
    {
        // Takes an int parameter, and returns the permissions of that user id.
        IList<UserPermission> GetAllUserPermissionsByUserId(int UserId, PermissionType permissionType = PermissionType.SendLocal);

        // Takes a UserPermission object and inserts it into the database.
        int InsertUserPermission(UserPermission userPermission);

        // Takes a UserPermission object and updates it in the database.
        int UpdateUserPermission(UserPermission usesrPermission);

        // Takes a UserPermission object and removes it from the database.
        int DeleteUserPermission(UserPermission userPermission);

        // Get UserPermission by userId and organizationId
        UserPermission GetUserPermissionsByUserIdAndOrganizationId(int userId, int organizationId);

        // Get User Sending permissions (local + global)
        IList<UserPermission> GetUserSendingPermissions(int UserId);

        // Get User admin permissions (local + global)
        IList<UserPermission> GetUserAdminPermissions(int UserId);
    }
}
