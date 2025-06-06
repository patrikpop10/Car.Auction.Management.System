using Application.Models.Dtos;
using Domain.Entities.Vehicles;

namespace Application.Extensions;

public static class VehicleExtensions
{

    public static VehicleDto ToDto(this Vehicle vehicle)
    {
        return new VehicleDto
        {
            Manufacturer = vehicle.Manufacturer,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Type = vehicle.Model
        };
    }
}