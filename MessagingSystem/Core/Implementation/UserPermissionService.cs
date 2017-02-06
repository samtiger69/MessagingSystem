using Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using System.Data.SqlClient;

namespace Core.Implementation
{
    public class UserPermissionService : IUserPermissionService
    {
        #region Fields
        private const int UNSPECIFIED = -1;
        private IOrganizationService _organizationService;
        private IDatabaseExecuter _databaseExecuter;
        #endregion

        #region Constructors
        public UserPermissionService()
        {
            _databaseExecuter = DatabaseExecuter.GetInstance();
            _organizationService = new OrganizationService();
        }
        #endregion

        #region Actions
        public IList<UserPermission> GetAllUserPermissionsByUserId(int userId, PermissionType permissionType = PermissionType.SendLocal)
        {
            IList<UserPermission> result = new List<UserPermission>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USER_PERMISSIONS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@UserId", userId));
                        command.Parameters.Add(new SqlParameter("@OrganizationId", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@permissionType", permissionType));
                    }
                    ,
                    delegate (SqlDataReader reader)
                    {
                        result = FillUserPermissionList(reader);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public UserPermission GetUserPermissionsByUserIdAndOrganizationId(int userId, int organizationId)
        {
            IList<UserPermission> result = new List<UserPermission>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USER_PERMISSIONS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@UserId", userId));
                        command.Parameters.Add(new SqlParameter("@OrganizationId", organizationId));
                        command.Parameters.Add(new SqlParameter("@permissionType", UNSPECIFIED));
                    }
                    ,
                    delegate (SqlDataReader reader)
                    {
                        result = FillUserPermissionList(reader);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result.FirstOrDefault();
        }

        public IList<UserPermission> GetUserSendingPermissions(int UserId)
        {
            IList<UserPermission> result = new List<UserPermission>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_SENDER_USER_PERMISSIONS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@UserId", UserId));
                    }
                    ,
                    delegate (SqlDataReader reader)
                    {
                        result = FillUserPermissionList(reader);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public IList<UserPermission> GetUserAdminPermissions(int UserId)
        {
            IList<UserPermission> result = new List<UserPermission>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_ADMIN_USER_PERMISSIONS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@UserId", UserId));
                    }
                    ,
                    delegate (SqlDataReader reader)
                    {
                        result = FillUserPermissionList(reader);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public int InsertUserPermission(UserPermission userPermission)
        {
            var result = 0;

            try
            {
                result = _databaseExecuter.ExecuteChange(StoredProcedures.INSERT_USER_PERMISSION,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@UserId", userPermission.UserId));
                        command.Parameters.Add(new SqlParameter("@OrganizationId", userPermission.OrganizationId));
                        command.Parameters.Add(new SqlParameter("@AdminPermission", userPermission.PermissionType));
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public int UpdateUserPermission(UserPermission userPermission)
        {
            var result = 0;
            try
            {
                result = _databaseExecuter.ExecuteChange(StoredProcedures.UPDATE_USER_PERMISSION,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@UserId", userPermission.UserId));
                        command.Parameters.Add(new SqlParameter("@OrganizationId", userPermission.OrganizationId));
                        command.Parameters.Add(new SqlParameter("@PermissionType", userPermission.PermissionType));
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public int DeleteUserPermission(UserPermission userPermission)
        {
            var result = 0;
            try
            {
                result = _databaseExecuter.ExecuteChange(StoredProcedures.DELETE_USER_PERMISSION,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", userPermission.Id));
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        private IList<UserPermission> FillUserPermissionList(SqlDataReader reader)
        {
            IList<UserPermission> result = new List<UserPermission>();
            while (reader.Read())
            {
                result.Add(new UserPermission
                {
                    Id = Convert.ToInt16(reader[0]),
                    OrganizationId = Convert.ToInt16(reader[1]),
                    UserId = Convert.ToInt16(reader[2]),
                    PermissionType = (PermissionType)(reader[3])
                });
            }
            return result;
        }
        #endregion
    }
}
