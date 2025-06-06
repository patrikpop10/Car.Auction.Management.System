namespace Application.Models.Dtos;

public record VehicleDto
{
    public required string Type { get; init; }
    public required string Manufacturer { get; init; }
    public required string Model { get; init; }
    public required int Year { get; init; }
    public int? NumberOfDoors { get; init; }
    public int? NumberOfSeats { get; init; }
    public double? LoadCapacity { get; init; }
}