using CarService.Domain.Cars;
using CarService.Domain.Cars.Enums;
using Contracts.RentalEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CarService.Infrastructure.Messaging.Consumers;

public class RentalEndedConsumer(
    ILogger<RentalEndedConsumer> logger,
    ICarRepository carRepository)
    : IConsumer<RentalEndedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RentalEndedIntegrationEvent> context)
    {
        var msg = context.Message;

        var car = await carRepository.GetCarByIdAsync(msg.CarId);
        if (car is null)
        {
            logger.LogWarning(
                "Car {CarId} not found for rental end {RentalId}",
                msg.CarId, msg.RentalId);
            return;
        }

        if (car.Status != AvailabilityStatus.Rented)
        {
            logger.LogInformation(
                "Car {CarId} is not in Rented status (status={Status}), skipping return for rental {RentalId}",
                car.Id, car.Status.Name, msg.RentalId);
            return;
        }

        car.MarkAsReturned();

        await carRepository.UpdateAsync(car);

        logger.LogInformation(
            "Car {CarId} returned and made Available after end of rental {RentalId}",
            car.Id, msg.RentalId);
    }
}
