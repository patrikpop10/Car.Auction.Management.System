using Application.DTOs;
using Domain.Entities.Vehicles;

namespace Application.Extensions;

public static class VehicleExtensions
{
    public static VehicleResponse ToDto(this Vehicle vehicle)
    {
        return new VehicleResponse(
            vehicle.Id.Id,
            vehicle.Manufacturer,
            vehicle.Model,
            vehicle.Year);
    }
}