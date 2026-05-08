using Microsoft.AspNetCore.Diagnostics;
using SEAR_DataContract.Misc;
using SEAR_DataContract.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<IExceptionHandlerFeature>();

        if (exception != null)
        {
            ShowExceptionMessage display = await Misc.LogException(exception.Error, "SEAR API");
            await context.Response.WriteAsJsonAsync(new ApiErrorModel
            {
                UUID = display.UUID,
                Message = exception.Error.Message,
                StackTrace = exception.Error.StackTrace
            });
        }
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider
                      .GetRequiredService<ILoggerFactory>()
                      .CreateLogger("SEAR API");
    AppLogger.Initialize(logger);
}

app.Run();