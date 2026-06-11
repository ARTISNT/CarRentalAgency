using Ardalis.SmartEnum;

namespace PaymentService.Domain.ValueObjects
{
    public class PaymentType : SmartEnum<PaymentType>
    {
        public static readonly PaymentType Deposit = new PaymentType(nameof(Deposit), 1);
        public static readonly PaymentType FullPayment = new PaymentType(nameof(FullPayment), 2);
        public static readonly PaymentType DepositRefund = new PaymentType(nameof(DepositRefund), 3);
        public static readonly PaymentType Fine = new PaymentType(nameof(Fine), 4);
        public static readonly PaymentType Additional = new PaymentType(nameof(Additional), 5);
        public static readonly PaymentType FineRefund = new PaymentType(nameof(FineRefund), 6);

        private PaymentType(string name, int value) : base(name, value) { }
    }
}
