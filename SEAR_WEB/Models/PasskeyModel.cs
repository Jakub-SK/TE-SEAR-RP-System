using SEAR_DataContract.Misc;
using SEAR_DataContract.Models;
using SEAR_WEB.Misc;

namespace SEAR_WEB.Models
{
    public class PasskeyModel
    {
        public static Guid CreateUserAccount(string username, string displayName)
        {
            return ApiCaller.CallApi<Guid>("Api/ApiPasskey/CreateUserAccount", new CreateUserAccountParameters
            {
                Username = username,
                DisplayName = displayName
            });
        }
        public static void RemoveUserAccountByUsername(string username)
        {
            ApiCaller.CallApi("Api/ApiPasskey/RemoveUserAccountByUsername", new RemoveUserAccountByUsernameParameters
            {
                Username = username
            });
        }
        public static Guid? GetUserIdByUsername(string username)
        {
            return ApiCaller.CallApi<ReturnGetUserIdByUsername>("Api/ApiPasskey/GetUserIdByUsername", new GetUserIdByUsernameParameters
            {
                Username = username
            }).UserId;
        }
        public static void InsertPasskey(Guid userId, byte[] credentialId, byte[] publicKey, uint counter)
        {
            ApiCaller.CallApi("Api/ApiPasskey/InsertPasskey", new InsertPasskeyParameters
            {
                UserId = userId,
                CredentialId = credentialId,
                PublicKey = publicKey,
                Counter = counter
            });
        }
        public static Passkey? GetPasskeyByCredentialId(byte[] credentialId)
        {
            return ApiCaller.CallApi<ReturnGetPasskeyByCredentialId>("Api/ApiPasskey/GetPasskeyByCredentialId", new GetPasskeyByCredentialIdParameters
            {
                CredentialId = credentialId
            }).Passkey;
        }
        public static void UpdateCounter(byte[] credentialId, uint newCounter)
        {
            ApiCaller.CallApi("Api/ApiPasskey/UpdateCounter", new UpdateCounterParameters
            {
                CredentialId = credentialId,
                NewCounter = newCounter
            });
        }
        public static ReturnGetUsernameByUserId GetUsernameByUserId(Guid userId)
        {
            return ApiCaller.CallApi<ReturnGetUsernameByUserId>("Api/ApiPasskey/GetUsernameByUserId", new GetUsernameByUserIdParameters
            {
                UserId = userId
            });
        }
        public static List<ReturnPasskeysByUserId> ViewAllPasskeysByUserId(Guid userId)
        {
            return ApiCaller.CallApi<List<ReturnPasskeysByUserId>>("Api/ApiPasskey/ViewAllPasskeysByUserId", new ViewAllPasskeysByUserIdParameters
            {
                UserId = userId
            });
        }
        public static string? CreateRegisterAdditionalPasskeyUrl(Guid userId)
        {
            if (ApiCaller.CallApi<ReturnCheckUserExistByUserId>("Api/ApiPasskey/CheckUserExistByUserId", new CheckUserExistByUserIdParameters
            {
                UserId = userId
            }).IsExist == true)
            {
                AppLogger.LogInformation("IsExist is true");
                Guid keyId = Guid.NewGuid();
                ApiCaller.CallApi("Api/ApiPasskey/InsertRegisterAdditionalPasskeyKeyId", new InsertRegisterAdditionalPasskeyKeyIdParameters
                {
                    KeyId = keyId,
                    UserId = userId
                });
                return keyId.ToString();
            }
            return null;
        }
        public static bool ValidateCreateRegisterAdditionalPasskeyKeyId(Guid keyId)
        {
            return ApiCaller.CallApi<ReturnValidateCreateRegisterAdditionalPasskeyKeyId>("Api/ApiPasskey/ValidateCreateRegisterAdditionalPasskeyKeyId", new ValidateCreateRegisterAdditionalPasskeyKeyIdParameters
            {
                KeyId = keyId
            }).IsValid;
        }
        public static void RemoveRegisterAdditionalPasskeyKeyId(Guid keyId)
        {
            ApiCaller.CallApi("Api/ApiPasskey/RemoveRegisterAdditionalPasskeyKeyId", new RemoveRegisterAdditionalPasskeyKeyIdParameters
            {
                KeyId = keyId
            });
        }
    }
}