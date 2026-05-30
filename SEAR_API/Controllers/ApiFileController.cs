using Microsoft.AspNetCore.Mvc;
using SEAR_DataContract.Misc;
using SEAR_DataContract.Models;
using System.Data;
using Npgsql;

namespace SEAR_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiFileController : Controller
    {
        [HttpPost("SaveFileToDatabase")]
        public async Task<ReturnSaveFileToDatabase> SaveFileToDatabase([FromBody] SaveFileToDatabaseParameters model)
        {
            int affectedRows = await DbHelper.ExecuteNonQueryAsync(executeItems =>
            {
                executeItems.Sql = @"
                    INSERT INTO files
                    (file_name, content_type, data)
                    VALUES
                    (@name, @type, @data);";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@name", model.FileName),
                    new NpgsqlParameter("@type", model.ContentType),
                    new NpgsqlParameter("@data", NpgsqlTypes.NpgsqlDbType.Bytea)
                    {
                        Value = model.FileBytes
                    }
                };

                return executeItems;
            });

            return new ReturnSaveFileToDatabase
            {
                IsSuccessful = affectedRows > 0
            };
        }
        [HttpPost("DownloadFile")]
        public async Task<ReturnDownloadFile> DownloadFile([FromBody] DownloadFileParameters model)
        {
            DataTable dt = await DbHelper.ExecuteQueryAsync(dBExecuteItems =>
            {
                dBExecuteItems.Sql = @"
                    SELECT *
                    FROM files
                    WHERE id = @id";
                
                dBExecuteItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@id", model.FileId)
                };
                return dBExecuteItems;
            });

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