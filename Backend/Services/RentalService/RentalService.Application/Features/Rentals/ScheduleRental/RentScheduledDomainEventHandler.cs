using Contracts.RentalEvents;
using RentalService.Application.Common;
using RentalService.Domain.DomainEvents;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.ScheduleRental;

public class RentScheduledDomainEventHandler(
    IRentalRepository rentalRepository,
    IIntegrationEventPublisher integrationEventPublisher)
    : IDomainEventHandler<RentScheduledDomainEvent>
{
    public async Task HandleAsync(RentScheduledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(domainEvent.Id, cancellationToken);
        if (rental is null)
            return;

        await integrationEventPublisher.Publish(
            new RentalScheduledIntegrationEvent(
                rental.Id,
                rental.CarRenterId,
                rental.RentCarId,
                DateTime.UtcNow),
            cancellationToken);
    }
}
