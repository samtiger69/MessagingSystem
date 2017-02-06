using Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using System.Data.SqlClient;

namespace Core.Implementation
{
    public class UserTypeService : IUserTypeService
    {
        #region Fields
        private const int UNSPECIFIED = -1;
        private IDatabaseExecuter _databaseExecuter;
        #endregion

        #region Constructors
        public UserTypeService()
        {
            _databaseExecuter = DatabaseExecuter.GetInstance();
        }
        #endregion

        #region Actions
        public IList<UserType> GetAllUserTypes()
        {
            IList<UserType> result = new List<UserType>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USER_TYPES,
                    delegate(SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@Name", ""));
                        command.Parameters.Add(new SqlParameter("@UserId", UNSPECIFIED));
                    }
                    ,

                    delegate(SqlDataReader reader)
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

        public UserType GetUserTypeById(int UserTypeId)
        {
            IList<UserType> result = new List<UserType>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USER_TYPES,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", UserTypeId));
                        command.Parameters.Add(new SqlParameter("@Name", ""));
                        command.Parameters.Add(new SqlParameter("@UserId", UNSPECIFIED));
                    }
                    ,

                    delegate (SqlDataReader reader)
                    {
                        result = FillUsersList(reader);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result.FirstOrDefault();
        }

        public UserType GetUserTypeByName(string Name)
        {
            IList<UserType> result = new List<UserType>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USER_TYPES,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@Name", Name));
                        command.Parameters.Add(new SqlParameter("@UserId", UNSPECIFIED));
                    }
                    ,
                    delegate (SqlDataReader reader)
                    {
                        result = FillUsersList(reader);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result.FirstOrDefault();
        }

        public UserType GetUserTypeByUserId(int UserId)
        {

            IList<UserType> result = new List<UserType>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_USER_TYPES,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@Name", ""));
                        command.Parameters.Add(new SqlParameter("@UserId", UserId));
                    }
                    ,

                    delegate (SqlDataReader reader)
                    {
                        result = FillUsersList(reader);
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result.FirstOrDefault();
        }

        public int InsesrtUserType(UserType userType)
        {
            var result = 0;

            try
            {
                result = _databaseExecuter.ExecuteChange(StoredProcedures.INSERT_USER_TYPE,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Name", userType.Name));
                    });
            }
            catch(Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public int UpdateUserType(UserType userType)
        {
            var result = 0;

            try
            {
                result = _databaseExecuter.ExecuteChange(StoredProcedures.UPDATE_USER_TYPE,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", userType.Id));
                        command.Parameters.Add(new SqlParameter("@Name", userType.Name));
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public int DeleteUserType(UserType userType)
        {
            var result = 0;

            try
            {
                result = _databaseExecuter.ExecuteChange(StoredProcedures.DELETE_USER_TYPE,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", userType.Id));
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        private IList<UserType> FillUsersList(SqlDataReader reader)
        {
            IList<UserType> result = new List<UserType>();
            while (reader.Read())
            {
                result.Add(new UserType
                {
                    Id = Convert.ToInt16(reader[0]),
                    Name = reader[1].ToString()
                });
            }
            return result;
        }
        #endregion
    }
}
