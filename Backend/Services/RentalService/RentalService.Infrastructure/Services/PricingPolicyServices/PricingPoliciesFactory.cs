using RentalService.Application.Common;
using RentalService.Domain.Rentals.PricingPolicies;
using RentalService.Infrastructure.Common;

namespace RentalService.Infrastructure.Services.PricingPolicyServices;

public class PricingPoliciesFactory(IJsonPriceSettingProvider jsonPriceSettingProvider) : IPricingPoliciesFactory
{
    public PricingPolicies Create()
    {
        var settings = jsonPriceSettingProvider.GetSettings();
        
        var pricingPolicies = new PricingPolicies
        {
            FinePolicy = new FinePolicy(settings.FinePolicy.DelinquencyRate),
            DiscountPolicy = new DiscountPolicy(settings.DiscountPolicy.PromoCodes, settings.DiscountPolicy.Rules,
                settings.DiscountPolicy.MaxDiscount),
            BasePricingPolicy = new BasePricingPolicy(),
            DepositPolicy = new DepositPolicy(settings.DepositPolicy.Percentage)
        };
        
        return pricingPolicies;
    }
}