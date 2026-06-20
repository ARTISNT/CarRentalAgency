using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Application.EmailOutbox;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure.EmailOutbox;

public class OutboxDispatcherHostedService(
    IServiceScopeFactory scopeFactory,
    IHttpClientFactory httpClientFactory,
    IOptions<OutboxOptions> options,
    ILogger<OutboxDispatcherHostedService> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly OutboxOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Outbox dispatcher started (PollInterval={PollInterval}s, BatchSize={BatchSize}, MaxAttempts={MaxAttempts})",
            _options.PollIntervalSeconds, _options.BatchSize, _options.MaxAttempts);

        var pollDelay = TimeSpan.FromSeconds(_options.PollIntervalSeconds);
        var lockDuration = TimeSpan.FromMinutes(_options.LockMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken, lockDuration);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox dispatcher iteration failed");
            }

            try
            {
                await Task.Delay(pollDelay, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        logger.LogInformation("Outbox dispatcher stopped");
    }

    private async Task ProcessBatchAsync(CancellationToken stoppingToken, TimeSpan lockDuration)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var reader = scope.ServiceProvider.GetRequiredService<IOutboxReader>();

        var now = DateTime.UtcNow;
        var batch = await reader.ClaimPendingAsync(now, _options.BatchSize, lockDuration, stoppingToken);
        if (batch.Count == 0)
            return;

        var http = httpClientFactory.CreateClient("NotificationService");

        foreach (var entry in batch)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            await ProcessEntryAsync(reader, http, entry, stoppingToken);
        }
    }

    private async Task ProcessEntryAsync(
        IOutboxReader reader,
        HttpClient http,
        EmailOutboxEntry entry,
        CancellationToken stoppingToken)
    {
        try
        {
            using var payloadContent = JsonContent.Create(ParsePayload(entry.PayloadJson));
            using var response = await http.PostAsync(
                "/api/notifications/email-verification",
                payloadContent,
                stoppingToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(stoppingToken);
                await HandleFailureAsync(reader, entry, $"HTTP {(int)response.StatusCode}: {body}", stoppingToken);
                return;
            }

            await reader.MarkProcessedAsync(entry.Id, stoppingToken);
            logger.LogInformation(
                "Outbox email dispatched: id={Id}, user={UserId}, type={EventType}, attempts={Attempts}",
                entry.Id, entry.UserId, entry.EventType, entry.Attempts + 1);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            await HandleFailureAsync(reader, entry, ex.Message, stoppingToken);
        }
    }

    private async Task HandleFailureAsync(
        IOutboxReader reader,
        EmailOutboxEntry entry,
        string errorMessage,
        CancellationToken stoppingToken)
    {
        var attempts = entry.Attempts + 1;
        var nextAttemptAt = ComputeNextAttempt(attempts, DateTime.UtcNow);

        if (attempts >= _options.MaxAttempts)
        {
            logger.LogError(
                "Outbox email permanently failed: id={Id}, user={UserId}, attempts={Attempts}, error={Error}",
                entry.Id, entry.UserId, attempts, errorMessage);
        }
        else
        {
            logger.LogWarning(
                "Outbox email failed: id={Id}, user={UserId}, attempt={Attempt}, nextRetry={NextRetry}, error={Error}",
                entry.Id, entry.UserId, attempts, nextAttemptAt, errorMessage);
        }

        await reader.MarkFailedAsync(entry.Id, attempts, errorMessage, nextAttemptAt, stoppingToken);
    }

    private DateTime ComputeNextAttempt(int attempts, DateTime now)
    {
        var seconds = Math.Pow(2, Math.Min(attempts, 6));
        return now.AddSeconds(seconds);
    }

    private static object ParsePayload(string json)
    {
        var payload = JsonSerializer.Deserialize<JsonElement>(json, JsonOptions);
        return payload.Deserialize<object>(JsonOptions) ?? new { };
    }
}
