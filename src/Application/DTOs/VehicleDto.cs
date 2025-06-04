namespace Application.DTOs;

public class VehicleDto
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public MoneyDto StartingBid { get; set; }
    public int? NumberOfDoors { get; set; }
    public int? NumberOfSeats { get; set; }
    public double? LoadCapacity { get; set; }
}
