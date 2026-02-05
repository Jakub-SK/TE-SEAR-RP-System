using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SEAR_DataContract.Misc;
using SEAR_DataContract.Models;
using SEAR_WEB.Models;
using SEAR_WEB.RedirectViewModels;
using System.Security.Claims;

namespace SEAR_WEB.Controllers
{
    public class PasskeyController : Controller
    {
        private readonly IFido2 _fido2;
        public PasskeyController(IFido2 fido2)
        {
            _fido2 = fido2;
        }
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Login", "Passkey");
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        public async Task<IActionResult> RegisterPasskey()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> ViewPasskey()
        {
            ViewData["Passkeys"] = await PasskeyModel.ViewAllPasskeysByUserId(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterRequest([FromBody] RegisterRequestParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.Username) || string.IsNullOrEmpty(parameters.DisplayName))
                return BadRequest();

            Guid? userId = await PasskeyModel.GetUserIdByUsername(parameters.Username);

            if (userId != null)
                return BadRequest();

            userId = await PasskeyModel.CreateUserAccount(parameters.Username, parameters.DisplayName);

            Fido2User user = new Fido2User
            {
                DisplayName = parameters.DisplayName,
                Name = parameters.Username,
                Id = userId.Value.ToByteArray()
            };

            CredentialCreateOptions options = _fido2.RequestNewCredential(new RequestNewCredentialParams
            {
                User = user,
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
        public async Task<IActionResult> RegisterRequestByUserId()
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ReturnGetUsernameByUserId users = await PasskeyModel.GetUsernameByUserId(userId);

            Fido2User user = new Fido2User
            {
                DisplayName = users.DisplayName,
                Name = users.Username,
                Id = userId.ToByteArray()
            };

            CredentialCreateOptions options = _fido2.RequestNewCredential(new RequestNewCredentialParams
            {
                User = user,
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
            string json = HttpContext.Session.GetString("fido2.attestationOptions")!;
            HttpContext.Session.Remove("fido2.attestationOptions");
            CredentialCreateOptions options = CredentialCreateOptions.FromJson(json);
            
            RegisteredPublicKeyCredential result = await _fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
            {
                AttestationResponse = attestationResponse,
                OriginalOptions = options,
                IsCredentialIdUniqueToUserCallback = async (args, cancellationToken) =>
                {
                    // Check if credential ID already exists in DB
                    Passkey? existing = await PasskeyModel.GetPasskeyByCredentialId(args.CredentialId);

                    // If null then it is unique
                    return existing == null;
                }
            });

            // Store in database
            // userId must match what was used during registration (Encoding UTF8)
            // Save to PostgreSQL
            PasskeyModel.InsertPasskey(new Guid(result.User.Id), result.Id, result.PublicKey, result.SignCount);

            return Json(new { success = true, redirectUrl = "/Passkey/Login" }); ;
        }
        [HttpPost]
        public async Task<IActionResult> RegisterResponseByUserId([FromBody] AuthenticatorAttestationRawResponse attestationResponse)
        {
            string json = HttpContext.Session.GetString("fido2.attestationOptions")!;
            HttpContext.Session.Remove("fido2.attestationOptions");
            CredentialCreateOptions options = CredentialCreateOptions.FromJson(json);

            RegisteredPublicKeyCredential result = await _fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
            {
                AttestationResponse = attestationResponse,
                OriginalOptions = options,
                IsCredentialIdUniqueToUserCallback = async (args, cancellationToken) =>
                {
                    // Check if credential ID already exists in DB
                    Passkey? existing = await PasskeyModel.GetPasskeyByCredentialId(args.CredentialId);

                    // If null then it is unique
                    return existing == null;
                }
            });

            // Store in database
            // userId must match what was used during registration (Encoding UTF8)
            // Save to PostgreSQL
            PasskeyModel.InsertPasskey(new Guid(result.User.Id), result.Id, result.PublicKey, result.SignCount);

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("IsRegisteringByUrl")))
                PasskeyModel.RemoveRegisterAdditionalPasskeyKeyId(Guid.Parse(HttpContext.Session.GetString("IsRegisteringByUrl")!));
            HttpContext.Session.Remove("IsRegisteringByUrl");

            return Json(new { success = true, redirectUrl = "/Passkey/ViewPasskey" }); ;
        }
        [HttpPost]
        public IActionResult RemoveUserByUsername([FromBody] RemoveUserByUsernameParameters parameters)
        {
            PasskeyModel.RemoveUserAccountByUsername(parameters.Username!);
            return Ok();
        }
        [HttpPost]
        public IActionResult LoginRequest()
        {
            AssertionOptions options = _fido2.GetAssertionOptions(new GetAssertionOptionsParams
            {
                UserVerification = UserVerificationRequirement.Preferred
            });

            HttpContext.Session.SetString("fido2.assertionOptions", options.ToJson());

            return Json(options);
        }
        [HttpPost]
        public async Task<IActionResult> LoginResponse([FromBody] AuthenticatorAssertionRawResponse assertionResponse)
        {
            string json = HttpContext.Session.GetString("fido2.assertionOptions")!;
            HttpContext.Session.Remove("fido2.assertionOptions");
            AssertionOptions options = AssertionOptions.FromJson(json);

            Passkey? storedCredential = await PasskeyModel.GetPasskeyByCredentialId(assertionResponse.RawId);

            if (storedCredential == null)
                return Unauthorized();

            var result = await _fido2.MakeAssertionAsync(new MakeAssertionParams
            {
                AssertionResponse = assertionResponse,
                OriginalOptions = options,
                StoredPublicKey = storedCredential.PublicKey!,
                StoredSignatureCounter = storedCredential.SignatureCounter,
                IsUserHandleOwnerOfCredentialIdCallback = async (args, cancellationToken) =>
                {
                    Passkey? credential = await PasskeyModel.GetPasskeyByCredentialId(args.CredentialId);

                    if (credential == null)
                        return false;

                    return credential.UserId == new Guid(args.UserHandle);
                }
            });

            PasskeyModel.UpdateCounter(result.CredentialId, result.SignCount);
            ReturnGetUsernameByUserId response = await PasskeyModel.GetUsernameByUserId(storedCredential.UserId);
            string displayName = response.DisplayName;

            // Create claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, storedCredential.UserId.ToString()),
                new Claim("DisplayName", displayName)
            };

