namespace RentalService.Domain.Rentals.PricingPolicies;

public class PromoCode
{
    public string Code { get; set; } = string.Empty;
    public decimal Discount { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Code))
            throw new ArgumentException("Promo code cannot be empty");

        if (Code.Length > 20)
            throw new ArgumentException("Promo code is too long");

        if (Discount <= 0 || Discount > 1)
            throw new ArgumentException("Discount must be between 0 and 1 (e.g., 0.1 = 10%)");
    }
}
