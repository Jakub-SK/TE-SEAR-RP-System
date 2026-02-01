using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SEAR_DataContract.Misc;
using SEAR_DataContract.Models;
using System.Data;

namespace SEAR_API.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class ApiPasskeyController : Controller
    {
        [HttpPost("CreateUserAccount")]
        public Guid CreateUserAccount([FromBody] CreateUserAccountParameters model)
        {
            Guid userID = Guid.NewGuid();
            string sql = @"
                INSERT INTO users
                (user_id, username, display_name)
                VALUES
                (@userID, @username, @displayName);";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@userID", userID),
                new NpgsqlParameter("@username", model.Username),
                new NpgsqlParameter("@displayName", model.DisplayName)
            };

            DBHelper.ExecuteDatabaseNonQuery(sql, parameters);

            return userID;
        }
        [HttpPost("RemoveUserAccountByUsername")]
        public IActionResult RemoveUserAccountByUsername([FromBody] RemoveUserAccountByUsernameParameters model)
        {
            string sql = "DELETE FROM users WHERE username = @username;";

            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@username", model.Username)
            };

            if (DBHelper.ExecuteDatabaseNonQuery(sql, parameters) >= 1)
                return Ok();

            return NotFound();
        }
        [HttpPost("GetUserIdByUsername")]
        public ReturnGetUserIdByUsername GetUserIdByUsername([FromBody] GetUserIdByUsernameParameters model)
        {
            string sql = @"
                SELECT user_id
                FROM users
                WHERE username = @username;";

            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@username", model.Username)
            };

            DataSet ds = DBHelper.ExecuteDatabaseQuery(sql, parameters);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return new ReturnGetUserIdByUsername
                {
                    UserId = null
                };
            }

            return new ReturnGetUserIdByUsername
            {
                UserId = (Guid)ds.Tables[0].Rows[0]["user_id"]
            };
        }
        [HttpPost("InsertPasskey")]
        public IActionResult InsertPasskey([FromBody] InsertPasskeyParameters model)
        {
            string sql = @"
                INSERT INTO passkeys
                (credential_id, user_id, public_key, signature_counter, user_handle)
                VALUES
                (@credential_id, @user_id, @public_key, @counter, @user_handle);";

            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@credential_id", NpgsqlTypes.NpgsqlDbType.Bytea) { Value = model.CredentialId },
                new NpgsqlParameter("@user_id", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = model.UserId },
                new NpgsqlParameter("@public_key", NpgsqlTypes.NpgsqlDbType.Bytea) { Value = model.PublicKey },
                new NpgsqlParameter("@counter", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (int)model.Counter },
                new NpgsqlParameter("@user_handle", NpgsqlTypes.NpgsqlDbType.Bytea) { Value = model.UserId.ToByteArray() }
            };

            DBHelper.ExecuteDatabaseNonQuery(sql, parameters);
            return Ok();
        }
        [HttpPost("GetPasskeyByCredentialId")]
        public ReturnGetPasskeyByCredentialId GetPasskeyByCredentialId([FromBody] GetPasskeyByCredentialIdParameters model)
        {
            string sql = @"
                SELECT *
                FROM passkeys
                WHERE credential_id = @credential_id;";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@credential_id", model.CredentialId)
            };

            DataSet ds = DBHelper.ExecuteDatabaseQuery(sql, parameters);
            if (ds.Tables[0].Rows.Count == 0)
                return new ReturnGetPasskeyByCredentialId
                {
                    Passkey = null
                };
            DataRow row = ds.Tables[0].Rows[0];

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
        public IActionResult UpdateCounter([FromBody] UpdateCounterParameters model)
        {
            string sql = @"
                UPDATE passkeys
                SET signature_counter = @counter
                WHERE credential_id = @credential_id;";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@counter", NpgsqlTypes.NpgsqlDbType.Integer)
                {
                    Value = (int)model.NewCounter
                },
                new NpgsqlParameter("@credential_id", model.CredentialId)
            };

            DBHelper.ExecuteDatabaseNonQuery(sql, parameters);
            return Ok();
        }
        [HttpPost("GetUsernameByUserId")]
        public ReturnGetUsernameByUserId GetUsernameByUserId([FromBody] GetUsernameByUserIdParameters model)
        {
            string sql = @"
                SELECT username, display_name
                FROM users
                WHERE user_id = @userId;";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@userId", model.UserId)
            };

            DataSet ds = DBHelper.ExecuteDatabaseQuery(sql, parameters);

            return new ReturnGetUsernameByUserId
            {
                Username = ds.Tables[0].Rows[0]["username"].ToString() ?? "Unknown User",
                DisplayName = ds.Tables[0].Rows[0]["display_name"].ToString() ?? "Unable to get display name"
            };
        }
        [HttpPost("ViewAllPasskeysByUserId")]
        public List<ReturnPasskeysByUserId> ViewAllPasskeysByUserId([FromBody] ViewAllPasskeysByUserIdParameters model)
        {
            string sql = @"
                SELECT passkeys.display_name, passkeys.create_date
                FROM passkeys
                JOIN users ON passkeys.user_id = users.user_id
                WHERE passkeys.user_id = @userId;";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@userId", model.UserId)
            };

            DataSet ds = DBHelper.ExecuteDatabaseQuery(sql, parameters);

            List<ReturnPasskeysByUserId> passkeys = new List<ReturnPasskeysByUserId>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                passkeys.Add(new ReturnPasskeysByUserId
                {
                    DisplayName = row["display_name"].ToString() ?? "",
                    CreateDate = Convert.ToDateTime(row["create_date"])
                });
            }
            return passkeys;
        }
        [HttpPost("CheckUserExistByUserId")]
        public ReturnCheckUserExistByUserId CheckUserExistByUserId([FromBody] CheckUserExistByUserIdParameters model)
        {
            string sql = @"
                SELECT *
                FROM users
                WHERE user_id = @userId;";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@userId", model.UserId)
            };

            DataSet ds = DBHelper.ExecuteDatabaseQuery(sql, parameters);

            return new ReturnCheckUserExistByUserId
            {
                IsExist = ds.Tables[0].Rows.Count >= 1
            };
        }
        [HttpPost("InsertRegisterAdditionalPasskeyKeyId")]
        public IActionResult InsertRegisterAdditionalPasskeyKeyId([FromBody] InsertRegisterAdditionalPasskeyKeyIdParameters model)
        {
            string sql = @"
                INSERT INTO register_additional_passkey (key_id, user_id)
                VALUES (@keyId, @userId);";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@keyId", model.KeyId),
                new NpgsqlParameter("@userId", model.UserId)
            };

            if (DBHelper.ExecuteDatabaseNonQuery(sql, parameters) >= 1)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost("ValidateCreateRegisterAdditionalPasskeyKeyId")]
        public ReturnValidateCreateRegisterAdditionalPasskeyKeyId ValidateCreateRegisterAdditionalPasskeyKeyId([FromBody] ValidateCreateRegisterAdditionalPasskeyKeyIdParameters model)
        {
            string sql = @"
                SELECT key_id
                FROM register_additional_passkey
                WHERE key_id = @keyId
                AND
                expire_date > NOW();";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@keyId", model.KeyId)
            };

            DataSet ds = DBHelper.ExecuteDatabaseQuery(sql, parameters);

            return new ReturnValidateCreateRegisterAdditionalPasskeyKeyId
            {
                IsValid = ds.Tables[0].Rows.Count >= 1
            };
        }
        [HttpPost("RemoveRegisterAdditionalPasskeyKeyId")]
        public IActionResult RemoveRegisterAdditionalPasskeyKeyId([FromBody] RemoveRegisterAdditionalPasskeyKeyIdParameters model)
        {
            string sql = "DELETE FROM register_additional_passkey WHERE key_id = @keyId;";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("keyId", model.KeyId)
            };

            if (DBHelper.ExecuteDatabaseNonQuery(sql, parameters) >= 1)
                return Ok();
            return BadRequest();
        }
    }
}