using Application.Extensions;
using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities.Vehicles;
using FluentValidation;

namespace Api.Endpoints;

public static class VehiclesEndpoints {
    public static RouteGroupBuilder MapVehiclesEndpoints(this WebApplication application, IValidator<VehicleRequest> validator) {
        var group = application.MapGroup("/vehicles").WithTags("Vehicles");

        group.MapPost("/", async (IVehicleService service, VehicleRequest vehicleRequest) => {
            var validationResult = await validator.ValidateAsync(vehicleRequest);
            if (!validationResult.IsValid) {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var vehicle = vehicleRequest.RequestToDomain();
            var addVehicleResult = await service.AddVehicle(vehicle);
            return addVehicleResult.ToApiResult(Results.Created($"/vehicles/{vehicle.Id}", vehicleRequest));
        });

        group.MapGet("/search", async (IVehicleService service, string? type, string? manufacturer, string? model, int? year) => {
            var results = await service.SearchVehicles(type, manufacturer, model, year);
            return Results.Ok(results);
        });

        return group;
    }
}