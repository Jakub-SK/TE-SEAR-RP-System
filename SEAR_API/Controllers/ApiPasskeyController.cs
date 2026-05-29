using Microsoft.AspNetCore.Mvc;
using SEAR_DataContract.Misc;
using SEAR_DataContract.Models;
using System.Data;
using Npgsql;

namespace SEAR_API.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class ApiPasskeyController : Controller
    {
        [HttpPost("CreateUserAccount")]
        public async Task<Guid> CreateUserAccount([FromBody] CreateUserAccountParameters model)
        {
            Guid userID = Guid.NewGuid();
            DbHelper.ExecuteNonQueryAsyncNoReturn(executeItems =>
            {
                executeItems.Sql = @"
                    INSERT INTO users
                    (user_id, username, display_name)
                    VALUES
                    (@userID, @username, @displayName);";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@userID", userID),
                    new NpgsqlParameter("@username", model.Username),
                    new NpgsqlParameter("@displayName", model.DisplayName)
                };

                return executeItems;
            });

            return userID;
        }
        [HttpPost("RemoveUserAccountByUsername")]
        public async Task<IActionResult> RemoveUserAccountByUsername([FromBody] RemoveUserAccountByUsernameParameters model)
        {
            int affectedRows = await DbHelper.ExecuteNonQueryAsync(executeItems =>
            {
                executeItems.Sql = "DELETE FROM users WHERE username = @username;";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@username", model.Username)
                };

                return executeItems;
            });

            if (affectedRows >= 1)
                return Ok();

            return NotFound();
        }
        [HttpPost("GetUserIdByUsername")]
        public async Task<ReturnGetUserIdByUsername> GetUserIdByUsername([FromBody] GetUserIdByUsernameParameters model)
        {
            DataTable dataTable = await DbHelper.ExecuteQueryAsync(executeItems =>
            {
                executeItems.Sql = @"
                    SELECT user_id
                    FROM users
                    WHERE username = @username;";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@username", model.Username)
                };

                return executeItems;
            });

            if (dataTable.Rows.Count == 0)
            {
                return new ReturnGetUserIdByUsername
                {
                    UserId = null
                };
            }

            return new ReturnGetUserIdByUsername
            {
                UserId = (Guid)dataTable.Rows[0]["user_id"]
            };
        }
        [HttpPost("InsertPasskey")]
        public async Task<IActionResult> InsertPasskey([FromBody] InsertPasskeyParameters model)
        {
            DbHelper.ExecuteNonQueryAsyncNoReturn(executeItems =>
            {
                executeItems.Sql = @"
                    INSERT INTO passkeys
                    (credential_id, user_id, public_key, signature_counter, user_handle)
                    VALUES
                    (@credential_id, @user_id, @public_key, @counter, @user_handle);";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@credential_id", NpgsqlTypes.NpgsqlDbType.Bytea) { Value = model.CredentialId },
                    new NpgsqlParameter("@user_id", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = model.UserId },
                    new NpgsqlParameter("@public_key", NpgsqlTypes.NpgsqlDbType.Bytea) { Value = model.PublicKey },
                    new NpgsqlParameter("@counter", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (int)model.Counter },
                    new NpgsqlParameter("@user_handle", NpgsqlTypes.NpgsqlDbType.Bytea) { Value = model.UserId.ToByteArray() }
                };

                return executeItems;
            });

            return Ok();
        }
        [HttpPost("GetPasskeyByCredentialId")]
        public async Task<ReturnGetPasskeyByCredentialId> GetPasskeyByCredentialId([FromBody] GetPasskeyByCredentialIdParameters model)
        {
            DataTable dataTable = await DbHelper.ExecuteQueryAsync(executeItems =>
            {
                executeItems.Sql = @"
                    SELECT *
                    FROM passkeys
                    WHERE credential_id = @credential_id;";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@credential_id", model.CredentialId)
                };

                return executeItems;
            });
            if (dataTable.Rows.Count == 0)
                return new ReturnGetPasskeyByCredentialId
                {
                    Passkey = null
                };
            DataRow row = dataTable.Rows[0];

            return new ReturnGetPasskeyByCredentialId
            {
                Passkey = new Passkey
                {
                    UserId = (Guid)row["user_id"],
                    CredentialId = (byte[])row["credential_id"],
                    PublicKey = (byte[])row["public_key"],
                    SignatureCounter = Convert.ToUInt32(row["signature_counter"]),
                    UserHandle = (byte[])row["user_handle"]
                }
            };
        }
        [HttpPost("UpdateCounter")]
        public async Task<IActionResult> UpdateCounter([FromBody] UpdateCounterParameters model)
        {
            DbHelper.ExecuteNonQueryAsyncNoReturn(executeItems =>
            {
                executeItems.Sql = @"
                    UPDATE passkeys
                    SET signature_counter = @counter
                    WHERE credential_id = @credential_id;";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@counter", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (int)model.NewCounter },
                    new NpgsqlParameter("@credential_id", model.CredentialId)
                };

                return executeItems;
            });

            return Ok();
        }
        [HttpPost("GetUsernameByUserId")]
        public async Task<ReturnGetUsernameByUserId> GetUsernameByUserId([FromBody] GetUsernameByUserIdParameters model)
        {
            DataTable dataTable = await DbHelper.ExecuteQueryAsync(executeItems =>
            {
                executeItems.Sql = @"
                    SELECT username, display_name
                    FROM users
                    WHERE user_id = @userId;";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@userId", model.UserId)
                };

                return executeItems;
            });

            return new ReturnGetUsernameByUserId
            {
                Username = dataTable.Rows[0]["username"].ToString() ?? "Unknown User",
                DisplayName = dataTable.Rows[0]["display_name"].ToString() ?? "Unable to get display name"
            };
        }
        [HttpPost("ViewAllPasskeysByUserId")]
        public async Task<List<ReturnViewAllPasskeysByUserId>> ViewAllPasskeysByUserId([FromBody] ViewAllPasskeysByUserIdParameters model)
        {
            DataTable dataTable = await DbHelper.ExecuteQueryAsync(executeItems =>
            {
                executeItems.Sql = @"
                    SELECT passkeys.display_name, passkeys.create_date
                    FROM passkeys
                    JOIN users ON passkeys.user_id = users.user_id
                    WHERE passkeys.user_id = @userId;";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@userId", model.UserId)
                };

                return executeItems;
            });

            List<ReturnViewAllPasskeysByUserId> passkeys = new List<ReturnViewAllPasskeysByUserId>();
            foreach (DataRow row in dataTable.Rows)
            {
                passkeys.Add(new ReturnViewAllPasskeysByUserId
                {
                    DisplayName = row["display_name"].ToString() ?? "",
                    CreateDate = Convert.ToDateTime(row["create_date"])
                });
            }
            return passkeys;
        }
        [HttpPost("CheckUserExistByUserId")]
        public async Task<ReturnCheckUserExistByUserId> CheckUserExistByUserId([FromBody] CheckUserExistByUserIdParameters model)
        {
            DataTable dataTable = await DbHelper.ExecuteQueryAsync(executeItems =>
            {
                executeItems.Sql = @"
                    SELECT *
                    FROM users
                    WHERE user_id = @userId;";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@userId", model.UserId)
                };

                return executeItems;
            });

            return new ReturnCheckUserExistByUserId
            {
                IsExist = dataTable.Rows.Count >= 1
            };
        }
        [HttpPost("InsertRegisterAdditionalPasskeyKeyId")]
        public async Task<IActionResult> InsertRegisterAdditionalPasskeyKeyId([FromBody] InsertRegisterAdditionalPasskeyKeyIdParameters model)
        {
            int affectedRows = await DbHelper.ExecuteNonQueryAsync(executeItems =>
            {
                executeItems.Sql = @"
                    INSERT INTO register_additional_passkey (key_id, user_id)
                    VALUES (@keyId, @userId);";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@keyId", model.KeyId),
                    new NpgsqlParameter("@userId", model.UserId)
                };

                return executeItems;
            });

            if (affectedRows >= 1)
                return Ok();
            return BadRequest();
        }
        [HttpPost("ValidateCreateRegisterAdditionalPasskeyKeyId")]
        public async Task<ReturnValidateCreateRegisterAdditionalPasskeyKeyId> ValidateCreateRegisterAdditionalPasskeyKeyId([FromBody] ValidateCreateRegisterAdditionalPasskeyKeyIdParameters model)
        {
            DataTable dataTable = await DbHelper.ExecuteQueryAsync(executeItems => {
                executeItems.Sql = @"
                    SELECT key_id
                    FROM register_additional_passkey
                    WHERE key_id = @keyId
                    AND
                    expire_date > NOW();";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@keyId", model.KeyId)
                };

                return executeItems;
            });

            return new ReturnValidateCreateRegisterAdditionalPasskeyKeyId
            {
                IsValid = dataTable.Rows.Count >= 1
            };
        }
        [HttpPost("RemoveRegisterAdditionalPasskeyKeyId")]
        public async Task<IActionResult> RemoveRegisterAdditionalPasskeyKeyId([FromBody] RemoveRegisterAdditionalPasskeyKeyIdParameters model)
        {
            int affectedRows = await DbHelper.ExecuteNonQueryAsync(executeItems =>
            {
                executeItems.Sql = "DELETE FROM register_additional_passkey WHERE key_id = @keyId;";

                executeItems.Parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("keyId", model.KeyId)
                };

                return executeItems;
            });
            if (affectedRows >= 1)
                return Ok();
            return BadRequest();
        }
    }
}