using CarService.Domain.Cars.Enums;

namespace CarService.Domain.Cars;

public sealed class CarAvailabilityStateMachine
{
    private static readonly Dictionary<AvailabilityStatus, HashSet<AvailabilityStatus>> _transitions
        = new()
        {
            [AvailabilityStatus.Available] = new()
            {
                AvailabilityStatus.Rented,
                AvailabilityStatus.Maintenance,
                AvailabilityStatus.Broken,
                AvailabilityStatus.Reserved
            },

            [AvailabilityStatus.Reserved] = new()
            {
                AvailabilityStatus.Available,
                AvailabilityStatus.Rented
            },

            [AvailabilityStatus.Rented] = new()
            {
                AvailabilityStatus.Available,
                AvailabilityStatus.Returned
            },

            [AvailabilityStatus.Broken] = new()
            {
                AvailabilityStatus.Maintenance
            },

            [AvailabilityStatus.Returned] = new()
            {
                AvailabilityStatus.Available,
                AvailabilityStatus.Maintenance,
                AvailabilityStatus.Broken
            },

            [AvailabilityStatus.Maintenance] = new()
            {
                AvailabilityStatus.Available
            }
        };

    public bool CanTransition(AvailabilityStatus from, AvailabilityStatus to)
    {
        if (!_transitions.TryGetValue(from, out var allowed))
            return false;

        return allowed.Contains(to);
    }
}
