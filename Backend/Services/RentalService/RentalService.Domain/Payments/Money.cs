namespace RentalService.Domain.Payments;

public sealed record Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency) =>
        new(0, currency);

    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);

        return new Money(
            left.Amount + right.Amount,
            left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);

        return new Money(
            left.Amount - right.Amount,
            left.Currency);
    }

    private static void EnsureSameCurrency(
        Money left,
        Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException(
                "Currency mismatch");
    }
}
