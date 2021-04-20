using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AesCloudData.Utils
{
    public static class ForPostgress
    {
        public static string ParseDatabaseUrl(string databaseUrl)
        {
            try
            {
                //DATABASE_URL=postgres://{user}:{password}@{hostname}:{port}/{database-name}
                //   const string PostgresConnectionString = "host=localhost;port=5432;database=clouddata;userid=postgres;password=1q1q";

                     var arr = databaseUrl.Split("/@:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var userid = arr[1];
                var password = arr[2];
                var host = arr[3];
                var port = arr[4];
                var database = arr[5];
                string connect = $"host={host}; port={port}; database={database}; userid={userid}; password={password}; sslmode=require ;";
                return connect;
            }
            catch (Exception ex)
            {
                var old = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.WriteLine("ParseDatabaseUrl:" + ex.Message);
                Console.BackgroundColor = old;
   

                return null;
            }
        }
    }
}
