namespace PaymentService.Domain.Entities
{
    public class PaymentMethod
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string SystemName { get; private set; }
        public bool IsActive { get; private set; }

        public PaymentMethod(string name, string systemName, bool isActive)
        {
            Name = name;
            SystemName = systemName;
            IsActive = isActive;
        }
    }
}
