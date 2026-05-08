using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SEAR_DataContract.Misc;
using SEAR_DataContract.Models;
using System.Data;

namespace SEAR_API.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class ApiFileController : Controller
    {
        [HttpPost("SaveFileToDatabase")]
        public async Task<ReturnSaveFileToDatabase> SaveFileToDatabase([FromBody] SaveFileToDatabaseParameters model)
        {
            string sql = @"
                INSERT INTO files
                (file_name, content_type, data)
                VALUES
                (@name, @type, @data);";

            List<NpgsqlParameter> parametersList = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@name", model.FileName),
                new NpgsqlParameter("@type", model.ContentType),
                new NpgsqlParameter("@data", NpgsqlTypes.NpgsqlDbType.Bytea)
                {
                    Value = model.FileBytes
                }
            };

            int affectedRows = await DBHelper.ExecuteDatabaseNonQueryAsync(sql, parametersList);

            return new ReturnSaveFileToDatabase
            {
                IsSuccessful = affectedRows > 0
            };
        }
        [HttpPost("DownloadFile")]
        public async Task<ReturnDownloadFile> DownloadFile([FromBody] DownloadFileParameters model)
        {
            string sql = @"
                SELECT *
                FROM files
                WHERE id = @id";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@id", model.FileId)
            };

            DataTable dt = await DBHelper.ExecuteDatabaseQueryAsync(sql, parameters);

            if (dt.Rows.Count == 0)
                return null!;

            DataRow row = dt.Rows[0];

            return new ReturnDownloadFile
            {
                Id = Convert.ToInt32(row["id"]),
                FileName = row["file_name"].ToString() ?? "",
                ContentType = row["content_type"].ToString() ?? "application/octet-stream",
                FileBytes = (byte[])row["data"]
            };
        }
    }
}