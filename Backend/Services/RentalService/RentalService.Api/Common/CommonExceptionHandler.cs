using System.Text.Json;
using AutoMapper;
using Contracts.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RentalService.Application.Exceptions;

namespace Api.Common;

public static class CommonExceptionHandler
{
    public static void UseCommonExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(builder => builder.Run(async context =>
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = feature?.Error;

            var (statusCode, body) = MapException(exception);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(body));
        }));
    }

    private static (int StatusCode, object Body) MapException(Exception? exception)
    {
        return exception switch
        {
            null => (StatusCodes.Status500InternalServerError, new { error = "internal_error" }),

            AccountDeactivatedException => (StatusCodes.Status403Forbidden, new { error = "account_deactivated" }),

            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, new { error = "unauthorized" }),

            KeyNotFoundException => (StatusCodes.Status404NotFound, new { error = "not_found" }),

            ForbiddenException f => (StatusCodes.Status403Forbidden, new { error = "forbidden", message = f.Message }),

            ArgumentNullException arg => (StatusCodes.Status400BadRequest, new { error = "bad_request", message = arg.Message }),
            ArgumentException arg => (StatusCodes.Status400BadRequest, new { error = "bad_request", message = arg.Message }),

            InvalidOperationException inv => (StatusCodes.Status400BadRequest, new { error = "bad_request", message = inv.Message }),

            HttpRequestException => (StatusCodes.Status502BadGateway, new { error = "upstream_unavailable" }),

            AutoMapperMappingException am => (StatusCodes.Status400BadRequest, new { error = "bad_request", message = (am.InnerException?.Message ?? am.Message).Split('\n', 2)[0] }),

            DbUpdateException db => (StatusCodes.Status409Conflict, new { error = "conflict", message = "Resource already exists or violates a database constraint" }),

            _ => (StatusCodes.Status500InternalServerError, new { error = "internal_error" })
        };
    }
}
