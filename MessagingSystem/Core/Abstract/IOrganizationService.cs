using Domain.Entities;
using System.Collections.Generic;

namespace Core.Abstract
{
    public interface IOrganizationService
    {

        // Return all the children of an organization.
        IList<Organization> GetAllChildrenOrganization(int parentOrganizationId, bool isDeleted = false);

        // Returns all the organizations.
        IList<Organization> GetAllOrganizations(bool isDeleted = false);

        // Returns the Organizations where the User has permission
        IList<Organization> GetAllOrganizationsByUserId(int userId, PermissionType permissionType = PermissionType.SendLocal, bool isDeleted = false);

        // Takes an int parameter, and returns the organization with that id or null if not found.
        Organization GetOrganizationById(int organizationId, bool isDeleted = false);

        // Takes a string parameter, and returns the organization with that name or null if not found.
        Organization GetOrganizationByName(string organizationName, bool isDeleted = false);

        // Takes an organization object and inserts it into the database.
        int InsertOrganization(Organization organization);

        // Takes an organization object and updates it in the database.
        int UpdateOrganization(Organization organization);

        // Takes an organization object and removes it from the database.
        int DeleteOrganization(Organization organization);

        // Checkes whether the Parent of an organization is one of it's children
        bool IsOrganizationChild(Organization parent, int childId);
    }
}
