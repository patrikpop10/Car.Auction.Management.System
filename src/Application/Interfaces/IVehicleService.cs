using Application.Models.Responses;
using Domain.Common;
using Domain.Entities.Vehicles;

namespace Application.Interfaces;

public interface IVehicleService {
    Task<Result> AddVehicle(Vehicle vehicle);
    Task<IEnumerable<SearchVehicleResponse>> SearchVehicles(string? type = null, string? manufacturer = null, string? model = null, int? year = null);
}