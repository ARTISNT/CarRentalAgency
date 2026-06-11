using CarService.Domain.Cars.Enums;
using CarService.Domain.Cars.ValueObjects;
using CarService.Domain.Common;
using CarService.Domain.DomainEvents;

namespace CarService.Domain.Cars;
public sealed class Car : Entity, IAggregateRoot
{
    public DateTime ReleaseDate { get; private set; }
    public AvailabilityStatus Status { get; private set; }
    public Guid? CurrentRenterId { get; private set; }
    public LicensePlate LicensePlate { get; private set; }
    public VinCode VinCode { get; private set; }
    public Color Color { get; private set; }
    public CarTechInfo TechInfo { get; private set; }
    public CarModelInfo ModelInfo { get; private set; }
    public PricePerHour PricePerHour { get; private set; }
    public CarClass Class { get; private set; }
    public string PhotoUrl { get; private set; }

    private readonly CarAvailabilityStateMachine _carAvailabilityStateMachine = new();

    private Car() { }

    private Car(
        DateTime releaseDate,
        LicensePlate licensePlate,
        VinCode vinCode,
        Color color,
        CarModelInfo modelInfo,
        CarTechInfo techInfo,
        PricePerHour pricePerHour,
        CarClass carClass,
        string photoUrl)
    {
        Id = Guid.NewGuid();
        SetReleaseDate(releaseDate);

        LicensePlate = licensePlate ?? throw new ArgumentNullException(nameof(licensePlate));
        VinCode = vinCode ?? throw new ArgumentNullException(nameof(vinCode));
        Color = color ?? throw new ArgumentNullException(nameof(color));
        ModelInfo = modelInfo ?? throw new ArgumentNullException(nameof(modelInfo));
        TechInfo = techInfo ?? throw new ArgumentNullException(nameof(techInfo));
        PricePerHour = pricePerHour ?? throw new ArgumentNullException(nameof(pricePerHour));
        Class = carClass ?? throw new ArgumentNullException(nameof(carClass));

        SetPhotoUrl(photoUrl);

        Status = AvailabilityStatus.Available;

        AddDomainEvent(new CarCreatedDomainEvent(Id, DateTime.UtcNow));
    }

    public static Car Create(
        DateTime releaseDate,
        LicensePlate licensePlate,
        VinCode vinCode,
        Color color,
        CarModelInfo modelInfo,
        CarTechInfo techInfo,
        PricePerHour pricePerHour,
        CarClass carClass,
        string photoUrl)
    {
        return new Car(
            releaseDate,
            licensePlate,
            vinCode,
            color,
            modelInfo,
            techInfo,
            pricePerHour,
            carClass,
            photoUrl);
    }

    // ========================
    // STATUS TRANSITIONS
    // ========================

    public void Rent(Guid renterId)
    {
        if (Status != AvailabilityStatus.Available && Status != AvailabilityStatus.Reserved)
            throw new InvalidOperationException(
                $"Car must be in 'Available' or 'Reserved' status, but current status is '{Status.Name}'");

        ChangeStatus(AvailabilityStatus.Rented);
        CurrentRenterId = renterId;

        AddDomainEvent(new CarRentedDomainEvent(Id, DateTime.UtcNow));
    }

    public void Reserve(Guid renterId)
    {
        EnsureStatusIs(AvailabilityStatus.Available);

        ChangeStatus(AvailabilityStatus.Reserved);
        CurrentRenterId = renterId;

        AddDomainEvent(new CarReservedDomainEvent(Id, DateTime.UtcNow));
    }

    public void ReleaseReservation()
    {
        EnsureStatusIs(AvailabilityStatus.Reserved);

        ChangeStatus(AvailabilityStatus.Available);
        CurrentRenterId = null;

        AddDomainEvent(new CarBecameAvailableDomainEvent(Id, DateTime.UtcNow));
    }

    public void Return()
    {
        EnsureStatusIs(AvailabilityStatus.Rented);

        ChangeStatus(AvailabilityStatus.Available);
        CurrentRenterId = null;

        AddDomainEvent(new CarBecameAvailableDomainEvent(Id, DateTime.UtcNow));
    }

    public void MarkAsReturned()
    {
        EnsureStatusIs(AvailabilityStatus.Rented);

        ChangeStatus(AvailabilityStatus.Returned);
        CurrentRenterId = null;

        AddDomainEvent(new CarReturnedDomainEvent(Id, DateTime.UtcNow));
    }

    public void ProcessReturn(AvailabilityStatus targetStatus)
    {
        EnsureStatusIs(AvailabilityStatus.Returned);

        ChangeStatus(targetStatus);

        if (targetStatus == AvailabilityStatus.Available)
            AddDomainEvent(new CarBecameAvailableDomainEvent(Id, DateTime.UtcNow));
        else if (targetStatus == AvailabilityStatus.Maintenance)
            AddDomainEvent(new CarWasSentToMaintenanceDomainEvent(Id, DateTime.UtcNow));
        else if (targetStatus == AvailabilityStatus.Broken)
            AddDomainEvent(new CarWasBrokenDomainEvent(Id, DateTime.UtcNow));
    }

