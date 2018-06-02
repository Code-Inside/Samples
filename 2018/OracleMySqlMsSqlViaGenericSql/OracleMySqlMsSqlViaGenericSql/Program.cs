using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OracleMySqlMsSqlViaGenericSql
{
    class Program
    {
        static void Main(string[] args)
        {
            var factoryClasses = DbProviderFactories.GetFactoryClasses();

            StringBuilder report = new StringBuilder();
            foreach (DataRow factoryClass in factoryClasses.Rows)
            {
                report.AppendLine(factoryClass.ItemArray[2].ToString());
            }

            Console.WriteLine("DbProviderFactories: ");
            Console.WriteLine(report.ToString());

            MySQLTest();
            MsSQLTest();
            OracleTest();


        }

        private static void MySQLTest()
        {
            string constr = "Server=localhost;Database=testdb;Uid=...;Pwd=...;";

            DbProviderFactory factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");

            using (DbConnection conn = factory.CreateConnection())
            {
                try
                {
                    conn.ConnectionString = constr;
                    conn.Open();

                    using (DbCommand dbcmd = conn.CreateCommand())
                    {
                        dbcmd.CommandType = CommandType.Text;
                        dbcmd.CommandText = "select name, address FROM contacts WHERE name LIKE Concat('%', @name, '%')  ";

                        var dbParam = dbcmd.CreateParameter();

                        // can be also be prefixed with @
                        dbParam.ParameterName = "name";
                        dbParam.Value = "foo";

                        dbcmd.Parameters.Add(dbParam);

                        using (DbDataReader dbrdr = dbcmd.ExecuteReader())
                        {
                            while (dbrdr.Read())
                            {
                                Console.WriteLine(dbrdr[0]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private static void MsSQLTest()
        {
            string constr = "Data Source=localhost;Initial Catalog=...;User ID=...;Password=...;MultipleActiveResultSets=True;";

            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");

            using (DbConnection conn = factory.CreateConnection())
            {
                try
                {
                    conn.ConnectionString = constr;
                    conn.Open();

                    using (DbCommand dbcmd = conn.CreateCommand())
                    {
                        dbcmd.CommandType = CommandType.Text;
                        dbcmd.CommandText = "select name, address FROM contacts WHERE name LIKE '%' + @name + '%' ";

                        var dbParam = dbcmd.CreateParameter();

                        // can be prefixed with @
                        dbParam.ParameterName = "name";
                        dbParam.Value = "foobar";

                        dbcmd.Parameters.Add(dbParam);

                        using (DbDataReader dbrdr = dbcmd.ExecuteReader())
                        {
                            while (dbrdr.Read())
                            {
                                Console.WriteLine(dbrdr[0]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private static void OracleTest()
        {
            string constr = "Data Source=localhost;User Id=...;Password=...;";

            DbProviderFactory factory = DbProviderFactories.GetFactory("Oracle.ManagedDataAccess.Client");

            using (DbConnection conn = factory.CreateConnection())
            {
                try
                {
                    conn.ConnectionString = constr;
                    conn.Open();

                    using (DbCommand dbcmd = conn.CreateCommand())
                    {
                        dbcmd.CommandType = CommandType.Text;
                        dbcmd.CommandText = "select name, address from contacts WHERE UPPER(name) Like UPPER('%' || :name || '%') ";

                        var dbParam = dbcmd.CreateParameter();
                        // prefix with : possible, but @ will be result in an error
                        dbParam.ParameterName = "name";
                        dbParam.Value = "foobar";

                        dbcmd.Parameters.Add(dbParam);

                        using (DbDataReader dbrdr = dbcmd.ExecuteReader())
                        {
                            while (dbrdr.Read())
                            {
                                Console.WriteLine(dbrdr[0]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
    }
}
