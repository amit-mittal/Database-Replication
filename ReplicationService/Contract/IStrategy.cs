using System;
using System.Data.Common;

namespace ReplicationService.Contract
{
    public interface IStrategy
    {
        DbConnection GetConnection();

        DbCommand GetCommand(string command, DbConnection connection);

        DbParameter GetDateTimeParameter(string paramterName, DateTime value);
    }
}
