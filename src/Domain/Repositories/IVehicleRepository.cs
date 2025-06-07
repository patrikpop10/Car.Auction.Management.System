using Domain.Common;
using Domain.Entities.Vehicles;

namespace Domain.Repositories {
    public interface IVehicleRepository {
        Task<Result> Add(Vehicle vehicle);
        Task<Result<Vehicle>> GetById(VehicleId id);
        Task<Result<IEnumerable<Vehicle>>> Search(string? type = null, string? manufacturer = null, string? model = null, int? year = null);
        Task<Result<bool>> Exists(VehicleId id);
        Task<Result> Remove(VehicleId id);
    }
}