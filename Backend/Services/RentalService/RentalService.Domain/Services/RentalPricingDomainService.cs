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
        return new Money(
            pricingPolicy.CalculateBasePrice(
                rental.StartDate,
                rental.EndDate,
                rental.RentCarSnapshot.PricePerHour),
            currency);
    }

    public Money CalculateEstimatedCost(
        PricingPolicies pricingPolicies,
        Rental rental,
        string currency)
    {
        var totalHours =
            pricingPolicies.BasePricingPolicy.GetTotalHours(
                rental.StartDate,
                rental.EndDate);

        var baseCost =
            CalculateBaseCost(
                pricingPolicies.BasePricingPolicy,
                rental,
                currency);

        var discount =
            pricingPolicies.DiscountPolicy.CalculateDiscount(
                baseCost.Amount,
                totalHours,
                rental.PromoCode);

        return new Money(
            baseCost.Amount - discount,
            currency);
    }

    public Money CalculateActualCost(
        PricingPolicies pricingPolicies,
        Rental rental,
        DateTime actualReturnDate,
        string currency)
    {
        var actualBaseCost =
            pricingPolicies.BasePricingPolicy.CalculateBasePrice(
                rental.StartDate,
                actualReturnDate,
                rental.RentCarSnapshot.PricePerHour);

        var actualHours =
            pricingPolicies.BasePricingPolicy.GetTotalHours(
                rental.StartDate,
                actualReturnDate);

        var discount =
            pricingPolicies.DiscountPolicy.CalculateDiscount(
                actualBaseCost,
                actualHours,
                rental.PromoCode);

        return new Money(
            actualBaseCost - discount,
            currency);
    }

    public Money CalculateFine(
        PricingPolicies pricingPolicies,
        Rental rental,
        DateTime actualReturnDate,
        string currency)
    {
        return new Money(
            pricingPolicies.FinePolicy.CalculateFine(
                rental.EndDate,
                actualReturnDate,
                rental.RentCarSnapshot.PricePerHour),
            currency);
    }

    public Money CalculateFinalCost(
        PricingPolicies pricingPolicies,
        Rental rental,
        DateTime actualReturnDate,
        string currency)
    {
        var actualCost =
            CalculateActualCost(
                pricingPolicies,
                rental,
                actualReturnDate,
                currency);

        var fine =
            CalculateFine(
                pricingPolicies,
                rental,
                actualReturnDate,
                currency);

        return actualCost + fine;
    }
}
