using Domain.Entities;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;
using Domain.Repositories;

namespace Infra.Repositories
{
    public class InMemoryAuctionRepository : IAuctionRepository
    {
        private readonly List<Auction?> _auctions = new();

        public Task Add(Auction? auction)
        {
            _auctions.Add(auction);
            return Task.CompletedTask;
        }

        public Task<Auction?> GetActiveByVehicleId(VehicleId vehicleId)
        {
            return Task.FromResult(_auctions.FirstOrDefault(a => a.VehicleId == vehicleId && a.IsActive));
        }

        public Task<Auction> GetByVehicleId(VehicleId vehicleId)
        {
            return Task.FromResult(_auctions.FirstOrDefault(a => a.VehicleId == vehicleId));
        }

        public Task<bool> CloseAuction(Auction auction)
        {
            // simulate closing the auction
            var existingAuction = _auctions.FirstOrDefault(a => a.VehicleId == auction.VehicleId && a.IsActive);
            if (existingAuction is null)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public async Task<bool> IsAuctionActive(VehicleId vehicleId)
        {
            return (await GetActiveByVehicleId(vehicleId)) is not null;
        }
    }
}