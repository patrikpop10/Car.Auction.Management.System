using Application.Models.Dtos;
using Application.Models.Requests;
using Domain.Entities.Vehicles;

namespace Application.Extensions;

public static class VehicleExtensions {

    public static VehicleDto ToDto(this Vehicle vehicle) => new() {
        Manufacturer = vehicle.Manufacturer,
        Model = vehicle.Model,
        Year = vehicle.Year,
        Type = vehicle.Model
    };

    public static Vehicle RequestToDomain(this VehicleRequest vehicle) => vehicle.Vehicle.Type switch {
        "Hatchback" => new Hatchback(new VehicleId(vehicle.Id), vehicle.Vehicle.Manufacturer, vehicle.Vehicle.Model, vehicle.Vehicle.Year, vehicle.StartingBid.ToDomain(), vehicle.Vehicle.NumberOfDoors ?? 5),
        "Sedan" => new Sedan(new VehicleId(vehicle.Id), vehicle.Vehicle.Manufacturer, vehicle.Vehicle.Model, vehicle.Vehicle.Year, vehicle.StartingBid.ToDomain(), vehicle.Vehicle.NumberOfDoors ?? 4),
        "SUV" => new SUV(new VehicleId(vehicle.Id), vehicle.Vehicle.Manufacturer, vehicle.Vehicle.Model, vehicle.Vehicle.Year, vehicle.StartingBid.ToDomain(), vehicle.Vehicle.NumberOfSeats ?? 5),
        "Truck" => new Truck(new VehicleId(vehicle.Id), vehicle.Vehicle.Manufacturer, vehicle.Vehicle.Model, vehicle.Vehicle.Year, vehicle.StartingBid.ToDomain(), vehicle.Vehicle.LoadCapacity ?? 1.0),
        _ => throw new ArgumentException("Unknown vehicle type")
    };
}