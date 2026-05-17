using RentalService.Domain.Payments;

namespace RentalService.Domain.Rentals.PricingPolicies;

public class DepositPolicy
{
    private readonly decimal _percent;

    public DepositPolicy(decimal percent)
    {
        _percent = percent;
    }

    public Money CalculateDeposit(Money estimatedTotal)
    {
        return new Money(
            estimatedTotal.Amount * _percent,
            estimatedTotal.Currency);
    } 
}