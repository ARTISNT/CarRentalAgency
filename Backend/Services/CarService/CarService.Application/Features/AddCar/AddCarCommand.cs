using CarService.Domain.Cars.Enums;
using Contracts.Common;
using CarService.Domain.Cars.ValueObjects;
using MediatR;

namespace CarService.Application.Features.AddCar;

public record AddCarCommand(
    DateTime ReleaseDate,
    LicensePlate LicensePlate,
    VinCode VinCode,
    Color Color,
    CarModelInfo CarModelInfo,
    CarTechInfo CarTechInfo,
    PricePerHour PricePerHour,
    CarClass CarClass,
    string PhotoUrl) : IRequest, IAuthorizedRequest;