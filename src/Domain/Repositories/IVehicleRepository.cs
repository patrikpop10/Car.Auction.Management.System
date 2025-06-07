using Domain.Entities.Vehicles;

namespace Domain.Repositories {
    public interface IVehicleRepository {
        Task Add(Vehicle vehicle);
        Task<Vehicle?> GetById(VehicleId id);
        Task<IEnumerable<Vehicle>> Search(string? type = null, string? manufacturer = null, string? model = null, int? year = null);
        Task<bool> Exists(VehicleId id);
        Task Remove(VehicleId id);
    }
}