using Application.Common;
using Application.DTOs;
using Application.Extensions;
using Domain.Entities.Vehicles;
using Domain.Repositories;

namespace Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepo;
    private readonly IAuctionRepository _auctionRepository;
    public VehicleService(IVehicleRepository vehicleRepo, IAuctionRepository auctionRepository)
    {
        _vehicleRepo = vehicleRepo;
        _auctionRepository = auctionRepository;
    }
    public async Task<Result> AddVehicle(Vehicle vehicle)
    {
        if (await _vehicleRepo.Exists(vehicle.Id))
        {
            return Result.Failure(Problem.DuplicateVehicle(vehicle.Id));
        }
        await _vehicleRepo.Add(vehicle);
        
        return Result.Success();
    }

    public async Task<IEnumerable<VehicleDto>> SearchVehicles(string? type = null, string? manufacturer = null, string? model = null, int? year = null)
    {
        var vehicleDtos =  (await _vehicleRepo.Search(type, manufacturer, model, year))
            .Select(MapToDto);
        var activeVehicles = new List<VehicleDto>();
        
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
    
    private static VehicleDto MapToDto(Vehicle v)
    {
        return v switch
        {
            Hatchback h => new VehicleDto
            {
                Id = h.Id.Id, Type = "Hatchback", Manufacturer = h.Manufacturer, Model = h.Model, Year = h.Year, StartingBid = h.StartingBid.ToDto(),
                NumberOfDoors = h.NumberOfDoors
            },
            Sedan s => new VehicleDto
            {
                Id = s.Id.Id, Type = "Sedan", Manufacturer = s.Manufacturer, Model = s.Model, Year = s.Year, StartingBid = s.StartingBid.ToDto(),
                NumberOfDoors = s.NumberOfDoors
            },
            SUV suv => new VehicleDto
            {
                Id = suv.Id.Id, Type = "SUV", Manufacturer = suv.Manufacturer, Model = suv.Model, Year = suv.Year, StartingBid = suv.StartingBid.ToDto(),
                NumberOfSeats = suv.NumberOfSeats
            },
            Truck t => new VehicleDto
            {
                Id = t.Id.Id, Type = "Truck", Manufacturer = t.Manufacturer, Model = t.Model, Year = t.Year, StartingBid = t.StartingBid.ToDto(),
                LoadCapacity = t.LoadCapacity
            },
            _ => throw new NotImplementedException()
        };
    }
    
}