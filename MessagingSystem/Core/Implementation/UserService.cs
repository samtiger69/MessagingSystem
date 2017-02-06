using Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using System.Data.SqlClient;

namespace Core.Implementation
{
    public class UserService : IUserService
    {
        #region Fields
        private IUserTypeService _userTypeService;
        private IOrganizationService _organizationService;
        private const int UNSPECIFIED = -1;
        private IDatabaseExecuter _databaseExecuter;
        #endregion

        #region Constructors
        public UserService()
        {
            _databaseExecuter = DatabaseExecuter.GetInstance();
            _userTypeService = new UserTypeService();
            _organizationService = new OrganizationService();
        }
        #endregion

        #region Actions
        public IList<User> GetUsersBySenderId(int senderId)
        {
            IList<User> result = new List<User>();
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USERS_BY_SENDER_ID,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@SenderId", senderId));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillUsersList(reader);
                    });
            }
            catch(Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public IList<User> GetUsersByAdminId(int adminId)
        {
            IList<User> result = new List<User>();
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USERS_BY_ADMIN_ID,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@adminId", adminId));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillUsersList(reader);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public IList<User> GetAllUsers()
        {
            IList<User> result = new List<User>();
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USERS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@Username", ""));
                        command.Parameters.Add(new SqlParameter("@UserTypeId", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@OrganizationId", UNSPECIFIED));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillUsersList(reader);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public IList<User> GetAllUsersByOrganizationId(int organizationId, UserTypes userType = UserTypes.User)
        {
            IList<User> result = new List<User>();
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USERS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@Username", ""));
                        command.Parameters.Add(new SqlParameter("@UserTypeId", userType));
                        command.Parameters.Add(new SqlParameter("@OrganizationId", organizationId));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillUsersList(reader);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public IList<User> GetAllUsersByTypeId(int typeId)
        {
            IList<User> result = new List<User>();
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USERS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@Username", ""));
                        command.Parameters.Add(new SqlParameter("@UserTypeId", typeId));
                        command.Parameters.Add(new SqlParameter("@OrganizationId", UNSPECIFIED));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillUsersList(reader);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public User GetUserById(int userId)
        {
            User result = null;
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USERS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", userId));
                        command.Parameters.Add(new SqlParameter("@Username", ""));
                        command.Parameters.Add(new SqlParameter("@UserTypeId", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@OrganizationId", UNSPECIFIED));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillUsersList(reader).FirstOrDefault();
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public User GetUserByUsername(string username)
        {
            User result = null;
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USERS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@Username", username));
                        command.Parameters.Add(new SqlParameter("@UserTypeId", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@OrganizationId", UNSPECIFIED));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillUsersList(reader).FirstOrDefault();
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public IList<User> GetMessageReceiversByMessageId(int messageId)
        {
            IList<User> result = new List<User>();
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_MESSAGE_RECEIVERS,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@messageId", messageId));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillUsersList(reader);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public int InsertUser(User user)
        {
            var result = 0;
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.INSERT_USER,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Username", user.Username));
                        command.Parameters.Add(new SqlParameter("@Name", user.Name));
                        command.Parameters.Add(new SqlParameter("@Password", user.Password));
                        command.Parameters.Add(new SqlParameter("@IsActive", user.IsActive));
                        command.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now));
                        command.Parameters.Add(new SqlParameter("@UpdateDate", DateTime.Now));
                        command.Parameters.Add(new SqlParameter("@UserTypeId", user.UserTypeId));
                        command.Parameters.Add(new SqlParameter("@OrganizationId", user.OrganizationId));
                    },
                    delegate (SqlDataReader reader)
                    {
                        reader.Read();
                        result = Convert.ToInt16(reader[0]);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public int UpdateUser(User user)
        {
            var result = 0;
            try
            {
                result = _databaseExecuter.ExecuteChange(StoredProcedures.UPDATE_USER,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", user.Id));
                        command.Parameters.Add(new SqlParameter("@Username", user.Username));
                        command.Parameters.Add(new SqlParameter("@Name", user.Name));
                        command.Parameters.Add(new SqlParameter("@Password", user.Password));
                        command.Parameters.Add(new SqlParameter("@IsActive", user.IsActive));
                        command.Parameters.Add(new SqlParameter("@IsDeleted", user.IsDeleted));
                        command.Parameters.Add(new SqlParameter("@CreateDate", user.CreateDate));
                        command.Parameters.Add(new SqlParameter("@UpdateDate", user.UpdateDate));
                        command.Parameters.Add(new SqlParameter("@UserTypeId", user.UserTypeId));
                        command.Parameters.Add(new SqlParameter("@OrganizationId", user.OrganizationId));
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public int DeleteUser(User user)
        {
            user.IsDeleted = true;
            return UpdateUser(user);
        }

        private IList<User> FillUsersList(SqlDataReader reader)
        {
            IList<User> result = new List<User>();
            while (reader.Read())
            {
                result.Add(new User
                {
                    Id = Convert.ToInt16(reader[0]),
                    Username = reader[1].ToString(),
                    Name = reader[2].ToString(),
                    Password = reader[3].ToString(),
                    IsActive = Convert.ToBoolean(reader[4]),
                    IsDeleted = Convert.ToBoolean(reader[5]),
                    CreateDate = reader[5] != DBNull.Value ? Convert.ToDateTime(reader[6]) : DateTime.Now,
                    UpdateDate = reader[6] != DBNull.Value ? Convert.ToDateTime(reader[7]) : DateTime.Now,
                    UserTypeId = Convert.ToInt16(reader[8]),
                    OrganizationId = Convert.ToInt16(reader[9]),
                    UserType = _userTypeService.GetUserTypeById(Convert.ToInt16(reader[8])),
                    Organization = _organizationService.GetOrganizationById(Convert.ToInt16(reader[9]))
                });
            }
            return result;
        }

        public bool Login(string username, string password)
        {
            var result = false;
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.USER_LOGIN,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@username", username));
                        command.Parameters.Add(new SqlParameter("@password", password));
                    },
                    delegate(SqlDataReader reader)
                    {
                        result = reader.HasRows;
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }
        #endregion
    }
}
