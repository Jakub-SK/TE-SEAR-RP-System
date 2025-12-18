using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.HttpOverrides;
using SEAR_WEB.AppServer;

var builder = WebApplication.CreateBuilder(args);

// Path where the PFX sits (same folder as exe)
var certPath = Path.Combine(AppContext.BaseDirectory, "SEAR_RP_CERT.pfx");
var certPassword = "SEAR_RP"; // set if you used a password

var cert = X509CertificateLoader.LoadPkcs12FromFile(certPath, certPassword);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(443, listenOptions =>
    {
        listenOptions.UseHttps(cert);
    });
});
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ApiCaller>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7001/");
});
//Add Apis Registration here
builder.Services.AddScoped<HomeApi>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UsePathBase("/");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapStaticAssets();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();