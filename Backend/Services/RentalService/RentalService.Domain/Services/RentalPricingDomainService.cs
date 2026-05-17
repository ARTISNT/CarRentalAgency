using RentalService.Domain.Rentals;
using RentalService.Domain.Rentals.PricingPolicies;

namespace RentalService.Domain.Services;

public class RentalPricingDomainService
{
    public decimal CalculateBaseCost(
        BasePricingPolicy pricingPolicy,
        Rental rental)
    {
        return pricingPolicy.CalculateBasePrice(
            rental.StartDate,
            rental.EndDate,
            rental.RentCarSnapshot.PricePerHour);
    } 

    public decimal CalculateFine(
        FinePolicy finePolicy,
        Rental rental)
    {
        if (!rental.ReturnDate.HasValue)
            return 0;

        return finePolicy.CalculateFine(
            rental.EndDate,
            rental.ReturnDate,
            rental.RentCarSnapshot.PricePerHour);
    } 
    
    public decimal CalculateTotal(
        PricingPolicies pricingPolicies,
        Rental rental,
        string? promoCode = null)
    {
        var totalHours = pricingPolicies.BasePricingPolicy.GetTotalHours(
            rental.StartDate,
            rental.EndDate);
        
        var baseCost = CalculateBaseCost(pricingPolicies.BasePricingPolicy, rental);
    
        var discount = pricingPolicies.DiscountPolicy.CalculateDiscount(
            baseCost,
            totalHours,
            promoCode);

        var fine = CalculateFine(pricingPolicies.FinePolicy, rental);

        return baseCost - discount + fine;
    }
}