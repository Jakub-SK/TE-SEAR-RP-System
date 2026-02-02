namespace SEAR_DataContract.Models
{
    public class Passkey
    {
        public Guid UserId { get; set; }
        public byte[]? CredentialId { get; set; }
        public byte[]? PublicKey { get; set; }
        public uint SignatureCounter { get; set; }
        public byte[]? UserHandle { get; set; }
    }
    public class CreateUserAccountParameters
    {
        public required string Username { get; set; }
        public required string DisplayName { get; set; }
    }
    public class RemoveUserAccountByUsernameParameters
    {
        public required string Username { get; set; }
    }
    public class GetUserIdByUsernameParameters
    {
        public required string Username { get; set; }
    }
    public class ReturnGetUserIdByUsername
    {
        public Guid? UserId { get; set; }
    }
    public class InsertPasskeyParameters
    {
        public required Guid UserId { get; set; }
        public required byte[] CredentialId { get; set; }
        public required byte[] PublicKey { get; set; }
        public required uint Counter { get; set; }
    }
    public class GetPasskeyByCredentialIdParameters
    {
        public required byte[] CredentialId { get; set; }
    }
    public class ReturnGetPasskeyByCredentialId
    {
        public required Passkey? Passkey { get; set; }
    }
    public class UpdateCounterParameters
    {
        public required byte[] CredentialId { get; set; }
        public required uint NewCounter { get; set; }
    }
    public class GetUsernameByUserIdParameters
    {
        public required Guid UserId { get; set; }
    }
    public class ReturnGetUsernameByUserId
    {
        public required string Username { get; set; }
        public required string DisplayName { get; set; }
    }
    public class ViewAllPasskeysByUserIdParameters{
        public required Guid UserId { get; set; }
    }
    public class ReturnViewAllPasskeysByUserId
    {
        public required string DisplayName { get; set; }
        public required DateTime CreateDate { get; set; }
    }
    public class CheckUserExistByUserIdParameters
    {
        public required Guid UserId { get; set; }
    }
    public class ReturnCheckUserExistByUserId
    {
        public required bool IsExist { get; set; }
    }
    public class InsertRegisterAdditionalPasskeyKeyIdParameters
    {
        public required Guid KeyId { get; set; }
        public required Guid UserId { get; set; }
    }
    public class ValidateCreateRegisterAdditionalPasskeyKeyIdParameters
    {
        public required Guid KeyId { get; set; }
    }
    public class ReturnValidateCreateRegisterAdditionalPasskeyKeyId
    {
        public required bool IsValid { get; set; }
    }
    public class RemoveRegisterAdditionalPasskeyKeyIdParameters
    {
        public required Guid KeyId { get; set; }
    }
}