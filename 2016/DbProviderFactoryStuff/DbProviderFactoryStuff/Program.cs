using System;
using System.Data;
using System.Data.Common;

namespace DbProviderFactoryStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("All registered DbProviderFactories:");
                var allFactoryClasses = DbProviderFactories.GetFactoryClasses();

                foreach (DataRow row in allFactoryClasses.Rows)
                {
                    Console.WriteLine(row[0] + ": " + row[2]);
                }

                Console.WriteLine();
                Console.WriteLine("Try to access a MySql DB:");

                DbProviderFactory dbf = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                using (DbConnection dbcn = dbf.CreateConnection())
                {
                    dbcn.ConnectionString = "Server=localhost;Database=testdb;Uid=root;Pwd=Pass1word;";
                    dbcn.Open();
                    using (DbCommand dbcmd = dbcn.CreateCommand())
                    {
                        dbcmd.CommandType = CommandType.Text;
                        dbcmd.CommandText = "SHOW TABLES;";

                        // parameter...
                        //var foo = dbcmd.CreateParameter();
                        //foo.ParameterName = "...";
                        //foo.Value = "...";

                        using (DbDataReader dbrdr = dbcmd.ExecuteReader())
                        {
                            while (dbrdr.Read())
                            {
                                Console.WriteLine(dbrdr[0]);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }


            Console.ReadLine();

        }
    }
}
