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
                AvailabilityStatus.Broken
            },

            [AvailabilityStatus.Rented] = new()
            {
                AvailabilityStatus.Available
            },

            [AvailabilityStatus.Broken] = new()
            {
                AvailabilityStatus.Maintenance
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
