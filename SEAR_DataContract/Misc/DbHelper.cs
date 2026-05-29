using Npgsql;
using System.Data;
using SEAR_DataContract.Models;

namespace SEAR_DataContract.Misc
{
    internal static class ConnectionString
    {
        internal static string GetDevelopmentString => "Host=localhost:15432;Username=sear_user;Password=sear_rp_truth_enforcers_v18;Database=SEAR_Database";
        internal static string GetProductionString => "Host=localhost:5432;Username=sear_user;Password=sear_rp_truth_enforcers_v18;Database=SEAR_Database";
    }
    public class DbExecuteItems
    {
        public string Sql { get; set; } = string.Empty;
        public IEnumerable<NpgsqlParameter>? Parameters { get; set; }
    }
    public static class DbHelper
    {
        private static NpgsqlConnection GetConnection()
        {
            if (Misc.CheckIsDevelopmentEnvironment())
            {
                var conn = new NpgsqlConnection(ConnectionString.GetDevelopmentString);
                AppLogger.LogInformation("Database connected with Development Environment");
                return conn;
            }
            else
            {
                var conn = new NpgsqlConnection(ConnectionString.GetProductionString);
                AppLogger.LogInformation("Database connected with Production Environment");
                return conn;
            }
        }
        public static async Task<DataTable> ExecuteQueryAsync(Func<DbExecuteItems, DbExecuteItems> func, bool throwExceptionWhenFallBack = false)
        {
            DbExecuteItems executeItems = func.Invoke(new DbExecuteItems());
            using DataTable dataTable = new DataTable();
            await using var conn = GetConnection();
            try
            {
                await conn.OpenAsync();
                await using (var cmd = new NpgsqlCommand(executeItems.Sql, conn))
                {
                    if (executeItems.Parameters != null)
                    {
                        foreach (NpgsqlParameter parameter in executeItems.Parameters)
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
                if (Misc.CheckIsDevelopmentEnvironment())
                {
                    throw CreateDatabaseException(conn, executeItems.Sql, ex);
                }
                else if (throwExceptionWhenFallBack)
                {
                    throw CreateDatabaseException(conn, executeItems.Sql, ex);
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
        public static async Task<int> ExecuteNonQueryAsync(Func<DbExecuteItems, DbExecuteItems> func, bool throwExceptionWhenFallBack = false)
        {
            DbExecuteItems executeItems = func.Invoke(new DbExecuteItems());
            int affectedRows = -1;
            await using var conn = GetConnection();
            try
            {
                await conn.OpenAsync();
                await using (var cmd = new NpgsqlCommand(executeItems.Sql, conn))
                {
                    if (executeItems.Parameters != null)
                    {
                        foreach (NpgsqlParameter parameter in executeItems.Parameters)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    affectedRows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                if (Misc.CheckIsDevelopmentEnvironment())
                {
                    throw CreateDatabaseException(conn, executeItems.Sql, ex);
                }
                else if (throwExceptionWhenFallBack)
                {
                    throw CreateDatabaseException(conn, executeItems.Sql, ex);
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
        public static async void ExecuteNonQueryAsyncNoReturn(Func<DbExecuteItems, DbExecuteItems> func, bool throwExceptionWhenFallBack = false)
        {
            DbExecuteItems executeItems = func.Invoke(new DbExecuteItems());
            await using var conn = GetConnection();
            try
            {
                await conn.OpenAsync();
                await using (var cmd = new NpgsqlCommand(executeItems.Sql, conn))
                {
                    if (executeItems.Parameters != null)
                    {
                        foreach (NpgsqlParameter parameter in executeItems.Parameters)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                if (Misc.CheckIsDevelopmentEnvironment())
                {
                    throw CreateDatabaseException(conn, executeItems.Sql, ex);
                }
                else if (throwExceptionWhenFallBack)
                {
                    throw CreateDatabaseException(conn, executeItems.Sql, ex);
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
        private static NpgsqlException CreateDatabaseException(NpgsqlConnection conn, string sql, Exception ex)
        {
            if (ex.Message.Contains("Failed to connect to"))
            {
                //Cannot connect to database
                throw new NpgsqlException(
                    $"Unable to establish connection to database\n" +
                    $"Connection: {conn.ConnectionString}\n" +
                    $"SQL: {sql}\n" +
                    $"FUCK U >:( Please check is the cloudflared is running when in development environment u \"fuckin stoopid\"\n" +
                    $"Exception Message: {ex.Message}"
                );
            }
            else
            {
                //Internal Database Exception
                throw new NpgsqlException(
                    $"Internal Database Exception\n" +
                    $"Check SQL statements\n" +
                    $"SQL: {sql}\n" +
                    $"DIU!!! Check SQL statements la >:(\n" +
                    $"Exception Message: {ex.Message}"
                );
            }
        }
    }
}