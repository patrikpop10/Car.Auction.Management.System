namespace Domain.Entities.Vehicles;

public class Truck(VehicleId id, string manufacturer, string model, int year, Money startingBid, double loadCapacity)
    : Vehicle(id, manufacturer, model, year, startingBid)
{
    public double LoadCapacity { get; } = loadCapacity;
}