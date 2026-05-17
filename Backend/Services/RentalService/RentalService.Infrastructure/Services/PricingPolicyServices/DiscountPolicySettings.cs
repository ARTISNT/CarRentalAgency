using RentalService.Domain.Rentals.PricingPolicies;

namespace RentalService.Infrastructure.Services.PricingPolicyServices;

public class DiscountPolicySettings
{
    public decimal MaxDiscount { get; set; }

    public List<PromoCode> PromoCodes { get; set; } = new();

    public List<DiscountRule> Rules { get; set; } = new();
}