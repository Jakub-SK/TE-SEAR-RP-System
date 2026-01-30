using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Mvc;
using SEAR_WEB.Models;
using System.Text;

namespace SEAR_WEB.Controllers
{
    public class PasskeyController : Controller
    {
        private readonly IFido2 _fido2;

        public PasskeyController(IFido2 fido2)
        {
            _fido2 = fido2;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterRequest(string username)
        {
            var user = new Fido2User
            {
                DisplayName = username,
                Name = username,
                Id = Encoding.UTF8.GetBytes(username)
            };

            var requestParams = new RequestNewCredentialParams
            {
                User = user,
                ExcludeCredentials = new List<PublicKeyCredentialDescriptor>(),
                AuthenticatorSelection = new AuthenticatorSelection
                {
                    UserVerification = UserVerificationRequirement.Preferred
                },
                AttestationPreference = AttestationConveyancePreference.None
            };

            var options = _fido2.RequestNewCredential(requestParams);

            HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

            return Json(options);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterResponse([FromBody] AuthenticatorAttestationRawResponse attestationResponse)
        {
            var json = HttpContext.Session.GetString("fido2.attestationOptions");
            HttpContext.Session.Remove("fido2.attestationOptions");
            var options = CredentialCreateOptions.FromJson(json!);

            var result = await _fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
            {
                AttestationResponse = attestationResponse,
                OriginalOptions = options,
                IsCredentialIdUniqueToUserCallback = async (args, cancellationToken) =>
                {
                    return true;
                }
            });

            // Store in database
            var credentialId = result.Id;
            var publicKey = result.PublicKey;
            var counter = result.SignCount;
            var userHandle = result.User.Id;

            // username must match what was used during registration
            var username = Encoding.UTF8.GetString(result.User.Id);

            // Save to PostgreSQL
            PasskeyModel.InsertPasskey(username, credentialId, publicKey, counter, userHandle);

            return Ok();
        }
        [HttpPost]
        public IActionResult LoginRequest(string username)
        {
            if (username == null)
                return Unauthorized();
            
            // Load user's credential ID from DB
            var credentialIds = PasskeyModel.GetCredentialIdsByUsername(username);

            if (credentialIds.Count == 0)
                return Unauthorized();
                //return BadRequest("No passkeys registered for user");

            var descriptors = credentialIds.Select(id => new PublicKeyCredentialDescriptor(id)).ToList();

            var options = _fido2.GetAssertionOptions(new GetAssertionOptionsParams
            {
                AllowedCredentials = descriptors,
                UserVerification = UserVerificationRequirement.Preferred
            });

            HttpContext.Session.SetString("fido2.username", username);
            HttpContext.Session.SetString("fido2.assertionOptions", options.ToJson());

            return Json(options);
        }
        [HttpPost]
        public async Task<IActionResult> LoginResponse([FromBody] AuthenticatorAssertionRawResponse assertionResponse)
        {   
            var username = HttpContext.Session.GetString("fido2.username");
            HttpContext.Session.Remove("fido2.username");
            var json = HttpContext.Session.GetString("fido2.assertionOptions");
            HttpContext.Session.Remove("fido2.assertionOptions");
            var options = AssertionOptions.FromJson(json!);

            var storedCredential = PasskeyModel.GetPasskeyByCredentialId(assertionResponse.RawId);

            if (storedCredential == null)
                return Unauthorized();
                //return BadRequest("Credential not found");

            if (storedCredential.Username != username)
                return Unauthorized();

            var result = await _fido2.MakeAssertionAsync(new MakeAssertionParams
            {
                AssertionResponse = assertionResponse,
                OriginalOptions = options,
                StoredPublicKey = storedCredential.PublicKey!,
                StoredSignatureCounter = storedCredential.SignatureCounter,
                IsUserHandleOwnerOfCredentialIdCallback = async (args, cancellationToken) =>
                {
                    return storedCredential.Username == username;
                }
            });

            // Update counter in DB
            PasskeyModel.UpdateCounter(result.CredentialId, result.SignCount);

            return Ok();
        }
    }
}