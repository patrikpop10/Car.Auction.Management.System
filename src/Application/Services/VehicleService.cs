using Application.DTOs;
using Application.DTOs.Requests;
using Application.Extensions;
using Domain;
using Domain.Common;
using Domain.Entities.Vehicles;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepo;
    private readonly IAuctionRepository _auctionRepository;
    private readonly ILogger<VehicleService> _logger;

    public VehicleService(
        IVehicleRepository vehicleRepo, 
        IAuctionRepository auctionRepository,
        ILogger<VehicleService> logger)
    {
        _vehicleRepo = vehicleRepo;
        _auctionRepository = auctionRepository;
        _logger = logger;
    }
    public async Task<Result> AddVehicle(Vehicle vehicle)
    {
        if (await _vehicleRepo.Exists(vehicle.Id))
        {
            _logger.LogWarning("Attempted to add a vehicle that already exists: {VehicleId}", vehicle.Id);
            return Result.Failure(Problem.DuplicateVehicle(vehicle.Id));
        }
        _logger.LogInformation("Adding vehicle: {VehicleId}, Type: {VehicleType}, Manufacturer: {Manufacturer}, Model: {Model}, Year: {Year}", vehicle.Id, vehicle.GetType().Name, vehicle.Manufacturer, vehicle.Model, vehicle.Year);
        await _vehicleRepo.Add(vehicle);
        
        return Result.Success();
    }

    public async Task<IEnumerable<VehicleRequest>> SearchVehicles(string? type = null, string? manufacturer = null, string? model = null, int? year = null)
    {
        var vehicleDtos =  (await _vehicleRepo.Search(type, manufacturer, model, year))
            .Select(MapToDto);
        var activeVehicles = new List<VehicleRequest>();
        
        foreach (var v in vehicleDtos)
        {
            if (await IsAuctionActive(new VehicleId(v.Id)))
                activeVehicles.Add(v);
        }
        return activeVehicles;
    }
    
    private async Task<bool> IsAuctionActive(VehicleId vehicleId)
    {
        return await _auctionRepository.IsAuctionActive(vehicleId);
    }
    
    private static VehicleRequest MapToDto(Vehicle v)
    {
        return v switch 
        {
            Hatchback h => new VehicleRequest
            {
                Id = h.Id.Id, Car = new CarDto { Type = "Hatchback", Manufacturer = h.Manufacturer, Model = h.Model, Year = h.Year, NumberOfDoors = h.NumberOfDoors }, StartingBid = h.StartingBid.ToDto(),
            },
            Sedan s => new VehicleRequest
            {
                Id = s.Id.Id, Car = new CarDto {Type = "Sedan", Manufacturer = s.Manufacturer, Model = s.Model, Year = s.Year, NumberOfDoors = s.NumberOfDoors }, StartingBid = s.StartingBid.ToDto(),
            },
            SUV suv => new VehicleRequest
            {
                Id = suv.Id.Id, Car = new CarDto(){Type = "SUV", Manufacturer = suv.Manufacturer, Model = suv.Model, Year = suv.Year, NumberOfSeats = suv.NumberOfSeats }, StartingBid = suv.StartingBid.ToDto(),
            },
            Truck t => new VehicleRequest
            {
                Id = t.Id.Id, Car = new CarDto{Type = "Truck", Manufacturer = t.Manufacturer, Model = t.Model, Year = t.Year, LoadCapacity = t.LoadCapacity }, StartingBid = t.StartingBid.ToDto(),
            },
            _ => throw new NotImplementedException()
        };
    }
    
}