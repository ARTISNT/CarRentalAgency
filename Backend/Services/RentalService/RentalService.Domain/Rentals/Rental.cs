using RentalService.Domain.Common;
using RentalService.Domain.DomainEvents;
using RentalService.Domain.Rentals.Enums;

namespace RentalService.Domain.Rentals;

public class Rental : Entity, IAggregateRoot
{
    public Guid? PaymentId { get; private set; } 
    public RentActivityStatus ActivityStatus { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }

    public RentCarSnapshot RentCarSnapshot { get; }
    public CarRenterSnapshot CarRenterSnapshot { get; }

    protected Rental() {}

    public Rental(DateTime startDate, DateTime endDate,
        RentCarSnapshot rentCarSnapshot,
        CarRenterSnapshot carRenterSnapshot)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be greater than end date");

        StartDate = startDate;
        EndDate = endDate;

        RentCarSnapshot = rentCarSnapshot ?? throw new ArgumentNullException(nameof(rentCarSnapshot));
        CarRenterSnapshot = carRenterSnapshot ?? throw new ArgumentNullException(nameof(carRenterSnapshot));
        
        ActivityStatus = RentActivityStatus.Active;
        AddDomainEvent(new RentStartedDomainEvent(Id, DateTime.UtcNow));
    }

    public void EndRental(DateTime returnDate)
    {
        if (ActivityStatus == RentActivityStatus.Completed)
            throw new InvalidOperationException("Rental already completed");

        if (returnDate < StartDate)
            throw new ArgumentException("Return date invalid");

        ReturnDate = returnDate;
        ActivityStatus = RentActivityStatus.Completed;
        AddDomainEvent(new RentEndedDomainEvent(Id, DateTime.UtcNow));
    }

    public void RenewRental(DateTime newEndDate)
    {
        if (ActivityStatus == RentActivityStatus.Completed)
            throw new InvalidOperationException("Cannot renew completed rental");

        if (newEndDate <= EndDate)
            throw new ArgumentException("New end date must be greater then end date");

        EndDate = newEndDate;
        AddDomainEvent(new RentRenewedDomainEvent(Id, DateTime.UtcNow));
    }
    

    public void CancelRental(DateTime cancelledAt)
    {
        if (ActivityStatus == RentActivityStatus.Cancelled)
            throw new InvalidOperationException("Rental already cancelled");

        if (ActivityStatus == RentActivityStatus.Completed)
            throw new InvalidOperationException("Completed rental cannot be cancelled");

        if (DateTime.UtcNow >= StartDate)
            throw new InvalidOperationException(
                "Cannot cancel rental after it has started");

        if (cancelledAt > StartDate)
            throw new ArgumentException(
                "Cancellation date cannot be greater than rental start date");

        ActivityStatus = RentActivityStatus.Cancelled;

        AddDomainEvent(
            new RentCancelledDomainEvent(Id, cancelledAt, DateTime.UtcNow));
    }    
    
    public void AttachPayment(Guid paymentId)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentException("Payment id invalid");

        if (PaymentId.HasValue)
            throw new InvalidOperationException(
                "Payment already attached");

        PaymentId = paymentId;
    } 
}