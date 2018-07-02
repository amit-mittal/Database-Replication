using ReplicationService.Contract;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ReplicationService.DatabaseAdapters
{
    public class SqlStrategy : IStrategy
    {
        private string connectionString;

        public SqlStrategy(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DbCommand GetCommand(string command, DbConnection connection)
        {
            var sqlCommand = new SqlCommand(command, connection as SqlConnection);
            return sqlCommand;
        }

        public DbConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(this.connectionString);

            return connection;
        }

        public DbParameter GetDateTimeParameter(string paramterName, DateTime value)
        {
            SqlParameter sqlParameter = new SqlParameter(paramterName, SqlDbType.DateTime2);
            sqlParameter.Value = value;

            return sqlParameter;
        }
    }
}
