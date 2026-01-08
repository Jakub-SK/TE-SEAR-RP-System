using Npgsql;
using System.Data;

namespace SEAR_DataContract.Misc
{
    internal class DatabaseResult
    {
        public DataSet? DataSet { get; set; }
        public int AffectedRows { get; set; }
    }
    internal static class ConnectionString
    {
        public static string GetDevelopmentString
        {
            get
            {
                return "Host=localhost:15432;Username=sear_user;Password=sear_rp_truth_enforcers_v18;Database=SEAR_Database";
            }
        }
        public static string GetProductionString
        {
            get
            {
                return "Host=localhost:5432;Username=sear_user;Password=sear_rp_truth_enforcers_v18;Database=SEAR_Database";
            }
        }
    }
    public static class DBHelper
    {
        //Example to insert parameter
        //
        //List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
        //parameters.Add(new NpgsqlParameter("p", "some"));
        public static NpgsqlConnection GetConnection()
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                var conn = new NpgsqlConnection(ConnectionString.GetDevelopmentString);
                if (Misc.CheckIsDevelopmentEnviroment())
                {
                    Logger.LogInformation("Database connected with Development Enviroment");
                }
                return conn;
            }
            else
            {
                var conn = new NpgsqlConnection(ConnectionString.GetProductionString);
                if (Misc.CheckIsDevelopmentEnviroment())
                {
                    Logger.LogInformation("Database connected with Production Enviroment");
                }
                return conn;
            }
        }
        public static DataSet ExecuteDatabaseQuery(string sql, List<NpgsqlParameter>? parameterList = null)
        {
            return Execute(sql, parameterList).DataSet!;
        }
        public static int ExecuteDatabaseNonQuery(string sql, List<NpgsqlParameter>? parameterList = null)
        {
            return Execute(sql, parameterList).AffectedRows;
        }
        private static DatabaseResult Execute(string sql, List<NpgsqlParameter>? parameterList = null)
        {
            DatabaseResult databaseResult = new DatabaseResult();
            using DataSet dataSet = new DataSet();
            using var conn = GetConnection();
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    if (parameterList != null)
                    {
                        foreach (NpgsqlParameter parameter in parameterList)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    using var adapter = new NpgsqlDataAdapter(cmd);
                    databaseResult.AffectedRows = cmd.ExecuteNonQuery();
                    if (sql.Contains("SELECT"))
                    {
                        adapter.Fill(dataSet);
                        databaseResult.DataSet = dataSet;
                    }
                }
            }
            catch (Exception ex)
            {
                if (Misc.CheckIsDevelopmentEnviroment())
                {
                    if (ex.Message.Contains("Failed to connect to"))
                    {
                        throw UnableToConnectDatabaseException(conn, sql, ex);
                    }
                    else
                    {
                        throw InternalDatabaseException(sql, ex);
                    }
                }
                else
                {
                    Misc.LogException(ex);
                }
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }
            return databaseResult;
        }
        internal static string ExecuteLogException(Exception ex, string? uuid = null)
        {
            uuid = uuid == null ? Guid.CreateVersion7().ToString() : uuid;
            
            string sql = "INSERT INTO log_exception (track_uuid, exception_message) VALUES (@UUID, @ExceptionMessage);";

            List<NpgsqlParameter> parameterList = new List<NpgsqlParameter>();
            parameterList.Add(new NpgsqlParameter("UUID", uuid));
            parameterList.Add(new NpgsqlParameter("ExceptionMessage", ex.Message));

            using var conn = GetConnection();
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    foreach (NpgsqlParameter parameter in parameterList)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    using var adapter = new NpgsqlDataAdapter(cmd);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                if (Misc.CheckIsDevelopmentEnviroment())
                {
                    if (ex.Message.Contains("Failed to connect to"))
                    {
                        throw UnableToConnectDatabaseException(conn, sql, ex);
                    }
                    else
                    {
                        throw InternalDatabaseException(sql, ex);
                    }
                }
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }
            return uuid;
        }
        internal static int ExecuteUpdateLogExceptionWithSteps(string uuid, string steps)
        {
            int affectedRows = -1;
            string sql = @"
                UPDATE log_exception
                SET steps = @Steps
                WHERE track_uuid = @UUID
            ;";

            List<NpgsqlParameter> parameterList = new List<NpgsqlParameter>();
            parameterList.Add(new NpgsqlParameter("Steps", steps));
            parameterList.Add(new NpgsqlParameter("UUID", uuid));

            using var conn = GetConnection();
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    foreach (NpgsqlParameter parameter in parameterList)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    using var adapter = new NpgsqlDataAdapter(cmd);
                    affectedRows = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                if (Misc.CheckIsDevelopmentEnviroment())
                {
                    if (ex.Message.Contains("Failed to connect to"))
                    {
                        throw UnableToConnectDatabaseException(conn, sql, ex);
                    }
                    else
                    {
                        throw InternalDatabaseException(sql, ex);
                    }
                }
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }
            return affectedRows;
        }
        private static NpgsqlException UnableToConnectDatabaseException(NpgsqlConnection conn, string sql, Exception ex)
        {
            return new NpgsqlException(
                $"Unable to establish connection to database\n" +
                $"Connection: {conn.ConnectionString}\n" +
                $"SQL: {sql}\n" +
                $"FUCK U >:( Please check is the cloudflared is running when in development enviroment u \"fuckin stoopid\"\n" +
                $"Exception Message: {ex.Message}"
            );
        }
        private static NpgsqlException InternalDatabaseException(string sql, Exception ex)
        {
            return new NpgsqlException(
                $"Internal Database Exception\n" +
                $"Check SQL statements\n" +
                $"SQL: {sql}\n" +
                $"DIU!!! Check SQL statements la >:(\n" +
                $"Exception Message: {ex.Message}"
            );
        }
    }
}