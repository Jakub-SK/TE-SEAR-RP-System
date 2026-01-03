using Npgsql;
using System.Data;
using SEAR_DataContract;
using SEAR_DataContract.Misc;

namespace SEAR_API.Models
{
    public static class HomeModel
    {
        public static List<DatabaseUsers> GetDatabaseUsersList()
        {
            List<DatabaseUsers> list = new List<DatabaseUsers>();

            string sql = "SELECT * FROM projecttest";
            using DataSet dataSet = DBHelper.ExecuteDatabaseQuery(sql);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DatabaseUsers user = new DatabaseUsers();
                user.ID = row["user_id"].ToString();
                user.Name = row["user_name"].ToString();
                user.Type = row["user_type"].ToString();
                user.Rank = row["user_rank"].ToString();
                list.Add(user);
            }
            return list;
        }
    }
}