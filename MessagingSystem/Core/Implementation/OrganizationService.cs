using Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using System.Data.SqlClient;

namespace Core.Implementation
{
    public class OrganizationService : IOrganizationService
    {
        #region Fields
        private const int UNSPECIFIED = -1;
        private IDatabaseExecuter _databaseExecuter;
        #endregion

        #region Constructors
        public OrganizationService()
        {
            _databaseExecuter = DatabaseExecuter.GetInstance();
        }
        #endregion

        #region Actions
        public int DeleteOrganization(Organization organization)
        {
            organization.IsDeleted = true;
            return UpdateOrganization(organization);
        }

        public IList<Organization> GetAllOrganizations(bool isDeleted = false)
        {
            IList<Organization> result = new List<Organization>();
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_ORGANIZATIONS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@Name", ""));
                        command.Parameters.Add(new SqlParameter("@IsDeleted", isDeleted));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillOrganizationListWithoutChildren(reader);
                        AssignOrganizationParent(result);
                    });
            }
            catch(Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public IList<Organization> GetAllOrganizationsByUserId(int userId, PermissionType permissionType = PermissionType.SendLocal, bool isDeleted = false)
        {
            IList<Organization> result = new List<Organization>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USER_ORGANIZATIONS_PERMISSIONS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@UserId", userId));
                        command.Parameters.Add(new SqlParameter("@permissionType", permissionType));
                        command.Parameters.Add(new SqlParameter("@IsDeleted", isDeleted));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillOrganizationListWithoutChildren(reader);
                    });
            }
            catch(Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public IList<Organization> GetAllChildrenOrganization(int parentOrganizationId, bool isDeleted = false)
        {
            IList<Organization> result = new List<Organization>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_ORGANIZATION_CHILDREN,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@ParentOrganizationId", parentOrganizationId));
                        command.Parameters.Add(new SqlParameter("@IsDeleted", isDeleted));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillOrganizationList(reader);
                    });
            }
            catch(Exception e)
            {
                // Error logging.
            }

            return result;
        }

        public Organization GetOrganizationById(int organizationId, bool isDeleted = false)
        {
            Organization result = null;
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_ORGANIZATIONS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", organizationId));
                        command.Parameters.Add(new SqlParameter("@Name", ""));
                        command.Parameters.Add(new SqlParameter("@IsDeleted", isDeleted));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillOrganizationList(reader).FirstOrDefault();
                    });
            }
            catch(Exception e)
            {
                // Error logging.
            }

            return result;
        }

        public Organization GetOrganizationByName(string organizationName, bool isDeleted = false)
        {
            Organization result = null;
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_ORGANIZATIONS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@Name", organizationName));
                        command.Parameters.Add(new SqlParameter("@IsDeleted", isDeleted));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillOrganizationList(reader).FirstOrDefault();
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }

            return result;
        }

        public int InsertOrganization(Organization organization)
        {
            var result = 0;
            try
            {
                result = _databaseExecuter.ExecuteChange(StoredProcedures.INSERT_ORGANIZATION,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Name", organization.Name));
                        command.Parameters.Add(new SqlParameter("@ParentOrganizationId", organization.ParentOrganizationId));
                    });
            }
            catch(Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public int UpdateOrganization(Organization organization)
        {
            var result = 0;
            try
            {
                result = _databaseExecuter.ExecuteChange(StoredProcedures.UPDATE_ORGANIZATION,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", organization.Id));
                        command.Parameters.Add(new SqlParameter("@Name", organization.Name));
                        command.Parameters.Add(new SqlParameter("@ParentOrganizationId", organization.ParentOrganizationId));
                        command.Parameters.Add(new SqlParameter("@IsDeleted", organization.IsDeleted));
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        private IList<Organization> FillOrganizationList(SqlDataReader reader)
        {
            IList<Organization> result = new List<Organization>();
            while (reader.Read())
            {
                result.Add(new Organization
                {
                    Id = Convert.ToInt16(reader[0]),
                    Name = reader[1].ToString(),
                    ParentOrganizationId = Convert.ToInt16(reader[2]),
                    IsDeleted = Convert.ToBoolean(reader[3]),
                    Children = GetAllChildrenOrganization(Convert.ToInt16(reader[0]))
                });
            }
            return result;
        }

        private IList<Organization> FillOrganizationListWithoutChildren(SqlDataReader reader)
        {
            IList<Organization> result = new List<Organization>();
            while (reader.Read())
            {
                result.Add(new Organization
                {
                    Id = Convert.ToInt16(reader[0]),
                    Name = reader[1].ToString(),
                    ParentOrganizationId = Convert.ToInt16(reader[2]),
                    IsDeleted = Convert.ToBoolean(reader[3])
                });
            }
            return result;
        }

        private void AssignOrganizationParent(IList<Organization> result)
        {
            if (result != null)
            {
                foreach (var child in result)
                {
                    child.ParentOrganization = result.FirstOrDefault(m => m.Id == child.ParentOrganizationId);
                }
            }
        }

        public bool IsOrganizationChild(Organization parent, int childId)
        {
            if (parent == null || parent.Name == "")
                return false;
            if (parent.ParentOrganizationId == childId)
                return true;
            return IsOrganizationChild(GetOrganizationById(parent.ParentOrganizationId), childId);
        }
        #endregion
    }
}
