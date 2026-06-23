using System.Text.Json;
using Contracts.Common;
using Microsoft.EntityFrameworkCore;

namespace PaymentService.Api.Common
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AccountDeactivatedException ex)
            {
                await WriteAsync(context, StatusCodes.Status403Forbidden, new { error = "account_deactivated", message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteAsync(context, StatusCodes.Status401Unauthorized, new { error = "unauthorized", message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                await WriteAsync(context, StatusCodes.Status404NotFound, new { error = "not_found", message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                await WriteAsync(context, StatusCodes.Status400BadRequest, new { error = "bad_request", message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                await WriteAsync(context, StatusCodes.Status409Conflict, new { error = "conflict", message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "Database update conflict");
                await WriteAsync(context, StatusCodes.Status409Conflict, new { error = "conflict", message = "Resource already exists or violates a database constraint" });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Upstream unavailable");
                await WriteAsync(context, StatusCodes.Status502BadGateway, new { error = "upstream_unavailable", message = ex.Message });
            }
        }

        private static async Task WriteAsync(HttpContext context, int statusCode, object body)
        {
            if (context.Response.HasStarted) return;
            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, body);
        }
    }
}
