using Application.DTOs;
using Application.DTOs.Requests;
using Domain;
using Domain.Common;
using Domain.Entities.Vehicles;

namespace Application.Services;

public interface IVehicleService
{
    Task<Result> AddVehicle(Vehicle vehicle);

    Task<IEnumerable<VehicleRequest>> SearchVehicles(string? type = null, string? manufacturer = null, string? model = null,
        int? year = null);
}