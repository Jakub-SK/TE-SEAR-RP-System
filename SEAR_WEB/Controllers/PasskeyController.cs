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
            Fido2User user = new Fido2User
            {
                DisplayName = username,
                Name = username,
                Id = Encoding.UTF8.GetBytes(username)
            };

            CredentialCreateOptions options = _fido2.RequestNewCredential(new RequestNewCredentialParams
            {
                User = user,
                ExcludeCredentials = new List<PublicKeyCredentialDescriptor>(),
                AuthenticatorSelection = new AuthenticatorSelection
                {
                    UserVerification = UserVerificationRequirement.Preferred
                },
                AttestationPreference = AttestationConveyancePreference.None
            });

            HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

            return Json(options);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterResponse([FromBody] AuthenticatorAttestationRawResponse attestationResponse)
        {
            string? json = HttpContext.Session.GetString("fido2.attestationOptions");
            HttpContext.Session.Remove("fido2.attestationOptions");
            CredentialCreateOptions options = CredentialCreateOptions.FromJson(json!);
            
            RegisteredPublicKeyCredential result = await _fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
            {
                AttestationResponse = attestationResponse,
                OriginalOptions = options,
                IsCredentialIdUniqueToUserCallback = async (args, cancellationToken) =>
                {
                    // Check if credential ID already exists in DB
                    var existing = PasskeyModel.GetPasskeyByCredentialId(args.CredentialId);

                    // If null → unique
                    return existing == null;
                }
            });

            // Store in database
            // username must match what was used during registration (Encoding UTF8)
            // Save to PostgreSQL
            try
            {
                PasskeyModel.InsertPasskey(Encoding.UTF8.GetString(result.User.Id), result.Id, result.PublicKey, result.SignCount, result.User.Id);
            }
            catch
            {
                return Unauthorized();
            }

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