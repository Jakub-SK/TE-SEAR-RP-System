using Npgsql;
using System.Data;

namespace SEAR_DataContract.Misc
{
    public static class DBHelper
    {
        //Example to insert parameter
        //public void SetParameter()
        //{
        //    List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
        //    parameters.Add(new NpgsqlParameter("p", "some"));
        //}
        private static string GetConnectionString()
        {
            return "Host=localhost:15432;Username=sear_user;Password=sear_rp_truth_enforcers_v18;Database=SEAR_Database";
        }
        public static NpgsqlConnection GetConnection()
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                var conn = new NpgsqlConnection(GetConnectionString());
                Logger.LogInformation("Database connected with Development Enviroment");
                return conn;
            }
            else
            {
                var conn = new NpgsqlConnection(GetConnectionString());
                Logger.LogInformation("Database connected with Production Enviroment");
                return conn;
            }
        }
        public static int ExecuteDatabaseNonQuery(string sql)
        {
            using var conn = GetConnection();
            int affectedRows;
            try
            {
                conn.Open();
            }
            catch
            {
                throw UnableToConnectDatabaseException(conn, sql);
            }
            try
            {
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    affectedRows = cmd.ExecuteNonQuery();
                }
                conn.Close();
                return affectedRows;
            }
            catch
            {
                throw InternalDatabaseException(sql);
            }
        }
        public static int ExecuteDatabaseNonQuery(string sql, List<NpgsqlParameter> parameterList)
        {
            using var conn = GetConnection();
            int affectedRows;
            try
            {
                conn.Open();
            }
            catch
            {
                throw UnableToConnectDatabaseException(conn, sql);
            }
            try
            {
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    foreach (NpgsqlParameter parameter in parameterList)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    affectedRows = cmd.ExecuteNonQuery();
                }
                conn.Close();
                return affectedRows;
            }
            catch
            {
                throw InternalDatabaseException(sql);
            }
        }
        public static DataSet ExecuteDatabaseQuery(string sql)
        {
            using var dataSet = new DataSet();
            using var conn = GetConnection();
            try
            {
                conn.Open();
            }
            catch
            {
                throw UnableToConnectDatabaseException(conn, sql);
            }
            try
            {
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                    using var adapter = new NpgsqlDataAdapter(cmd);
                    adapter.Fill(dataSet);
                }
                conn.Close();
                return dataSet;
            }
            catch
            {
                throw InternalDatabaseException(sql);
            }
        }
        public static DataSet ExecuteDatabaseQuery(string sql, List<NpgsqlParameter> parameterList)
        {
            using var dataSet = new DataSet();
            using var conn = GetConnection();
            try
            {
                conn.Open();
            }
            catch
            {
                throw UnableToConnectDatabaseException(conn, sql);
            }
            try
            {
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    foreach (NpgsqlParameter parameter in parameterList)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.ExecuteNonQuery();
                    using var adapter = new NpgsqlDataAdapter(cmd);
                    adapter.Fill(dataSet);
                }
                conn.Close();
                return dataSet;
            }
            catch
            {
                throw InternalDatabaseException(sql);
            }
        }
        private static NpgsqlException UnableToConnectDatabaseException(NpgsqlConnection conn, string sql)
        {
            return new NpgsqlException(
                $"Unable to establish connection to database\n" +
                $"Connection: {conn.ConnectionString}\n" +
                $"SQL: {sql}\n" +
                $"FUCK U >:( Please check is the cloudflared is running when in development enviroment u \"fuckin stoopid\""
            );
        }
        private static NpgsqlException InternalDatabaseException(string sql)
        {
            return new NpgsqlException(
                $"Internal Database Exception\n" +
                $"Check SQL statements\n" +
                $"SQL: {sql}\n" +
                $"DIU!!! Check SQL statements la >:("
            );
        }
    }
}