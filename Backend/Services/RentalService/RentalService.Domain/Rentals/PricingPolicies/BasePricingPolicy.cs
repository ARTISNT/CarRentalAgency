namespace RentalService.Domain.Rentals.PricingPolicies;

public class BasePricingPolicy
{
    public decimal CalculateBasePrice(DateTime startDate, DateTime endDate, decimal pricePerHour)
    {
        if (endDate <= startDate)
            return 0m;

        var hours = Math.Max(1, Math.Ceiling((endDate - startDate).TotalHours));
        return (decimal)hours * pricePerHour;
    }

    public decimal GetTotalHours(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
            return 0m;

        return (decimal)Math.Max(1, Math.Ceiling((endDate - startDate).TotalHours));
    }
}