            // Create identity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign in user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return Json(new { success = true, redirectUrl = "/Home/Index" });
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Passkey");
        }
        [HttpPost]
        public async Task<IActionResult> CreateRegisterAdditionalPasskey()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            string keyId = await PasskeyModel.CreateRegisterAdditionalPasskeyUrl(Guid.Parse(userId));

            if (string.IsNullOrEmpty(keyId))
                return BadRequest();

            keyId = SEAR_DataContract.Misc.Misc.GetWebsiteUrl() + Url.Action("RegisterAdditionalPasskey", "Passkey") + "/" + keyId;

            return Json(new { keyUrl = keyId });
        }
        [HttpGet("Passkey/RegisterAdditionalPasskey/{registerKey}")]
        public async Task<IActionResult> RegisterAdditionalPasskey(string registerKey)
        {
            if (string.IsNullOrEmpty(registerKey))
                return BadRequest();

            bool response = await PasskeyModel.ValidateCreateRegisterAdditionalPasskeyKeyId(Guid.Parse(registerKey));
            if (response)
            {
                return View("RegisterAdditionalPasskey", new RegisterAdditionalPasskeyKeyIdViewModel
                {
                    KeyId = registerKey
                });
            }
            return Unauthorized();
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmRegisterAdditionalPasskey([FromBody] RegisterAdditionalPasskeyKeyIdViewModel model)
        {
            if (string.IsNullOrEmpty(model.KeyId))
                return BadRequest();

            bool response = await PasskeyModel.ValidateCreateRegisterAdditionalPasskeyKeyId(Guid.Parse(model.KeyId));
            if (response)
            {
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                ReturnGetUsernameByUserId users = await PasskeyModel.GetUsernameByUserId(userId);

                Fido2User user = new Fido2User
                {
                    DisplayName = users.DisplayName,
                    Name = users.Username,
                    Id = userId.ToByteArray()
                };

                CredentialCreateOptions options = _fido2.RequestNewCredential(new RequestNewCredentialParams
                {
                    User = user,
                    AuthenticatorSelection = new AuthenticatorSelection
                    {
                        UserVerification = UserVerificationRequirement.Preferred
                    },
                    AttestationPreference = AttestationConveyancePreference.None
                });

                HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());
                HttpContext.Session.SetString("IsRegisteringByUrl", model.KeyId);

                return Json(options);
            }
            return Unauthorized();
        }
    }
}