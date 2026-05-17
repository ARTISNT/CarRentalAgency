using MediatR;
using RentalService.Application.Common;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Domain.Services;

namespace RentalService.Application.Features.Rentals.CreateRental;

public class CreateRentalCommandHandler(
    IRentalRepository rentalRepository,
    IPaymentRepository paymentRepository,
    IUserExternalService userExternalService, 
    ICarExternalService carExternalService, 
    IPricingPoliciesFactory pricingPoliciesFactory,
    RentalPricingDomainService rentalPricingDomainService) 
    : IRequestHandler<CreateRentalCommand, Guid>
{
    public async Task<Guid> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        var user = await userExternalService.GetUserForRentAsync(request.UserId);
        var car = await carExternalService.GetCarForRentAsync(request.CarId);

        var rentCarSnapshot = new RentCarSnapshot(car.Model, car.Brand, car.Generation, car.Variant, 
            car.IsFacelift, car.LicensePlate, car.AvailabilityStatus, car.PricePerHour, car.CarClass);

        var carRenterSnapshot = new CarRenterSnapshot(user.Name, user.SurName, 
            user.Patronymic, user.PhoneNumber, user.Email);
        var pricingPolicies = pricingPoliciesFactory.Create();
        
        var rental = new Rental(request.StartDate, request.EndDate, rentCarSnapshot, carRenterSnapshot);

        var baseCost = new Money(rentalPricingDomainService.CalculateBaseCost(
            pricingPolicies.BasePricingPolicy, rental), "BYN");
        var deposit = pricingPolicies.DepositPolicy.CalculateDeposit(baseCost);

        var payment = new Payment(rental.Id, baseCost, deposit);
        Console.WriteLine(payment.Id);
        rental.AttachPayment(payment.Id);
        
        await rentalRepository.AddRentalAsync(rental);
        await paymentRepository.AddPaymentAsync(payment);
        
        return rental.Id;
    }
}