using RentalService.Domain.Rentals.PricingPolicies;

namespace RentalService.Application.Common;

public interface IPricingPoliciesFactory
{
    public PricingPolicies Create();
}