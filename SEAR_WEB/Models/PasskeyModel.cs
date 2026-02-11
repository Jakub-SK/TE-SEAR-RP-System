using SEAR_DataContract.Models;
using SEAR_WEB.Misc;

namespace SEAR_WEB.Models
{
    public class RegisterRequestParameters
    {
        public required string Username { get; set; }
        public required string DisplayName { get; set; }
    }
    public class RemoveUserByUsernameParameters
    {
        public required string Username { get; set; }
    }
    public class PasskeyModel
    {
        public static async Task<Guid> CreateUserAccount(string username, string displayName)
        {
            return await ApiCaller.CallApiAsync<Guid>("Api/ApiPasskey/CreateUserAccount", new CreateUserAccountParameters
            {
                Username = username,
                DisplayName = displayName
            });
        }
        public static async void RemoveUserAccountByUsername(string username)
        {
            ApiCaller.CallApiAsync("Api/ApiPasskey/RemoveUserAccountByUsername", new RemoveUserAccountByUsernameParameters
            {
                Username = username
            });
        }
        public static async Task<Guid?> GetUserIdByUsername(string username)
        {
            ReturnGetUserIdByUsername response = await ApiCaller.CallApiAsync<ReturnGetUserIdByUsername>("Api/ApiPasskey/GetUserIdByUsername", new GetUserIdByUsernameParameters
            {
                Username = username
            });
            return response.UserId;
        }
        public static async void InsertPasskey(Guid userId, byte[] credentialId, byte[] publicKey, uint counter)
        {
            ApiCaller.CallApiAsync("Api/ApiPasskey/InsertPasskey", new InsertPasskeyParameters
            {
                UserId = userId,
                CredentialId = credentialId,
                PublicKey = publicKey,
                Counter = counter
            });
        }
        public static async Task<Passkey?> GetPasskeyByCredentialId(byte[] credentialId)
        {
            ReturnGetPasskeyByCredentialId response = await ApiCaller.CallApiAsync<ReturnGetPasskeyByCredentialId>("Api/ApiPasskey/GetPasskeyByCredentialId", new GetPasskeyByCredentialIdParameters
            {
                CredentialId = credentialId
            });
            return response.Passkey;
        }
        public static async void UpdateCounter(byte[] credentialId, uint newCounter)
        {
            ApiCaller.CallApiAsync("Api/ApiPasskey/UpdateCounter", new UpdateCounterParameters
            {
                CredentialId = credentialId,
                NewCounter = newCounter
            });
        }
        public static async Task<ReturnGetUsernameByUserId> GetUsernameByUserId(Guid userId)
        {
            return await ApiCaller.CallApiAsync<ReturnGetUsernameByUserId>("Api/ApiPasskey/GetUsernameByUserId", new GetUsernameByUserIdParameters
            {
                UserId = userId
            });
        }
        public static async Task<List<ReturnViewAllPasskeysByUserId>> ViewAllPasskeysByUserId(Guid userId)
        {
            return await ApiCaller.CallApiAsync<List<ReturnViewAllPasskeysByUserId>>("Api/ApiPasskey/ViewAllPasskeysByUserId", new ViewAllPasskeysByUserIdParameters
            {
                UserId = userId
            });
        }
        public static async Task<string> CreateRegisterAdditionalPasskeyUrl(Guid userId)
        {
            ReturnCheckUserExistByUserId response = await ApiCaller.CallApiAsync<ReturnCheckUserExistByUserId>("Api/ApiPasskey/CheckUserExistByUserId", new CheckUserExistByUserIdParameters
            {
                UserId = userId
            });
            if (response.IsExist == true)
            {
                Guid keyId = Guid.NewGuid();
                ApiCaller.CallApiAsync("Api/ApiPasskey/InsertRegisterAdditionalPasskeyKeyId", new InsertRegisterAdditionalPasskeyKeyIdParameters
                {
                    KeyId = keyId,
                    UserId = userId
                });
                return keyId.ToString();
            }
            return "";
        }
        public static async Task<bool> ValidateCreateRegisterAdditionalPasskeyKeyId(Guid keyId)
        {
            ReturnValidateCreateRegisterAdditionalPasskeyKeyId response = await ApiCaller.CallApiAsync<ReturnValidateCreateRegisterAdditionalPasskeyKeyId>("Api/ApiPasskey/ValidateCreateRegisterAdditionalPasskeyKeyId", new ValidateCreateRegisterAdditionalPasskeyKeyIdParameters
            {
                KeyId = keyId
            });
            return response.IsValid;
        }
        public static async void RemoveRegisterAdditionalPasskeyKeyId(Guid keyId)
        {
            ApiCaller.CallApiAsync("Api/ApiPasskey/RemoveRegisterAdditionalPasskeyKeyId", new RemoveRegisterAdditionalPasskeyKeyIdParameters
            {
                KeyId = keyId
            });
        }
    }
}