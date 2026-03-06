using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.Cookies;
using Fido2NetLib;
using SEAR_DataContract.Misc;
using SEAR_WEB.Session;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews().AddViewLocalization().AddDataAnnotationsLocalization();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SessionCache>();

builder.Services.AddSingleton<IFido2>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var fido2Config = new Fido2Configuration();
    fido2Config.ServerDomain = Misc.GetDomainUrl();
    fido2Config.ServerName = "SEAR Web";
    if (builder.Environment.IsDevelopment())
    {
        fido2Config.Origins = new HashSet<string> { "https://" + Misc.GetDomainUrl() + ":5002" };
    }
    else
    {
        fido2Config.Origins = new HashSet<string> { "https://" + Misc.GetDomainUrl() };
    }

    return new Fido2(fido2Config);
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
    {
        options.LoginPath = "/Passkey/Index";
        options.AccessDeniedPath = "/Passkey/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
}
else
{
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
    {
        options.LoginPath = "/Passkey/Index";
        options.AccessDeniedPath = "/Passkey/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.HttpOnly = true;
    });
}
builder.Services.AddAuthorization();

var app = builder.Build();

//if (!app.Environment.IsDevelopment())
//{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
//}

var supportedCultures = new[] { "en", "zh-HK" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
localizationOptions.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
app.UseRequestLocalization(localizationOptions);

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UsePathBase("/");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapStaticAssets();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider
                      .GetRequiredService<ILoggerFactory>()
                      .CreateLogger("SEAR Web");
    AppLogger.Initialize(logger);
}

app.Run();