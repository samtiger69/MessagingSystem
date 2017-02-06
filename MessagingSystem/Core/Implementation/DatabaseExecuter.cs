using Core.Abstract;
using System;
using System.Data.SqlClient;
using System.Configuration;

namespace Core.Implementation
{
    public class DatabaseExecuter : IDatabaseExecuter
    {
        #region Fields
        private static volatile DatabaseExecuter instance;
        private static object syncRoot = new Object();
        #endregion

        #region Constructors
        private DatabaseExecuter()
        {
        }
        #endregion

        #region Actions
        public static DatabaseExecuter GetInstance()
        {
            lock (syncRoot)
            {
                if (instance == null)
                {
                    return instance = new DatabaseExecuter();
                }
                return instance;
            }
        }

        private string CONNECTION_STRING = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public int ExecuteChange(string storedProcedureName, Action<SqlCommand> FillCommandParams)
        {
            var result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = storedProcedureName;
                    FillCommandParams(cmd);
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                throw;

            }
            return result;
        }

        public void ExecuteReader(string storedProcedureName, Action<SqlCommand> FillCommandParams, Action<SqlDataReader> FetchData)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = storedProcedureName;
                    FillCommandParams(cmd);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        FetchData(reader);
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        #endregion
    }
}
