using Domain.Entities.Vehicles;

namespace Application.DTOs;

public record BidDto
{
    public required VehicleId Id { get; init; }
    public required string Bidder { get; init; }
    public required MoneyDto Amount { get; init; }
};