using Npgsql;
using SEAR_DataContract.Misc;
using System.Data;

namespace SEAR_WEB.Models
{
    public class PasskeyModel
    {
        public class Passkey
        {
            public string? Username { get; set; }
            public byte[]? CredentialId { get; set; }
            public byte[]? PublicKey { get; set; }
            public uint SignatureCounter { get; set; }
            public byte[]? UserHandle { get; set; }
        }
        public static void InsertPasskey(string username, byte[] credentialId, byte[] publicKey, uint counter, byte[] userHandle)
        {
            string sql = @"
                INSERT INTO passkeys
                (username, credential_id, public_key, signature_counter, user_handle)
                VALUES
                (@username, @credential_id, @public_key, @counter, @user_handle)";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@username", username),
                new NpgsqlParameter("@credential_id", credentialId),
                new NpgsqlParameter("@public_key", publicKey),
                new NpgsqlParameter("@counter", counter),
                new NpgsqlParameter("@user_handle", userHandle)
            };

            DBHelper.ExecuteDatabaseNonQuery(sql, parameters);
        }
        public static Passkey? GetPasskeyByCredentialId(byte[] credentialId)
        {
            string sql = @"
                SELECT *
                FROM passkeys
                WHERE credential_id = @credential_id";

            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@credential_id", credentialId)
            };

            var ds = DBHelper.ExecuteDatabaseQuery(sql, parameters);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            var row = ds.Tables[0].Rows[0];

            return new Passkey
            {
                Username = row["username"].ToString(),
                CredentialId = (byte[])row["credential_id"],
                PublicKey = (byte[])row["public_key"],
                SignatureCounter = Convert.ToUInt32(row["signature_counter"]),
                UserHandle = (byte[])row["user_handle"]
            };
        }
        public static List<byte[]> GetCredentialIdsByUsername(string username)
        {
            string sql = @"
                SELECT credential_id
                FROM passkeys
                WHERE username = @username";

            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@username", username)
            };

            var ds = DBHelper.ExecuteDatabaseQuery(sql, parameters);

            var list = new List<byte[]>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add((byte[])row["credential_id"]);
            }

            return list;
        }
        public static void UpdateCounter(byte[] credentialId, uint newCounter)
        {
            string sql = @"
                UPDATE passkeys
                SET signature_counter = @counter
                WHERE credential_id = @credential_id";

            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@counter", newCounter),
                new NpgsqlParameter("@credential_id", credentialId)
            };

            DBHelper.ExecuteDatabaseNonQuery(sql, parameters);
        }
    }
}
