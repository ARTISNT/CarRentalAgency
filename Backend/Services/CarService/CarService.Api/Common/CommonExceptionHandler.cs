using System.Text.Json;
using AutoMapper;
using Contracts.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

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

            ArgumentNullException arg => (StatusCodes.Status400BadRequest, new { error = "bad_request", message = arg.Message }),
            ArgumentException arg => (StatusCodes.Status400BadRequest, new { error = "bad_request", message = arg.Message }),

            InvalidOperationException inv => (StatusCodes.Status400BadRequest, new { error = "bad_request", message = inv.Message }),

            HttpRequestException => (StatusCodes.Status502BadGateway, new { error = "upstream_unavailable" }),

            AutoMapperMappingException am => (StatusCodes.Status400BadRequest, new { error = "bad_request", message = ExtractAutoMapperMessage(am) }),

            DbUpdateException db => (StatusCodes.Status409Conflict, new { error = "conflict", message = ExtractConstraintMessage(db) }),

            _ => (StatusCodes.Status500InternalServerError, new { error = "internal_error" })
        };
    }

    private static string ExtractAutoMapperMessage(AutoMapperMappingException ex)
    {
        var inner = ex.InnerException?.Message ?? ex.Message;
        return inner.Split('\n', 2)[0];
    }

    private static string ExtractConstraintMessage(DbUpdateException db)
    {
        var inner = db.InnerException?.Message ?? string.Empty;
        if (inner.Contains("IX_cars_license_plate", StringComparison.OrdinalIgnoreCase)
            || inner.Contains("license_plate", StringComparison.OrdinalIgnoreCase))
            return "Car with this license plate already exists";

        if (inner.Contains("IX_cars_vin_code", StringComparison.OrdinalIgnoreCase)
            || inner.Contains("vin_code", StringComparison.OrdinalIgnoreCase))
            return "Car with this VIN code already exists";

        if (inner.Contains("IX_Transactions_TrackingId", StringComparison.OrdinalIgnoreCase))
            return "Transaction with this tracking id already exists";

        if (inner.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase)
            || inner.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
            return "Resource already exists";

        return "Database constraint violation";
    }
}
