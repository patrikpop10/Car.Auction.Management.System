namespace Application.DTOs;

public record CarDto
{
    public required string Type { get; set; } 
    public required string Manufacturer { get; set; } 
    public required string Model { get; set; } = string.Empty;
    public required int Year { get; set; }
    public int? NumberOfDoors { get; set; }
    public int? NumberOfSeats { get; set; }
    public double? LoadCapacity { get; set; }
}