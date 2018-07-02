using ReplicationService.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReplicationService.DatabaseAdapters
{
    public class DatabaseAdapter : ISqlAdapter, IParser
    {
        private IStrategy strategy;

        public DatabaseAdapter(IStrategy strategy)
        {
            this.strategy = strategy;
        }

        public bool ExecuteStatement(string sqlCommand)
        {
            using (var connection = strategy.GetConnection())
            {
                connection.Open();

                using (var command = strategy.GetCommand(sqlCommand, connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e);
                        return false;
                    }
                }
            }
        }

        public DateTime GetLastSyncTime()
        {
            using (var connection = strategy.GetConnection())
            {
                connection.Open();

                using (var command = strategy.GetCommand("SELECT * FROM Metadata", connection))
                {
                    try
                    {
                        using (var dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                DateTime time = DateTime.Parse(dataReader["LastSyncTime"].ToString());
                                return time;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e);
                    }

                    return DateTime.MinValue;
                }
            }
        }

        public bool UpdateLastSyncTime(DateTime time)
        {
            using (var connection = strategy.GetConnection())
            {
                connection.Open();

                using (var command = strategy.GetCommand("UPDATE Metadata SET LastSyncTime = @TIME", connection))
                {
                    command.Parameters.Add(strategy.GetDateTimeParameter("@TIME", time));

                    try
                    {
                        if (command.ExecuteNonQuery() != 1)
                        {
                            return false;
                        }
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e);
                        return false;
                    }
                }
            }
        }

        public List<Tuple<DateTime, string>> GetChangesSince(DateTime dt)
        {
            var commands = new List<Tuple<DateTime, string>>();

            using (var connection = strategy.GetConnection())
            {
                connection.Open();

                using (var command = strategy.GetCommand("SELECT * FROM  mysql.general_log WHERE event_time > @TIME", connection))
                {
                    command.Parameters.Add(strategy.GetDateTimeParameter("@TIME", dt));

                    try
                    {
                        using (var dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                DateTime time = DateTime.Parse(dataReader["event_time"].ToString());
                                string c = Encoding.UTF8.GetString((byte[])dataReader["argument"]);

                                if (IsTableOfInterest(c) &&
                                    (c.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase) ||
                                    c.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) ||
                                    c.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase)))
                                {
                                    commands.Add(new Tuple<DateTime, string>(time, c));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e);
                    }

                    return commands;
                }
            }
        }

        private bool IsTableOfInterest(string command)
        {
            if (command.Contains("Patients") || command.Contains("Appointments"))
            {
                return true;
            }

            return false;
        }
    }
}
