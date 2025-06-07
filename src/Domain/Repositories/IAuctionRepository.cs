using Domain.Common;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;

namespace Domain.Repositories;

public interface IAuctionRepository {
    Task<Result> Add(Auction auction);
    Task<Result<Auction>> GetActiveByVehicleId(VehicleId vehicleId);
    Task<Result<bool>> IsAuctionForVehicleActive(VehicleId auctionId);
}