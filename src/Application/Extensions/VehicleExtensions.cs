using Application.DTOs;
using Domain.Entities.Vehicles;

namespace Application.Extensions;

public static class VehicleExtensions
{
    public static VehicleResponse ToResponse(this Vehicle vehicle)
    {
        return new VehicleResponse(
            vehicle.Id.Id,
            vehicle.Manufacturer,
            vehicle.Model,
            vehicle.Year);
    }
    
    public static CarDto ToDto(this Vehicle vehicle)
    {
        return new CarDto
        {
            Manufacturer = vehicle.Manufacturer,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Type = vehicle.Model
        };
    }
}