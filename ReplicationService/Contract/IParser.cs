using System;
using System.Collections.Generic;

namespace ReplicationService.Contract
{
    public interface IParser
    {
        List<Tuple<DateTime, string>> GetChangesSince(DateTime dt);
    }
}
