using System;

namespace ReplicationService.Contract
{
    public interface ISqlAdapter
    {
        bool ExecuteStatement(string sqlCommand);

        bool UpdateLastSyncTime(DateTime time);

        DateTime GetLastSyncTime();
    }
}
