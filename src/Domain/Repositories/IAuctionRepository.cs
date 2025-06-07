using Domain.Entities.Auction;
using Domain.Entities.Vehicles;

namespace Domain.Repositories;

public interface IAuctionRepository {
    Task Add(Auction? auction);
    Task<Auction?> GetActiveByVehicleId(VehicleId vehicleId);
    Task<Auction> GetByVehicleId(VehicleId vehicleId);
    Task<bool> CloseAuction(Auction auction);
    Task<bool> IsAuctionActive(VehicleId vehicleId);
}