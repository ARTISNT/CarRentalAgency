using RentalService.Infrastructure.Services.PricingPolicyServices;

namespace RentalService.Infrastructure.Common;

public interface IJsonPriceSettingProvider
{
    PricingSettings GetSettings();
}