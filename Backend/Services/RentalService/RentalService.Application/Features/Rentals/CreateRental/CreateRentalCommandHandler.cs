using Contracts.RentalEvents;
using MediatR;
using RentalService.Application.Authorization;
using RentalService.Application.Common;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Domain.Rentals.PricingPolicies;
using RentalService.Domain.Services;

namespace RentalService.Application.Features.Rentals.CreateRental;

public class CreateRentalCommandHandler(
    IRentalRepository rentalRepository,
    IPaymentRepository paymentRepository,
    IUserExternalService userExternalService, 
    ICarExternalService carExternalService, 
    IPricingPoliciesFactory pricingPoliciesFactory,
    RentalPricingDomainService rentalPricingDomainService,
    IIntegrationEventPublisher publisher,
    IRentalAuthorizationService authorizationService) 
    : IRequestHandler<CreateRentalCommand, Guid>
{
    public async Task<Guid> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        authorizationService.EnsureCanCreateRental(request.UserId);
        
        var userTask = userExternalService.GetUserForRentAsync(request.UserId);
        var carTask = carExternalService.GetCarForRentAsync(request.CarId);
        
        await Task.WhenAll(userTask, carTask);
        
        var user = await userTask;
        var car = await carTask;

        var rentCarSnapshot = new RentCarSnapshot(car.Model, car.Brand, car.Generation, car.Variant, 
            car.IsFacelift, car.LicensePlate, car.AvailabilityStatus, car.PricePerHour, car.CarClass);

        var carRenterSnapshot = new CarRenterSnapshot(user.Name, user.SurName, 
            user.Patronymic, user.PhoneNumber, user.Email);
        var pricingPolicies = pricingPoliciesFactory.Create();
        
        var rental = new Rental(request.UserId,
            request.CarId,
            request.StartDate,
            request.EndDate,
            rentCarSnapshot,
            carRenterSnapshot);
       
        if(request.PromoCode != null)
            rental.ApplyPromoCode(request.PromoCode);

        var baseCostWithDiscount = rentalPricingDomainService.CalculateEstimatedCost(
            pricingPolicies, rental, "BYN");
        
        var deposit = pricingPolicies.DepositPolicy.CalculateDeposit(baseCostWithDiscount);

        var payment = new Payment(rental.Id, baseCostWithDiscount, deposit);
        rental.AttachPayment(payment.Id);

        await rentalRepository.AddRentalAsync(rental);
        await paymentRepository.AddPaymentAsync(payment);

        var integrationEvent = new RentalCreatedIntegrationEvent(
            rental.Id,
            request.UserId,
            request.CarId,
            rental.StartDate,
            rental.EndDate,
            baseCostWithDiscount.Amount);
        await publisher.Publish(integrationEvent, cancellationToken);
        
        return rental.Id;
    }
}