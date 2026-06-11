using RentalService.Domain.Rentals;

namespace RentalService.Api.BackgroundServices;

public class RentalExpirationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RentalExpirationService> _logger;

    public RentalExpirationService(IServiceScopeFactory scopeFactory, ILogger<RentalExpirationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RentalExpirationService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IRentalRepository>();

                var expiredRentals = await repository.GetExpiredAwaitingConfirmationRentalsAsync(stoppingToken);

                foreach (var rental in expiredRentals)
                {
                    rental.CancelRental(DateTime.UtcNow);
                    await repository.UpdateRentalAsync(rental, stoppingToken);
                    _logger.LogInformation("Rental {RentalId} auto-cancelled due to expiration", rental.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in rental expiration service");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
