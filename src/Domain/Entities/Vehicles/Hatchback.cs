namespace Domain.Entities.Vehicles;

public class Hatchback : Vehicle
{
    public int NumberOfDoors { get; }
    public Hatchback(VehicleId id, string manufacturer, string model, int year, Money startingBid, int numberOfDoors)
        : base(id, manufacturer, model, year, startingBid)
    {
        NumberOfDoors = numberOfDoors;
    }
}