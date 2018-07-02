using ReplicationService.Contract;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ReplicationService
{
    public class Poller
    {
        private IParser source;
        private ISqlAdapter destination;

        public Poller(IParser source, ISqlAdapter destination)
        {
            this.source = source;
            this.destination = destination;
        }

        /// <summary>
        /// Improvements:
        /// 1. Command executing and LastSyncTime update should be in a transaction
        /// 2. LastSyncTime should be updated periodically as well
        /// 3. Improve on the precision of DateTime
        /// </summary>
        public void Start()
        {
            while (true)
            {
                try
                {
                    // Get last timestamp from a file
                    DateTime dt = destination.GetLastSyncTime();
                    Console.WriteLine($"LastSyncTime: {dt}");

                    // Get commands after that
                    List<Tuple<DateTime, string>> commands = source.GetChangesSince(dt);
                    foreach (Tuple<DateTime, string> pair in commands)
                    {
                        DateTime time = pair.Item1;
                        string command = pair.Item2;

                        Console.WriteLine($"{time}, {command}");

                        // Execute them on the destination
                        bool isSuccess = destination.ExecuteStatement(command);
                        if (!isSuccess)
                        {
                            throw new Exception($"Failed to replicate command {command}");
                        }

                        // Update the timestamp after success
                        bool isUpdated = destination.UpdateLastSyncTime(time.AddSeconds(1));
                        Console.WriteLine($"IsUpdated {isUpdated} | Time: {time}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e}");
                }

                Console.WriteLine("Sleeping...");
                Thread.Sleep(5 * 1000);
            }
        }
    }
}