    public void Break()
    {
        EnsureAvailability();

        ChangeStatus(AvailabilityStatus.Broken);

        AddDomainEvent(new CarWasBrokenDomainEvent(Id, DateTime.UtcNow));
    }

    public void SendToMaintenance()
    {
        EnsureAvailability();

        ChangeStatus(AvailabilityStatus.Maintenance);

        AddDomainEvent(new CarWasSentToMaintenanceDomainEvent(Id, DateTime.UtcNow));
    }

    public void SendToRepair()
    {
        EnsureStatusIs(AvailabilityStatus.Broken);

        ChangeStatus(AvailabilityStatus.Maintenance);

        AddDomainEvent(new CarWasSentToMaintenanceDomainEvent(Id, DateTime.UtcNow));
    }

    public void CompleteMaintenance()
    {
        EnsureStatusIs(AvailabilityStatus.Maintenance);

        ChangeStatus(AvailabilityStatus.Available);

        AddDomainEvent(new CarBecameAvailableDomainEvent(Id, DateTime.UtcNow));
    }

    // ========================
    // BUSINESS OPERATIONS
    // ========================

    public void ChangePrice(PricePerHour newPrice)
    {
        EnsureAvailability();

        PricePerHour = newPrice ?? throw new ArgumentNullException(nameof(newPrice));

        AddDomainEvent(new CarPriceChangedDomainEvent(Id, newPrice, DateTime.UtcNow));
    }

    public void Repaint(Color newColor)
    {
        EnsureAvailability();

        Color = newColor ?? throw new ArgumentNullException(nameof(newColor));

        AddDomainEvent(new CarRepaintedDomainEvent(Id, newColor, DateTime.UtcNow));
    }

    public void UpdateTechInfo(CarTechInfo techInfo)
    {
        EnsureAvailability();

        TechInfo = techInfo ?? throw new ArgumentNullException(nameof(techInfo));

        AddDomainEvent(new CarTechInfoUpdatedDomainEvent(Id, techInfo, DateTime.UtcNow));
    }

    public void UpdateModelInfo(CarModelInfo modelInfo)
    {
        ModelInfo = modelInfo ?? throw new ArgumentNullException(nameof(modelInfo));

        AddDomainEvent(new CarModelInfoUpdatedDomainEvent(Id, modelInfo, DateTime.UtcNow));
    }

    public void ChangeLicensePlate(LicensePlate licensePlate)
    {
        EnsureAvailability();

        LicensePlate = licensePlate ?? throw new ArgumentNullException(nameof(licensePlate));

        AddDomainEvent(new CarLicensePlateUpdatedDomainEvent(Id, licensePlate, DateTime.UtcNow));
    }

    public void ChangePhoto(string photoUrl)
    {
        SetPhotoUrl(photoUrl);
    }

    public void ChangeReleaseDate(DateTime releaseDate)
    {
        EnsureAvailability();

        SetReleaseDate(releaseDate);

        AddDomainEvent(new CarReleaseDateChangedDomainEvent(Id, ReleaseDate, DateTime.UtcNow));
    }

    public void SetCarClass(CarClass carClass)
    {
        Class = carClass;
        AddDomainEvent(new CarClassChangedDomainEvent(Id, carClass, DateTime.UtcNow));
    }

    // ========================
    // INTERNAL LOGIC
    // ========================

    private void ChangeStatus(AvailabilityStatus newStatus)
    {
        if (!_carAvailabilityStateMachine.CanTransition(Status, newStatus))
            throw new InvalidOperationException(
            $"Cannot transition from {Status.Name} to {newStatus.Name}");
    
        Status = newStatus;
    }

    // ========================
    // GUARDS 
    // ========================

    private void EnsureAvailability()
    {
        if (Status != AvailabilityStatus.Available)
            throw new InvalidOperationException("Operation not allowed while car is not available");
    }

    private void EnsureStatusIs(AvailabilityStatus requiredStatus)
    {
        if (Status != requiredStatus)
            throw new InvalidOperationException(
                $"Car must be in '{requiredStatus.Name}' status");
    }

    // ========================
    // VALIDATION
    // ========================

    private void SetReleaseDate(DateTime releaseDate)
    {
        if (releaseDate > DateTime.UtcNow)
            throw new ArgumentException("Release date cannot be in the future");

        ReleaseDate = releaseDate;
    }

    private void SetPhotoUrl(string photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
            throw new ArgumentNullException(nameof(photoUrl));

        if (!Uri.TryCreate(photoUrl, UriKind.Absolute, out _))
            throw new ArgumentException("Invalid photo URL");

        PhotoUrl = photoUrl;
    }
}