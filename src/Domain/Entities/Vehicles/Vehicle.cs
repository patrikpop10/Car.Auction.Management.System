namespace Domain.Entities.Vehicles;

public abstract class Vehicle(VehicleId id, string manufacturer, string model, int year, Money startingBid)
{
    public VehicleId Id { get; } = id;
    public string Manufacturer { get; } = manufacturer;
    public string Model { get; } = model;
    public int Year { get; } = year;
    public Money StartingBid { get; } = startingBid;

}
public record struct VehicleId(Guid Id);