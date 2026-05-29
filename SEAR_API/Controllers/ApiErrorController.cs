using Microsoft.AspNetCore.Mvc;
using SEAR_DataContract.Misc;
using SEAR_DataContract.Models;
using Npgsql;

namespace SEAR_API.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class ApiErrorController : Controller
    {
        [HttpPost("LogException")]
        public async Task<ShowExceptionMessage> LogException([FromBody] LogExceptionParameters model)
        {
            ExceptionTypeModel exceptionType = Misc.GetExceptionType(model.Exception);
            ShowExceptionMessage display = new ShowExceptionMessage
            {
                UUID = model.UUID ?? Guid.CreateVersion7().ToString(),
                IsApi500 = exceptionType.IsApi500,
                ExceptionType = exceptionType.ExceptionType
            };

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
                        new NpgsqlParameter("uuid", model.UUID),
                        new NpgsqlParameter("exceptionMessage", model.Exception.Message),
                        new NpgsqlParameter("errorType", exceptionType.ExceptionType),
                        new NpgsqlParameter("stackTrace", model.Exception.StackTrace ?? string.Empty)
                    };
                    if (!string.IsNullOrEmpty(model.AppType))
                    {
                        parameters.Add(new NpgsqlParameter("appType", model.AppType));
                    }
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
            return display;
        }
        [HttpPost("SubmitExceptionSteps")]
        public async Task<IActionResult> SubmitExceptionSteps([FromBody] SubmitExceptionStepsParameters model)
        {
            try
            {
                DbHelper.ExecuteNonQueryAsyncNoReturn(executeItems =>
                {
                    executeItems.Sql = @"
                        UPDATE log_exception
                        SET steps = @Steps
                        WHERE track_uuid = @UUID;";

                    executeItems.Parameters = new List<NpgsqlParameter>
                    {
                        new NpgsqlParameter("Steps", model.StepsToReproduce),
                        new NpgsqlParameter("UUID", model.UUID)
                    };

                    return executeItems;
                }, true);
            }
            catch
            {
                if (Misc.CheckIsDevelopmentEnvironment())
                {
                    AppLogger.LogError("Unable to update steps to database,\nFUCK U >:( Please check is the cloudflared is running when in development environment u \"fuckin stoopid\"");
                }
                else
                {
                    AppLogger.LogError("Unable to update steps to database, Database down already la ;|");
                }
            }
            return Ok();
        }
    }
}