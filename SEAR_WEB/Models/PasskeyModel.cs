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
        public static async Task<bool> CheckUserExistByUserId(Guid userId)
        {
            ReturnCheckUserExistByUserId response = await ApiCaller.CallBackObjectApiAsync<ReturnCheckUserExistByUserId>("Api/ApiPasskey/CheckUserExistByUserId", new CheckUserExistByUserIdParameters
            {
                UserId = userId
            });
            return response.IsExist;
        }
        public static async Task<Guid> CreateUserAccount(string username, string displayName)
        {
            return await ApiCaller.CallBackObjectApiAsync<Guid>("Api/ApiPasskey/CreateUserAccount", new CreateUserAccountParameters
            {
                Username = username,
                DisplayName = displayName
            });
        }
        public static async void RemoveUserAccountByUsername(string username)
        {
            ApiCaller.CallBackApiAsync("Api/ApiPasskey/RemoveUserAccountByUsername", new RemoveUserAccountByUsernameParameters
            {
                Username = username
            });
        }
        public static async Task<Guid?> GetUserIdByUsername(string username)
        {
            ReturnGetUserIdByUsername response = await ApiCaller.CallBackObjectApiAsync<ReturnGetUserIdByUsername>("Api/ApiPasskey/GetUserIdByUsername", new GetUserIdByUsernameParameters
            {
                Username = username
            });
            return response.UserId;
        }
        public static async void InsertPasskey(Guid userId, byte[] credentialId, byte[] publicKey, uint counter)
        {
            ApiCaller.CallBackApiAsync("Api/ApiPasskey/InsertPasskey", new InsertPasskeyParameters
            {
                UserId = userId,
                CredentialId = credentialId,
                PublicKey = publicKey,
                Counter = counter
            });
        }
        public static async Task<Passkey?> GetPasskeyByCredentialId(byte[] credentialId)
        {
            ReturnGetPasskeyByCredentialId response = await ApiCaller.CallBackObjectApiAsync<ReturnGetPasskeyByCredentialId>("Api/ApiPasskey/GetPasskeyByCredentialId", new GetPasskeyByCredentialIdParameters
            {
                CredentialId = credentialId
            });
            return response.Passkey;
        }
        public static async void UpdateCounter(byte[] credentialId, uint newCounter)
        {
            ApiCaller.CallBackApiAsync("Api/ApiPasskey/UpdateCounter", new UpdateCounterParameters
            {
                CredentialId = credentialId,
                NewCounter = newCounter
            });
        }
        public static async Task<ReturnGetUsernameByUserId> GetUsernameByUserId(Guid userId)
        {
            return await ApiCaller.CallBackObjectApiAsync<ReturnGetUsernameByUserId>("Api/ApiPasskey/GetUsernameByUserId", new GetUsernameByUserIdParameters
            {
                UserId = userId
            });
        }
        public static async Task<List<ReturnViewAllPasskeysByUserId>> ViewAllPasskeysByUserId(Guid userId)
        {
            return await ApiCaller.CallBackObjectApiAsync<List<ReturnViewAllPasskeysByUserId>>("Api/ApiPasskey/ViewAllPasskeysByUserId", new ViewAllPasskeysByUserIdParameters
            {
                UserId = userId
            });
        }
        public static async Task<string> CreateRegisterAdditionalPasskeyUrl(Guid userId)
        {
            bool response = await CheckUserExistByUserId(userId);
            if (response == true)
            {
                Guid keyId = Guid.NewGuid();
                ApiCaller.CallBackApiAsync("Api/ApiPasskey/InsertRegisterAdditionalPasskeyKeyId", new InsertRegisterAdditionalPasskeyKeyIdParameters
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
            ReturnValidateCreateRegisterAdditionalPasskeyKeyId response = await ApiCaller.CallBackObjectApiAsync<ReturnValidateCreateRegisterAdditionalPasskeyKeyId>("Api/ApiPasskey/ValidateCreateRegisterAdditionalPasskeyKeyId", new ValidateCreateRegisterAdditionalPasskeyKeyIdParameters
            {
                KeyId = keyId
            });
            return response.IsValid;
        }
        public static async void RemoveRegisterAdditionalPasskeyKeyId(Guid keyId)
        {
            ApiCaller.CallBackApiAsync("Api/ApiPasskey/RemoveRegisterAdditionalPasskeyKeyId", new RemoveRegisterAdditionalPasskeyKeyIdParameters
            {
                KeyId = keyId
            });
        }
    }
}