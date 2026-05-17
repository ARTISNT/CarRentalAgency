namespace RentalService.Domain.Rentals.PricingPolicies;

public class PricingPolicies
{
    public FinePolicy FinePolicy { get; set; }
    public DiscountPolicy DiscountPolicy { get; set; }
    public BasePricingPolicy  BasePricingPolicy { get; set; }
    public DepositPolicy DepositPolicy { get; set; }
}