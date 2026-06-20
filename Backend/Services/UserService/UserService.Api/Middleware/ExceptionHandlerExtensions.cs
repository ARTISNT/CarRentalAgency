using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Users;

namespace UserService.Api.Middleware;

public static class ExceptionHandlerExtensions
{
    public static void UseUserServiceExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(builder => builder.Run(async context =>
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = feature?.Error;

            var (statusCode, body) = MapException(exception);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(body);
        }));
    }

    private static (int StatusCode, object Body) MapException(Exception? exception)
    {
        return exception switch
        {
            EmailNotVerifiedException => (StatusCodes.Status403Forbidden, new { error = "email_not_verified" }),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, new { error = "unauthorized" }),
            KeyNotFoundException => (StatusCodes.Status404NotFound, new { error = "not_found" }),
            ArgumentException arg => (StatusCodes.Status400BadRequest, new { error = "bad_request", message = arg.Message }),
            _ => (StatusCodes.Status500InternalServerError, new { error = "internal_error" })
        };
    }
}
