using Domain.Common;
using Domain.Entities.Vehicles;
using Domain.Events;

namespace Domain.Entities.Auction;

public class Auction {
    public AuctionId Id { get; }
    public VehicleId VehicleId { get; }
    public bool IsActive { get; private set; }
    public List<Bid> Bids { get; }
    public DateTime StartedAt { get; }
    public DateTime? ClosedAt { get; private set; }

    public Auction(VehicleId vehicleId) {
        Id = AuctionId.New();
        VehicleId = vehicleId;
        IsActive = true;
        Bids = new List<Bid>();
        StartedAt = DateTime.UtcNow;
    }

    public Result PlaceBid(Bid bid, Vehicle vehicle) {
        if (vehicle.Id != VehicleId) {
            return Result.Failure(Problem.VehicleIdMismatch(VehicleId, vehicle.Id));
        }
        if (!IsActive) {
            return Result.Failure(Problem.AuctionForVehicleNotActive(VehicleId));
        }

        var isBidValid = IsBidValid(bid, vehicle);
        if (!isBidValid) {
            return Result.Failure(Problem.InvalidBidAmount());
        }

        Bids.Add(bid);
        return Result.Success();
    }

    public Result<AuctionClosed> Close() {
        if (!IsActive) {
            return Result<AuctionClosed>.Failure(Problem.AuctionForVehicleNotActive(VehicleId));
        }
        IsActive = false;
        ClosedAt = DateTime.UtcNow;
        return Result<AuctionClosed>.Success(new AuctionClosed(Id.Id, VehicleId.Id, Bids.LastOrDefault()?.Bidder ?? "", Bids.LastOrDefault()?.Value ?? Money.None()));
    }

    private bool IsBidValid(Bid bid, Vehicle vehicle) => bid.Value.Amount > 0
                                                      && (Bids.Count == 0 || bid.Value.Amount > CurrentHighestBid.Value.Amount
                                                          && bid.Value.IsTheSameCurrency(CurrentHighestBid.Value))
                                                      && bid.Value.Amount >= vehicle.StartingBid.Amount
                                                      && bid.Value.IsTheSameCurrency(vehicle.StartingBid);

    private Bid CurrentHighestBid => Bids.Count > 0 ? Bids[^1] : Bid.Empty;

}

public readonly record struct AuctionId(Guid Id) {
    public static AuctionId New() => new(Guid.NewGuid());
};