namespace PaymentService.Application.PaymentMethods
{
    public record PaymentMethodDto(string Name, string SystemName, bool IsActive);
}
