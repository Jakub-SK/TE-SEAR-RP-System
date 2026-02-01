using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SEAR_DataContract.Models;
using SEAR_WEB.Models;

namespace SEAR_WEB.Controllers
{
    public class PasskeyController : Controller
    {
        private readonly IFido2 _fido2;

        public PasskeyController(IFido2 fido2)
        {
            _fido2 = fido2;
        }
        public class RegisterRequestParameters
        {
            public string? Username { get; set; }
            public string? DisplayName { get; set; }
        }
        public class RemoveUserByUsernameParameters
        {
            public string? Username { get; set; }
        }
        public IActionResult Index()
        {
            return RedirectToAction("Login", "Passkey");
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult RegisterPasskey()
        {
            return View();
        }
        [Authorize]
        public IActionResult ViewPasskey()
        {
            ViewData["Passkeys"] = PasskeyModel.ViewAllPasskeysByUserId(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
            return View();
        }
        [HttpPost]
        public IActionResult RegisterRequest([FromBody] RegisterRequestParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.Username) || string.IsNullOrEmpty(parameters.DisplayName))
                return BadRequest();

            Guid? userId = PasskeyModel.GetUserIdByUsername(parameters.Username);

            if (userId == null)
            {
                userId = PasskeyModel.CreateUserAccount(parameters.Username, parameters.DisplayName);
            }

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
        public IActionResult RegisterRequestByUserId()
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ReturnGetUsernameByUserId users = PasskeyModel.GetUsernameByUserId(userId);

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
                    Passkey? existing = PasskeyModel.GetPasskeyByCredentialId(args.CredentialId);

                    // If null then it is unique
                    return existing == null;
                }
            });

            // Store in database
            // username must match what was used during registration (Encoding UTF8)
            // Save to PostgreSQL
            Guid userId = new Guid(result.User.Id);
            PasskeyModel.InsertPasskey(userId, result.Id, result.PublicKey, result.SignCount);

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
                    Passkey? existing = PasskeyModel.GetPasskeyByCredentialId(args.CredentialId);

                    // If null then it is unique
                    return existing == null;
                }
            });

            // Store in database
            // username must match what was used during registration (Encoding UTF8)
            // Save to PostgreSQL
            Guid userId = new Guid(result.User.Id);
            PasskeyModel.InsertPasskey(userId, result.Id, result.PublicKey, result.SignCount);
            
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

            Passkey? storedCredential = PasskeyModel.GetPasskeyByCredentialId(assertionResponse.RawId);

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
                    Passkey? credential = PasskeyModel.GetPasskeyByCredentialId(args.CredentialId);

                    if (credential == null)
                        return false;

                    return credential.UserId == new Guid(args.UserHandle);
                }
            });

            PasskeyModel.UpdateCounter(result.CredentialId, result.SignCount);
            string displayName = PasskeyModel.GetUsernameByUserId(storedCredential.UserId).DisplayName;

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
        public IActionResult CreateRegisterAdditionalPasskey()
        {
            string? cacheUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (cacheUserId == null)
                return BadRequest();
            string? keyId = PasskeyModel.CreateRegisterAdditionalPasskeyUrl(Guid.Parse(cacheUserId));
            if (keyId == null)
                return BadRequest();
            if (SEAR_DataContract.Misc.Misc.CheckIsDevelopmentEnviroment())
            {
                keyId = SEAR_DataContract.Misc.Misc.GetWebsiteUrl() + Url.Action("RegisterAdditionalPasskey", "Passkey") + keyId;
            }
            return Json(new { keyUrl = keyId });
        }
        [HttpGet]
        public IActionResult RegisterAdditionalPasskey(string registerKey)
        {
            if (string.IsNullOrEmpty(registerKey))
                return BadRequest();
            return View();
        }
    }
}