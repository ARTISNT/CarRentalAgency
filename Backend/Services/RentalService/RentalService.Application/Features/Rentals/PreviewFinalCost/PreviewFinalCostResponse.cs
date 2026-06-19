namespace RentalService.Application.Features.Rentals.PreviewFinalCost;

public record PreviewFinalCostResponse(
    decimal FinalCost,
    decimal Estimated,
    decimal Diff,
    string Currency);
