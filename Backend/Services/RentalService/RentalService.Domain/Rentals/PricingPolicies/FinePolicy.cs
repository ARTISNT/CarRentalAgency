namespace RentalService.Domain.Rentals.PricingPolicies;

public class FinePolicy 
{
    private readonly decimal _delinquencyRate;

    public FinePolicy(decimal delinquencyRate)
    {
        if (delinquencyRate <= 0)
            throw new ArgumentException("Delinquency rate must be greater than 0");

        _delinquencyRate = delinquencyRate;
    }

    public decimal CalculateFine(DateTime endDate, DateTime? returnDate, decimal pricePerHour)
    {
        if (returnDate is null || returnDate <= endDate)
            return 0;

        var delayHours = Math.Max(1, Math.Ceiling((returnDate.Value - endDate).TotalHours));

        return (decimal)delayHours * pricePerHour * _delinquencyRate;
    }
}
