namespace RentalService.Domain.Rentals.PricingPolicies;

public class DiscountPolicy 
{
    private readonly Dictionary<string, decimal> _promoCodes;
    private readonly List<DiscountRule> _rules;
    private readonly decimal _maxDiscount;

    public DiscountPolicy(
        List<PromoCode> promoCodes,
        List<DiscountRule> rules,
        decimal maxDiscount = 0.3m)
    {
        if (promoCodes == null) throw new ArgumentNullException(nameof(promoCodes));
        if (rules == null) throw new ArgumentNullException(nameof(rules));

        Console.WriteLine(promoCodes[0].Discount);
        _promoCodes = promoCodes.ToDictionary(p => p.Code, p => p.Discount);
        _rules = rules;
        _maxDiscount = maxDiscount;
    }

    public decimal CalculateDiscount(decimal baseCost, decimal totalHours, string? promoCode)
    {
        decimal discountPercent = 0;

        var rule = _rules
            .Where(r => totalHours > r.Hours)
            .OrderByDescending(r => r.Hours)
            .FirstOrDefault();

        if (rule != null)
            discountPercent += rule.Discount;

        if (!string.IsNullOrWhiteSpace(promoCode) &&
            _promoCodes.TryGetValue(promoCode, out var promoDiscount))
        {
            discountPercent += promoDiscount;
        }

        discountPercent = Math.Min(discountPercent, _maxDiscount);

        return baseCost * discountPercent;
    }
}