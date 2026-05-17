using RentalService.Domain.Rentals.PricingPolicies;

namespace RentalService.Infrastructure.Services.PricingPolicyServices;

public class PricingSettings
{
    public DiscountPolicySettings DiscountPolicy { get; set; } = new();
    public FinePolicySettings FinePolicy { get; set; } = new();
    public DepositPolicySettings DepositPolicy { get; set; } = new();
}