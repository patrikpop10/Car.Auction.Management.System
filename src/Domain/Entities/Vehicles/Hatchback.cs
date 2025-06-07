namespace Domain.Entities.Vehicles;

public class Hatchback(VehicleId id, string manufacturer, string model, int year, Money startingBid, int numberOfDoors)
    : Vehicle(id, manufacturer, model, year, startingBid) {
    public int NumberOfDoors { get; } = numberOfDoors;
}