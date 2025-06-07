using Domain.Common;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;
using Domain.Repositories;

namespace Infra.Repositories {
    public class InMemoryAuctionRepository : IAuctionRepository {
        private readonly List<Auction> _auctions = [];

        public Task Add(Auction auction) {
            _auctions.Add(auction);
            return Task.CompletedTask;
        }
        public Task<Auction?> GetActiveByAuctionId(AuctionId auctionId) => Task.FromResult(_auctions.FirstOrDefault(a => a.Id == auctionId && a.IsActive));

        public Task<Result<Auction>> GetActiveByVehicleId(VehicleId vehicleId) {
            var auction = _auctions.FirstOrDefault(a => a.VehicleId == vehicleId && a.IsActive);
            return Task.FromResult(auction is null ? Result<Auction>.Failure(Problem.AuctionForVehicleNotActive(vehicleId)) : Result<Auction>.Success(auction));
        }

        public Task<Auction> GetByVehicleId(VehicleId vehicleId) => Task.FromResult(_auctions.FirstOrDefault(a => a.VehicleId == vehicleId));

        public Task<bool> CloseAuction(Auction auction) {
            // simulate closing the auction
            var existingAuction = _auctions.FirstOrDefault(a => a.VehicleId == auction.VehicleId && a.IsActive);
            return Task.FromResult(existingAuction is not null);
        }

        public async Task<Result<bool>> IsAuctionForVehicleActive(VehicleId vehicleId) {
            var result = await GetActiveByVehicleId(vehicleId);
            return Result<bool>.Success(result.IsSuccess && (result.Value?.IsActive ?? false));
        }
    }
}