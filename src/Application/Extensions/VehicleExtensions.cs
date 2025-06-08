using Application.Models.Dtos;
using Application.Models.Requests;
using Application.Models.Responses;
using Domain.Entities.Vehicles;

namespace Application.Extensions;

public static class VehicleExtensions {

    public static VehicleDto ToDto(this Vehicle vehicle) => new() {
        Id = vehicle.Id.Id,
        Manufacturer = vehicle.Manufacturer,
        Model = vehicle.Model,
        Year = vehicle.Year,
        Type = vehicle.Model
    };

    public static Vehicle RequestToDomain(this VehicleRequest vehicleRequest) => vehicleRequest.Vehicle.Type switch {
        nameof(Hatchback) => new Hatchback(new VehicleId(vehicleRequest.Vehicle.Id), vehicleRequest.Vehicle.Manufacturer, vehicleRequest.Vehicle.Model, vehicleRequest.Vehicle.Year, vehicleRequest.StartingBid.ToDomain(), vehicleRequest.Vehicle.NumberOfDoors ?? 5),
        nameof(Sedan) => new Sedan(new VehicleId(vehicleRequest.Vehicle.Id), vehicleRequest.Vehicle.Manufacturer, vehicleRequest.Vehicle.Model, vehicleRequest.Vehicle.Year, vehicleRequest.StartingBid.ToDomain(), vehicleRequest.Vehicle.NumberOfDoors ?? 4),
        nameof(SUV) => new SUV(new VehicleId(vehicleRequest.Vehicle.Id), vehicleRequest.Vehicle.Manufacturer, vehicleRequest.Vehicle.Model, vehicleRequest.Vehicle.Year, vehicleRequest.StartingBid.ToDomain(), vehicleRequest.Vehicle.NumberOfSeats ?? 5),
        nameof(Truck) => new Truck(new VehicleId(vehicleRequest.Vehicle.Id), vehicleRequest.Vehicle.Manufacturer, vehicleRequest.Vehicle.Model, vehicleRequest.Vehicle.Year, vehicleRequest.StartingBid.ToDomain(), vehicleRequest.Vehicle.LoadCapacity ?? 1.0),
        _ => throw new ArgumentException($"Unknown vehicle type: {vehicleRequest.Vehicle.Type}")
    };

    public static SearchVehicleResponse DomainToResponse(this Vehicle vehicle) => vehicle switch {
        Hatchback h => new SearchVehicleResponse {
            Vehicle = new VehicleDto { Id = vehicle.Id.Id, Type = nameof(Hatchback), Manufacturer = h.Manufacturer, Model = h.Model, Year = h.Year, NumberOfDoors = h.NumberOfDoors },
            StartingBid = h.StartingBid.ToDto(),
        },
        Sedan s => new SearchVehicleResponse {
            Vehicle = new VehicleDto { Id = vehicle.Id.Id, Type = nameof(Sedan), Manufacturer = s.Manufacturer, Model = s.Model, Year = s.Year, NumberOfDoors = s.NumberOfDoors },
            StartingBid = s.StartingBid.ToDto(),
        },
        SUV suv => new SearchVehicleResponse {
            Vehicle = new VehicleDto { Id = vehicle.Id.Id, Type = nameof(SUV), Manufacturer = suv.Manufacturer, Model = suv.Model, Year = suv.Year, NumberOfSeats = suv.NumberOfSeats },
            StartingBid = suv.StartingBid.ToDto(),
        },
        Truck t => new SearchVehicleResponse {
            Vehicle = new VehicleDto { Id = vehicle.Id.Id, Type = nameof(Truck), Manufacturer = t.Manufacturer, Model = t.Model, Year = t.Year, LoadCapacity = t.LoadCapacity },
            StartingBid = t.StartingBid.ToDto(),
        },
        _ => throw new ArgumentException("Unknown vehicle type")
    };
}