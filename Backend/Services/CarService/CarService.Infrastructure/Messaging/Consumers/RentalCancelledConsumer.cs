using CarService.Domain.Cars;
using CarService.Domain.Cars.Enums;
using Contracts.RentalEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CarService.Infrastructure.Messaging.Consumers;

public class RentalCancelledConsumer(
    ILogger<RentalCancelledConsumer> logger,
    ICarRepository carRepository)
    : IConsumer<RentalCancelledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RentalCancelledIntegrationEvent> context)
    {
        var msg = context.Message;

        var car = await carRepository.GetCarByIdAsync(msg.CarId);
        if (car is null)
        {
            logger.LogWarning("Car {CarId} not found for rental cancellation", msg.CarId);
            return;
        }

        if (car.Status != AvailabilityStatus.Reserved)
        {
            logger.LogInformation(
                "Car {CarId} is not reserved (status={Status}), skipping release for rental {RentalId}",
                msg.CarId, car.Status.Name, msg.RentalId);
            return;
        }

        car.ReleaseReservation();
        await carRepository.UpdateAsync(car);

        logger.LogInformation(
            "Car {CarId} reservation released due to cancellation of rental {RentalId}",
            msg.CarId, msg.RentalId);
    }
}
