using RentalService.Domain.Common;
using RentalService.Domain.DomainEvents;
using RentalService.Domain.Rentals.Enums;

namespace RentalService.Domain.Rentals;

public class Rental : Entity, IAggregateRoot
{
    public Guid CarRenterId { get; private set; }
    public Guid RentCarId { get; private set; }
    public Guid? PaymentId { get; private set; } 
    public RentActivityStatus ActivityStatus { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public string? PromoCode { get; private set; }
    public RentCarSnapshot RentCarSnapshot { get; }
    public CarRenterSnapshot CarRenterSnapshot { get; }

    protected Rental() {}

    public Rental(Guid carRenterId, 
        Guid rentCarId,
        DateTime startDate, 
        DateTime endDate,
        RentCarSnapshot rentCarSnapshot,
        CarRenterSnapshot carRenterSnapshot)
    {
        if(carRenterId == Guid.Empty)
            throw new ArgumentException("Car renter id cannot be empty");
        
        if (rentCarId == Guid.Empty)
            throw new ArgumentException("Rental car id cannot be empty");
        
        var duration = endDate - startDate;
        
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be greater than end date");

        if (duration.TotalDays > 31)
            throw new ArgumentException(
                "Rental period cannot exceed 31 days"); 

        StartDate = startDate;
        EndDate = endDate;

        RentCarSnapshot = rentCarSnapshot ?? throw new ArgumentNullException(nameof(rentCarSnapshot));
        CarRenterSnapshot = carRenterSnapshot ?? throw new ArgumentNullException(nameof(carRenterSnapshot));
        
        CarRenterId = carRenterId;
        RentCarId = rentCarId;
        
        ActivityStatus = RentActivityStatus.AwaitingConfirmation;
        AddDomainEvent(new RentCreatedDomainEvent(Id, DateTime.UtcNow));
    }

    public void StartRental()
    {
        if (ActivityStatus != RentActivityStatus.AwaitingConfirmation)
            throw new InvalidOperationException("Cannot start rental");
        
        if(!PaymentId.HasValue)
            throw new InvalidOperationException("Payment didn't attached");

        ActivityStatus = RentActivityStatus.Active;
        AddDomainEvent(new RentStartedDomainEvent(Id, DateTime.UtcNow));
    }
    
    public void EndRental(DateTime returnDate)
    {
        if (ActivityStatus != RentActivityStatus.Active)
            throw new InvalidOperationException("Only active rental can be completed");

        if (returnDate < StartDate)
            throw new ArgumentException("Return date invalid");

        ReturnDate = returnDate;
        ActivityStatus = RentActivityStatus.Completed;
        AddDomainEvent(new RentEndedDomainEvent(Id, DateTime.UtcNow));
    }

    public void RenewRental(DateTime newEndDate)
    {
        if (ActivityStatus != RentActivityStatus.Active)
            throw new InvalidOperationException("Cannot renew rental");

        if (newEndDate <= EndDate)
            throw new ArgumentException("New end date must be greater then end date");

        var totalDuration = newEndDate - StartDate;

        if (totalDuration.TotalDays > 31)
            throw new ArgumentException(
                "Rental period cannot exceed 31 days"); 
        
        EndDate = newEndDate;
        AddDomainEvent(new RentRenewedDomainEvent(Id, DateTime.UtcNow));
    }
    

    public void CancelRental(DateTime cancelledAt)
    {
        if (ActivityStatus != RentActivityStatus.Active && 
            ActivityStatus != RentActivityStatus.AwaitingConfirmation)
        {
            throw new InvalidOperationException("Cannot cancel rental");
        }
        
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
        if (ActivityStatus != RentActivityStatus.AwaitingConfirmation)
            throw new InvalidOperationException("Cannot attach payment");
        
        if (paymentId == Guid.Empty)
            throw new ArgumentException("Payment id invalid");

        if (PaymentId.HasValue)
            throw new InvalidOperationException(
                "Payment already attached");

        PaymentId = paymentId;
    } 
    
    public void ApplyPromoCode(string promoCode)
    {
        if (ActivityStatus != RentActivityStatus.AwaitingConfirmation)
            throw new InvalidOperationException("Promo code cannot be applied");
        
        if (string.IsNullOrWhiteSpace(promoCode))
            throw new ArgumentException();

        if (PromoCode != null)
            throw new InvalidOperationException(
                "Promo code already applied");

        PromoCode = promoCode;
    } 
}