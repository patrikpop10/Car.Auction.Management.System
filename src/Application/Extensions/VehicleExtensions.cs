using Application.Models.Dtos;
using Application.Models.Requests;
using Application.Models.Responses;
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
        nameof(Hatchback) => new Hatchback(new VehicleId(vehicle.Id), vehicle.Vehicle.Manufacturer, vehicle.Vehicle.Model, vehicle.Vehicle.Year, vehicle.StartingBid.ToDomain(), vehicle.Vehicle.NumberOfDoors ?? 5),
        nameof(Sedan) => new Sedan(new VehicleId(vehicle.Id), vehicle.Vehicle.Manufacturer, vehicle.Vehicle.Model, vehicle.Vehicle.Year, vehicle.StartingBid.ToDomain(), vehicle.Vehicle.NumberOfDoors ?? 4),
        nameof(SUV) => new SUV(new VehicleId(vehicle.Id), vehicle.Vehicle.Manufacturer, vehicle.Vehicle.Model, vehicle.Vehicle.Year, vehicle.StartingBid.ToDomain(), vehicle.Vehicle.NumberOfSeats ?? 5),
        nameof(Truck) => new Truck(new VehicleId(vehicle.Id), vehicle.Vehicle.Manufacturer, vehicle.Vehicle.Model, vehicle.Vehicle.Year, vehicle.StartingBid.ToDomain(), vehicle.Vehicle.LoadCapacity ?? 1.0),
        _ => throw new ArgumentException("Unknown vehicle type")
    };

    public static SearchVehicleResponse DomainToResponse(this Vehicle v) => v switch {
        Hatchback h => new SearchVehicleResponse {
            Id = h.Id.Id,
            Vehicle = new VehicleDto { Type = nameof(Hatchback), Manufacturer = h.Manufacturer, Model = h.Model, Year = h.Year, NumberOfDoors = h.NumberOfDoors },
            StartingBid = h.StartingBid.ToDto(),
        },
        Sedan s => new SearchVehicleResponse {
            Id = s.Id.Id,
            Vehicle = new VehicleDto { Type = nameof(Sedan), Manufacturer = s.Manufacturer, Model = s.Model, Year = s.Year, NumberOfDoors = s.NumberOfDoors },
            StartingBid = s.StartingBid.ToDto(),
        },
        SUV suv => new SearchVehicleResponse {
            Id = suv.Id.Id,
            Vehicle = new VehicleDto() { Type = nameof(SUV), Manufacturer = suv.Manufacturer, Model = suv.Model, Year = suv.Year, NumberOfSeats = suv.NumberOfSeats },
            StartingBid = suv.StartingBid.ToDto(),
        },
        Truck t => new SearchVehicleResponse {
            Id = t.Id.Id,
            Vehicle = new VehicleDto { Type = nameof(Truck), Manufacturer = t.Manufacturer, Model = t.Model, Year = t.Year, LoadCapacity = t.LoadCapacity },
            StartingBid = t.StartingBid.ToDto(),
        },
        _ => throw new NotImplementedException()
    };
}