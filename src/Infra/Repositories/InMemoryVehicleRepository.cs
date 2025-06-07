using Domain.Common;
using Domain.Entities.Vehicles;
using Domain.Repositories;

namespace Infra.Repositories;

public class InMemoryVehicleRepository : IVehicleRepository {
    private readonly Dictionary<VehicleId, Vehicle> _vehicles = new();

    public Task<Result> Add(Vehicle vehicle) {
        var result = _vehicles.TryAdd(vehicle.Id, vehicle);
        return Task.FromResult(result ? Result.Success() : Result.Failure(Problem.DuplicateVehicle(vehicle.Id)));
    }

    public Task<Result<Vehicle>> GetById(VehicleId id) {
        var vehicle = _vehicles.GetValueOrDefault(id);
        return Task.FromResult(vehicle is not null
            ? Result<Vehicle>.Success(vehicle)
            : Result<Vehicle>.Failure(Problem.VehicleNotFound(id)));
    }

    public Task<Result<IEnumerable<Vehicle>>> Search(string? type = null, string? manufacturer = null, string? model = null, int? year = null) {
        var result = _vehicles.Values.Where(v =>
            (type == null || v.GetType().Name.Equals(type, StringComparison.OrdinalIgnoreCase)) &&
            (manufacturer == null || v.Manufacturer.Equals(manufacturer, StringComparison.OrdinalIgnoreCase)) &&
            (model == null || v.Model.Equals(model, StringComparison.OrdinalIgnoreCase)) &&
            (year == null || v.Year == year));
        return Task.FromResult(Result<IEnumerable<Vehicle>>.Success(result));
    }

    public Task<Result<bool>> Exists(VehicleId id) {
        var exists = _vehicles.ContainsKey(id);
        return Task.FromResult(exists ? Result<bool>.Success(true) : Result<bool>.Success(false));
    }
    public Task<Result> Remove(VehicleId id) {
        _vehicles.Remove(id);
        return Task.FromResult(Result.Success());
    }
}