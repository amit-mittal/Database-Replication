using MySql.Data.MySqlClient;
using ReplicationService.Contract;
using System;
using System.Data.Common;

namespace ReplicationService.DatabaseAdapters
{
    public class MySqlStrategy : IStrategy
    {
        private string connectionString;

        public MySqlStrategy(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DbCommand GetCommand(string command, DbConnection connection)
        {
            MySqlCommand sqlCommand = new MySqlCommand(command, connection as MySqlConnection);
            return sqlCommand;
        }

        public DbConnection GetConnection()
        {
            MySqlConnection connection = new MySqlConnection(this.connectionString);

            return connection;
        }

        public DbParameter GetDateTimeParameter(string parameter, DateTime value)
        {
            MySqlParameter mySqlParameter = new MySqlParameter(parameter, MySqlDbType.DateTime);
            mySqlParameter.Value = value;

            return mySqlParameter;
        }
    }
}
