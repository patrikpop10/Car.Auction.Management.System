using Application.Extensions;
using Application.Interfaces;
using Application.Models.Responses;
using Domain.Common;
using Domain.Entities.Vehicles;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class VehicleService : IVehicleService {
    private readonly IVehicleRepository _vehicleRepo;
    private readonly IAuctionRepository _auctionRepository;
    private readonly ILogger<VehicleService> _logger;

    public VehicleService(
        IVehicleRepository vehicleRepo,
        IAuctionRepository auctionRepository,
        ILogger<VehicleService> logger) {
        _vehicleRepo = vehicleRepo;
        _auctionRepository = auctionRepository;
        _logger = logger;
    }
    public async Task<Result> AddVehicle(Vehicle vehicle) {
        if (await _vehicleRepo.Exists(vehicle.Id)) {
            _logger.LogWarning("Attempted to add a vehicle that already exists: {VehicleId}", vehicle.Id);
            return Result.Failure(Problem.DuplicateVehicle(vehicle.Id));
        }
        _logger.LogInformation("Adding vehicle: {VehicleId}, Type: {VehicleType}, Manufacturer: {Manufacturer}, Model: {Model}, Year: {Year}", vehicle.Id, vehicle.GetType().Name, vehicle.Manufacturer, vehicle.Model, vehicle.Year);
        await _vehicleRepo.Add(vehicle);

        return Result.Success();
    }

    public async Task<IEnumerable<SearchVehicleResponse>> SearchVehicles(string? type = null, string? manufacturer = null, string? model = null, int? year = null) {
        var vehicleDtos = (await _vehicleRepo.Search(type, manufacturer, model, year))
            .Select(v => v.DomainToResponse());
        var activeVehicles = new List<SearchVehicleResponse>();

        foreach (var v in vehicleDtos) {
            if (await IsAuctionActive(new VehicleId(v.Id)))
                activeVehicles.Add(v);
        }
        return activeVehicles;
    }

    private async Task<bool> IsAuctionActive(VehicleId vehicleId) {
        var result = await _auctionRepository.IsAuctionForVehicleActive(vehicleId);
        if (!result.IsSuccess) {
            _logger.LogWarning("Error checking auction status for vehicle {VehicleId}: {ErrorMessage}", vehicleId, result.Problem?.ErrorMessage);
            return false;
        }
        return result.Value;
    }
}