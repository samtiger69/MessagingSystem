using System;
using System.Data.SqlClient;

namespace Core.Abstract
{
    public interface IDatabaseExecuter
    {
        void ExecuteReader(string storedProcedureName, Action<SqlCommand> FillCommandParams, Action<SqlDataReader> FetchData);

        int ExecuteChange(string storedProcedureName, Action<SqlCommand> FillCommandParams);
    }
}
