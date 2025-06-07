using Application.Models.Dtos;

namespace Application.Models.Requests;

public record VehicleRequest {
    public required Guid Id { get; init; }
    public required VehicleDto Vehicle { get; init; }
    public required MoneyDto StartingBid { get; init; }
}