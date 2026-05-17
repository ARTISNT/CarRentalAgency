namespace RentalService.Domain.Rentals.PricingPolicies;

public class DiscountRule
{
    public decimal Discount { get; set; }
    public int Hours { get; set; }

    private DiscountRule() {} 

    public DiscountRule(int hours, decimal discount)
    {
        if (hours < 0) throw new ArgumentException("Hours cannot be negative");
        if (discount < 0 || discount > 1) throw new ArgumentException("Discount must be between 0 and 1");

        Hours = hours;
        Discount = discount;
    }
}
