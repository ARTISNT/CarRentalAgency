using CarService.Domain.Cars;
using Contracts.RentalEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CarService.Infrastructure.Messaging.Consumers;

public class RentalScheduledConsumer(
    ILogger<RentalScheduledConsumer> logger,
    ICarRepository carRepository)
    : IConsumer<RentalScheduledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RentalScheduledIntegrationEvent> context)
    {
        var msg = context.Message;

        var car = await carRepository.GetCarByIdAsync(msg.CarId);
        if (car is null)
        {
            logger.LogWarning("Car {CarId} not found for rental schedule", msg.CarId);
            return;
        }

        car.Reserve(msg.UserId);
        await carRepository.UpdateAsync(car);

        logger.LogInformation(
            "Car {CarId} reserved by user {UserId} for rental {RentalId}",
            msg.CarId, msg.UserId, msg.RentalId);
    }
}
