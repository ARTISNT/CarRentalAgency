namespace PaymentService.Infrastructure.Implementations.ExternalServices.BePaid.DTOs.Request
{
    public class Cart
    {
        public List<CartItem> Positions { get; set; } = new();
    }
}
