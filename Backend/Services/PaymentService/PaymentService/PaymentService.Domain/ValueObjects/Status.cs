using Ardalis.SmartEnum;

namespace PaymentService.Domain.ValueObjects
{
    public class Status : SmartEnum<Status>
    {
        public static readonly Status Pending = new Status(nameof(Pending), 1);
        public static readonly Status Success = new Status(nameof(Success), 2);
        public static readonly Status Failed = new Status(nameof(Failed), 3);

        private Status(string name, int value) : base(name, value) { }
    }
}
