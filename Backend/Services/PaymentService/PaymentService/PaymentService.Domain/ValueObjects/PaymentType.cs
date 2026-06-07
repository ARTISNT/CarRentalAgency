using Ardalis.SmartEnum;

namespace PaymentService.Domain.ValueObjects
{
    public class PaymentType : SmartEnum<PaymentType>
    {
        public static readonly PaymentType Deposit = new PaymentType(nameof(Deposit), 1);
        public static readonly PaymentType FullPayment = new PaymentType(nameof(FullPayment), 2);
        public static readonly PaymentType DepositRefund = new PaymentType(nameof(DepositRefund), 3);

        private PaymentType(string name, int value) : base(name, value) { }
    }
}
