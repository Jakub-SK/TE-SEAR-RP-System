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
        public static NpgsqlConnection OpenConnection()
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                var conn = new NpgsqlConnection("Host=localhost:15432;Username=sear_user;Password=sear_rp_truth_enforcers_v18;Database=SEAR_Database");
                Logger.LogInformation("Database connected with Development Enviroment");
                return conn;
            }
            else
            {
                var conn = new NpgsqlConnection("Host=localhost:5432;Username=sear_user;Password=sear_rp_truth_enforcers_v18;Database=SEAR_Database");
                Logger.LogInformation("Database connected with Production Enviroment");
                return conn;
            }
        }
        public static void ExecuteDatabaseNonQuery(string sql)
        {
            using var conn = OpenConnection();

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
            conn.Close();
            return;
        }
        public static void ExecuteDatabaseNonQuery(string sql, List<NpgsqlParameter> parameterList)
        {
            using var conn = OpenConnection();

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                foreach (NpgsqlParameter parameter in parameterList)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.ExecuteNonQuery();
            }
            conn.Close();
            return;
        }
        public static DataSet ExecuteDatabaseQuery(string sql)
        {
            using var dataSet = new DataSet();
            using var conn = OpenConnection();

            conn.Open();
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
                using var adapter = new NpgsqlDataAdapter(cmd);
                adapter.Fill(dataSet);
            }
            conn.Close();
            return dataSet;
        }
        public static DataSet ExecuteDatabaseQuery(string sql, List<NpgsqlParameter> parameterList)
        {
            using var dataSet = new DataSet();
            using var conn = OpenConnection();

            conn.Open();
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
    }
}