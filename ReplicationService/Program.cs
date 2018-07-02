using ReplicationService.Contract;
using ReplicationService.DatabaseAdapters;
using System;
using System.Configuration;

namespace ReplicationService
{
    class Program
    {
        /// <summary>
        /// Improvements:
        /// 1. Source and Destination database types can be provided as input
        /// and then, appropriate strategy can be chosen through reflection.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            IStrategy sqlStrategy = new SqlStrategy(ConfigurationManager.AppSettings["SqlConnectionString"]);
            IStrategy mysqlStrategy = new MySqlStrategy(ConfigurationManager.AppSettings["MySqlConnectionString"]);
            IParser source = new DatabaseAdapter(mysqlStrategy);
            ISqlAdapter destination = new DatabaseAdapter(sqlStrategy);

            Poller poller = new Poller(source, destination);
            poller.Start();

            Console.WriteLine("Enter key...");
            Console.ReadKey();
        }
    }
}
