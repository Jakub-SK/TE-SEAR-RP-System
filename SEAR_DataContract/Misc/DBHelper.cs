using Npgsql;
using System.Data;

namespace SEAR_DataContract.Misc
{
    internal class DatabaseResult
    {
        public DatabaseResult()
        {
            DataSet = new DataSet();
        }
        public DataSet DataSet { get; set; }
        public int AffectedRows { get; set; }
    }
    internal static class ConnectionString
    {
        public static string GetDevelopmentString => "Host=localhost:15432;Username=sear_user;Password=sear_rp_truth_enforcers_v18;Database=SEAR_Database";
        public static string GetProductionString => "Host=localhost:5432;Username=sear_user;Password=sear_rp_truth_enforcers_v18;Database=SEAR_Database";
    }
    public class ShowExceptionMessage
    {
        public ShowExceptionMessage()
        {
            UUID = "Unable to get UUID";
            ErrorType = "Unknown";
        }
        public string UUID { get; set; }
        public string ErrorType { get; set; }
    }
    public static class DBHelper
    {
        //Example to insert parameter
        //
        //List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
        //parameters.Add(new NpgsqlParameter("p", "some"));
        public static NpgsqlConnection GetConnection()
        {
            if (Misc.CheckIsDevelopmentEnviroment())
            {
                var conn = new NpgsqlConnection(ConnectionString.GetDevelopmentString);
                AppLogger.LogInformation("Database connected with Development Enviroment");
                return conn;
            }
            else
            {
                var conn = new NpgsqlConnection(ConnectionString.GetProductionString);
                AppLogger.LogInformation("Database connected with Production Enviroment");
                return conn;
            }
        }
        //public static DataSet ExecuteDatabaseQuery(string sql, List<NpgsqlParameter>? parameterList = null)
        //{
        //    return Execute(sql, parameterList).DataSet;
        //}
        //public static int ExecuteDatabaseNonQuery(string sql, List<NpgsqlParameter>? parameterList = null)
        //{
        //    return Execute(sql, parameterList).AffectedRows;
        //}
        //private static DatabaseResult Execute(string sql, List<NpgsqlParameter>? parameterList = null)
        //{
        //    DatabaseResult databaseResult = new DatabaseResult();
        //    using var conn = GetConnection();
        //    try
        //    {
        //        conn.Open();
        //        using (var cmd = new NpgsqlCommand(sql, conn))
        //        {
        //            if (parameterList != null)
        //            {
        //                foreach (NpgsqlParameter parameter in parameterList)
        //                {
        //                    cmd.Parameters.Add(parameter);
        //                }
        //            }
        //            using var adapter = new NpgsqlDataAdapter(cmd);
        //            databaseResult.AffectedRows = cmd.ExecuteNonQuery();
        //            if (sql.Contains("SELECT"))
        //            {
        //                adapter.Fill(databaseResult.DataSet);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (Misc.CheckIsDevelopmentEnviroment())
        //        {
        //            if (ex.Message.Contains("Failed to connect to"))
        //            {
        //                throw UnableToConnectDatabaseException(conn, sql, ex);
        //            }
        //            else
        //            {
        //                throw InternalDatabaseException(sql, ex);
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        if (conn.State != ConnectionState.Closed)
        //        {
        //            conn.Close();
        //        }
        //    }
        //    return databaseResult;
        //}
        public static async Task<DataTable> ExecuteDatabaseQueryAsync(string sql, List<NpgsqlParameter>? parameterList = null)
        {
            return await ExecuteQueryAsync(sql, parameterList);
        }
        public static async void ExecuteDatabaseQueryAsyncNoReturn(string sql, List<NpgsqlParameter>? parameterList = null)
        {
            ExecuteQueryAsyncNoReturn(sql, parameterList);
        }
        public static async Task<int> ExecuteDatabaseNonQueryAsync(string sql, List<NpgsqlParameter>? parameterList = null)
        {
            return await ExecuteNonQueryAsync(sql, parameterList);
        }
        public static async void ExecuteDatabaseNonQueryAsyncNoReturn(string sql, List<NpgsqlParameter>? parameterList = null)
        {
            ExecuteNonQueryAsyncNoReturn(sql, parameterList);
        }
        private static async Task<DataTable> ExecuteQueryAsync(string sql, List<NpgsqlParameter>? parameterList = null)
        {
            using DataTable dataTable = new DataTable();
            await using var conn = GetConnection();
            try
            {
                await conn.OpenAsync();
                await using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    if (parameterList != null)
                    {
                        foreach (NpgsqlParameter parameter in parameterList)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    await using var reader = await cmd.ExecuteReaderAsync();
                    dataTable.Load(reader);
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
            return dataTable;
        }
        private static async void ExecuteQueryAsyncNoReturn(string sql, List<NpgsqlParameter>? parameterList = null)
        {
            await using var conn = GetConnection();
            try
            {
                await conn.OpenAsync();
                await using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    if (parameterList != null)
                    {
                        foreach (NpgsqlParameter parameter in parameterList)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    await cmd.ExecuteReaderAsync();
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
        }
        private static async Task<int> ExecuteNonQueryAsync(string sql, List<NpgsqlParameter>? parameterList = null)
        {
            int affectedRows = -1;
            await using var conn = GetConnection();
            try
            {
                await conn.OpenAsync();
                await using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    if (parameterList != null)
                    {
                        foreach (NpgsqlParameter parameter in parameterList)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    affectedRows = await cmd.ExecuteNonQueryAsync();
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
        private static async void ExecuteNonQueryAsyncNoReturn(string sql, List<NpgsqlParameter>? parameterList = null)
        {
            await using var conn = GetConnection();
            try
            {
                await conn.OpenAsync();
                await using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    if (parameterList != null)
                    {
                        foreach (NpgsqlParameter parameter in parameterList)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    await cmd.ExecuteNonQueryAsync();
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
        }
        internal static async Task<ShowExceptionMessage> LogException(Exception ex, string errorType, string appType, string? uuid = null)
        {
            uuid = uuid == null ? Guid.CreateVersion7().ToString() : uuid;
            ShowExceptionMessage display = new ShowExceptionMessage
            {
                UUID = uuid,
                ErrorType = errorType
            };
            
            string sql = "INSERT INTO log_exception (track_uuid, exception_message, error_type, app_type) VALUES (@uuid, @exceptionMessage, @errorType, @appType);";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("uuid", uuid),
                new NpgsqlParameter("exceptionMessage", ex.Message),
                new NpgsqlParameter("errorType", errorType),

            };
           
            if (!String.IsNullOrEmpty(appType))
            {
                parameters.Add(new NpgsqlParameter("appType", appType));
            }

            try
            {
                ExecuteDatabaseNonQueryAsyncNoReturn(sql, parameters);
            }
            catch
            {
                if (Misc.CheckIsDevelopmentEnviroment())
                {
                    //throw UnableToConnectDatabaseException(conn, sql, ex);
                    AppLogger.LogError("Unable to log exception to database,\nFUCK U >:( Please check is the cloudflared is running when in development enviroment u \"fuckin stoopid\"");
                }
            }
            return display;
        }
        internal static async void UpdateLogExceptionWithSteps(string uuid, string steps)
        {
            string sql = @"
                UPDATE log_exception
                SET steps = @Steps
                WHERE track_uuid = @UUID;";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("Steps", steps),
                new NpgsqlParameter("UUID", uuid)
            };
            
            try
            {
                ExecuteDatabaseNonQueryAsyncNoReturn(sql, parameters);
            }
            catch
            {
                if (Misc.CheckIsDevelopmentEnviroment())
                {
                    //throw UnableToConnectDatabaseException(conn, sql, ex);
                    AppLogger.LogError("Unable to update steps to database,\nFUCK U >:( Please check is the cloudflared is running when in development enviroment u \"fuckin stoopid\"");
                }
            }
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