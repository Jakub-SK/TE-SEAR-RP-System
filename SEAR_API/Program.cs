using Microsoft.AspNetCore.Diagnostics;
using Npgsql;
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
            string uuid = Guid.CreateVersion7().ToString();
            try
            {
                DbHelper.ExecuteNonQueryAsyncNoReturn(executeItems =>
                {
                    executeItems.Sql = @"
                        INSERT INTO log_exception
                        (track_uuid, exception_message, app_type, error_type, stack_trace)
                        VALUES
                        (@uuid, @exceptionMessage, @appType, @errorType, @stackTrace);";

                    List<NpgsqlParameter> parameters = new List<NpgsqlParameter>
                    {
                        new NpgsqlParameter("uuid", uuid),
                        new NpgsqlParameter("exceptionMessage", exception.Error.Message),
                        new NpgsqlParameter("errorType", Misc.GetExceptionType(exception.Error.Message).ExceptionType),
                        new NpgsqlParameter("stackTrace", exception.Error.StackTrace ?? string.Empty),
                        new NpgsqlParameter("appType", "SEAR API")
                    };
                    executeItems.Parameters = parameters;

                    return executeItems;
                }, true);
            }
            catch
            {
                if (Misc.CheckIsDevelopmentEnvironment())
                {
                    AppLogger.LogError("Unable to log exception message to database,\nFUCK U >:( Please check is the cloudflared is running when in development environment u \"fuckin stoopid\"");
                }
                else
                {
                    AppLogger.LogError("Unable to log exception message to database, Database down already la ;|");
                }
            }
            await context.Response.WriteAsJsonAsync(new ApiErrorModel
            {
                UUID = uuid,
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