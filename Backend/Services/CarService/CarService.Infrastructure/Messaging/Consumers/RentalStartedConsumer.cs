using CarService.Domain.Cars;
using Contracts.RentalEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CarService.Infrastructure.Messaging.Consumers;

public class RentalStartedConsumer(
    ILogger<RentalStartedConsumer> logger,
    ICarRepository carRepository)
    : IConsumer<RentalStartedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RentalStartedIntegrationEvent> context)
    {
        var msg = context.Message;

        var car = await carRepository.GetCarByIdAsync(msg.CarId);
        if (car is null)
        {
            logger.LogWarning("Car {CarId} not found for rental start", msg.CarId);
            return;
        }

        car.Rent(msg.UserId);
        await carRepository.UpdateAsync(car);

        logger.LogInformation(
            "Car {CarId} marked as Rented by user {UserId} for rental {RentalId}",
            msg.CarId, msg.UserId, msg.RentalId);
    }
}
