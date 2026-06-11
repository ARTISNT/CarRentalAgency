using RentalService.Domain.Rentals;

namespace RentalService.Api.BackgroundServices;

public class RentalActivationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RentalActivationService> _logger;

    public RentalActivationService(IServiceScopeFactory scopeFactory, ILogger<RentalActivationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RentalActivationService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IRentalRepository>();

                var rentals = await repository.GetScheduledReadyRentalsAsync(stoppingToken);

                foreach (var rental in rentals)
                {
                    rental.ActivateScheduledRental();
                    await repository.UpdateRentalAsync(rental, stoppingToken);
                    _logger.LogInformation("Rental {RentalId} activated by scheduler", rental.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in rental activation service");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
