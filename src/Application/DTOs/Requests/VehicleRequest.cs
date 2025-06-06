namespace Application.DTOs.Requests;

public record VehicleRequest
{
    public required Guid Id { get; init; }
    public required CarDto Car { get; init; }
    public required MoneyDto StartingBid { get; init; }
}
