using System.Text.Json;
using RentalService.Infrastructure.Common;

namespace RentalService.Infrastructure.Services.PricingPolicyServices;

public class JsonPriceSettingProvider : IJsonPriceSettingProvider
{
    public PricingSettings GetSettings()
    {
        var json = File.ReadAllText(@"PricingPolicySettings.json");
         return JsonSerializer.Deserialize<PricingSettings>(json) ??
                      throw new InvalidOperationException("Could not find PricingSettings");
    } 
}