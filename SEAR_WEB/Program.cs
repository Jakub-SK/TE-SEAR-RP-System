using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.HttpOverrides;
using SEAR_DataContract;
using SEAR_WEB.Session;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//builder.Services.AddHttpClient<ApiCaller>(client =>
//{
//    client.BaseAddress = new Uri("https://localhost:7001/");
//});
builder.Services.AddHttpContextAccessor();

//Add Apis Registration here
//builder.Services.AddScoped<HomeApi>();
//Register Session Cache Class
builder.Services.AddScoped<SessionCache>();
//End
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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();