using Microsoft.AspNetCore.HttpOverrides;
using Fido2NetLib;
using SEAR_DataContract.Misc;
using SEAR_WEB.Session;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

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
    if (builder.Environment.IsDevelopment())
    {
        fido2Config.ServerDomain = "localhost";
        fido2Config.ServerName = "SEAR Web";
        fido2Config.Origins = new HashSet<string> { "https://localhost:5002" };
    }
    else
    {
        fido2Config.ServerDomain = "noobxryan.org";
        fido2Config.ServerName = "SEAR Web";
        fido2Config.Origins = new HashSet<string> { "https://tesear.noobxryan.org" };
    }

    return new Fido2(fido2Config);
});

var app = builder.Build();

//if (!app.Environment.IsDevelopment())
//{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
//}

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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider
                      .GetRequiredService<ILoggerFactory>()
                      .CreateLogger("Global");

    AppLogger.Initialize(logger);
}

app.Run();