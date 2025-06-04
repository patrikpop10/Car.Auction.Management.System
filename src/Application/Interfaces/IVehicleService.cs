using Application.Common;
using Application.DTOs;
using Domain.Entities.Vehicles;

namespace Application.Services;

public interface IVehicleService
{
    Task<Result> AddVehicle(Vehicle vehicle);

    Task<IEnumerable<VehicleDto>> SearchVehicles(string? type = null, string? manufacturer = null, string? model = null,
        int? year = null);
}