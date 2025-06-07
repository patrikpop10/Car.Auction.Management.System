using Domain.Entities;
using Domain.Entities.Vehicles;
using Domain.Repositories;

namespace Infra.Repositories;

public class InMemoryVehicleRepository : IVehicleRepository {
    private readonly Dictionary<VehicleId, Vehicle> _vehicles = new();

    public Task Add(Vehicle vehicle) {
        _vehicles.Add(vehicle.Id, vehicle);
        return Task.CompletedTask;
    }

    public Task<Vehicle?> GetById(VehicleId id) => Task.FromResult(_vehicles.GetValueOrDefault(id));

    public Task<IEnumerable<Vehicle>> Search(string? type = null, string? manufacturer = null, string? model = null, int? year = null) {
        return Task.FromResult(_vehicles.Values.Where(v =>
            (type == null || v.GetType().Name.Equals(type, StringComparison.OrdinalIgnoreCase)) &&
            (manufacturer == null || v.Manufacturer.Equals(manufacturer, StringComparison.OrdinalIgnoreCase)) &&
            (model == null || v.Model.Equals(model, StringComparison.OrdinalIgnoreCase)) &&
            (year == null || v.Year == year)));
    }

    public Task<bool> Exists(VehicleId id) => Task.FromResult(_vehicles.ContainsKey(id));
    public Task Remove(VehicleId id) {
        _vehicles.Remove(id);
        return Task.CompletedTask;
    }
}