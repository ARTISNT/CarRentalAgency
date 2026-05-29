using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Domain.Rentals.PricingPolicies;

namespace RentalService.Domain.Services;

public class RentalPricingDomainService
{
    public Money CalculateBaseCost(
        BasePricingPolicy pricingPolicy,
        Rental rental,
        string currency)
    {
        return new Money(pricingPolicy.CalculateBasePrice(
            rental.StartDate,
            rental.EndDate,
            rental.RentCarSnapshot.PricePerHour), currency);
    }

    public Money CalculateBaseCostWithDiscount(
        PricingPolicies pricingPolicies,
        Rental rental,
        string currency,
        string? promoCode = null)
    {
        var totalHours = pricingPolicies.BasePricingPolicy.GetTotalHours(
            rental.StartDate,
            rental.EndDate);
        
        var baseCost = CalculateBaseCost(pricingPolicies.BasePricingPolicy, rental, currency);
    
        var discount = new Money(pricingPolicies.DiscountPolicy.CalculateDiscount(
            baseCost.Amount,
            totalHours,
            promoCode), currency);
        
        return baseCost - discount;
    }

    public Money CalculateFine(
        FinePolicy finePolicy,
        Rental rental,
        string currency)
    {
        if (!rental.ReturnDate.HasValue)
            return Money.Zero(currency);

        return new Money(finePolicy.CalculateFine(
            rental.EndDate,
            rental.ReturnDate,
            rental.RentCarSnapshot.PricePerHour), currency);
    } 
    
    public Money CalculateTotal(
        PricingPolicies pricingPolicies,
        Rental rental,
        Payment rentalPayment,
        string currency)
    {
        var fine = CalculateFine(pricingPolicies.FinePolicy, rental, currency);

        return rentalPayment.EstimatedAmount + fine;
    }
}