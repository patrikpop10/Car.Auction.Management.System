using Application.DTOs;
using Application.Extensions;
using Application.Services;
using Domain.Entities.Vehicles;

namespace Api.Endpoints;

public static class VehiclesEndpoints
{
    public static RouteGroupBuilder MapVehiclesEndpoints(this WebApplication application)
    {
        var group = application.MapGroup("/vehicles").WithTags("Vehicles");
        
        group.MapPost("/", async (IVehicleService service, VehicleDto dto) =>
        {
            Vehicle vehicle = dto.Type switch
            {
                "Hatchback" => new Hatchback(new VehicleId(dto.Id), dto.Manufacturer, dto.Model, dto.Year, dto.StartingBid.ToDomain(), dto.NumberOfDoors ?? 5),
                "Sedan"     => new Sedan(new VehicleId(dto.Id), dto.Manufacturer, dto.Model, dto.Year, dto.StartingBid.ToDomain(), dto.NumberOfDoors ?? 4),
                "SUV"       => new SUV(new VehicleId(dto.Id), dto.Manufacturer, dto.Model, dto.Year, dto.StartingBid.ToDomain(), dto.NumberOfSeats ?? 5),
                "Truck"     => new Truck(new VehicleId(dto.Id), dto.Manufacturer, dto.Model, dto.Year, dto.StartingBid.ToDomain(), dto.LoadCapacity ?? 1.0),
                _           => throw new ArgumentException("Unknown vehicle type")
            };

            var addVehicleResult = await service.AddVehicle(vehicle);
            return addVehicleResult.ToApiResult(Results.Created($"/vehicles/{vehicle.Id}", dto));
        });

        group.MapGet("/search", async (IVehicleService service, string? type, string? manufacturer, string? model, int? year) =>
        {
            var results = await service.SearchVehicles(type, manufacturer, model, year);
            return Results.Ok(results);
        });

        return group;
    }
}