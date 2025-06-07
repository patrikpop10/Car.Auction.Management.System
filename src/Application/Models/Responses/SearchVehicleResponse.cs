using Application.Models.Dtos;

namespace Application.Models.Responses;


public sealed record SearchVehicleResponse {
    public required Guid Id { get; init; }
    public required VehicleDto Vehicle { get; init; }
    public required MoneyDto StartingBid { get; init; }
}