using CarService.Domain.Cars;
using Contracts.ContractEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CarService.Infrastructure.Messaging.Consumers;

public class ContractEndedConsumer(
    ILogger<ContractEndedConsumer> logger,
    ICarRepository carRepository)
    : IConsumer<ContractEndedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ContractEndedIntegrationEvent> context)
    {
        var msg = context.Message;

        var car = await carRepository.GetCarByIdAsync(msg.CarId);
        if (car is null)
        {
            logger.LogWarning("Car {CarId} not found for return processing", msg.CarId);
            return;
        }

        car.MarkAsReturned();
        await carRepository.UpdateAsync(car);

        logger.LogInformation(
            "Car {CarId} marked as Returned: mileage={Mileage}, fuel={FuelLevel}%, damage={DamageDescription}",
            msg.CarId, msg.Mileage, msg.FuelLevel, msg.DamageDescription);
    }
}
