namespace Domain.Entities.Vehicles;

public class SUV(VehicleId id, string manufacturer, string model, int year, Money startingBid, int numberOfSeats)
    : Vehicle(id, manufacturer, model, year, startingBid)
{
    public int NumberOfSeats { get; } = numberOfSeats;
}