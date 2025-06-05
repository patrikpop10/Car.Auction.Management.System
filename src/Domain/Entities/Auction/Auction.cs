using Domain.Common;
using Domain.Entities.Vehicles;
using Domain.Events;

namespace Domain.Entities.Auction;

public class Auction
{
    public VehicleId VehicleId { get; }
    public bool IsActive { get; private set; }
    public List<Bid> Bids { get; }
    public DateTime StartedAt { get; }
    public DateTime? ClosedAt { get; private set; }

    public Auction(VehicleId vehicleId)
    {
        VehicleId = vehicleId;
        IsActive = true;
        StartedAt = DateTime.UtcNow;
        Bids = new List<Bid>();
    }

    public Result PlaceBid(Bid bid, Vehicle vehicle)
    {
        if (vehicle.Id != VehicleId)
        {
            return Result.Failure(Problem.VehicleIdMismatch(VehicleId, vehicle.Id));
        }
        if (!IsActive)
        {
            return Result.Failure(Problem.AuctionNotActive(VehicleId));
        }
        
        var isBidValid = IsBidValid(bid, vehicle);
        if (!isBidValid)
        {
            return Result.Failure(Problem.InvalidBidAmount());
        }
        
        Bids.Add(bid);
        return Result.Success();
    }

    public Result<AuctionClosed> Close()
    {
        if (!IsActive)
        {
           return Result<AuctionClosed>.Failure(Problem.AuctionNotActive(VehicleId));
        }
        IsActive = false;
        ClosedAt = DateTime.UtcNow;
        return Result<AuctionClosed>.Success(new AuctionClosed(VehicleId.Id, Bids.LastOrDefault()?.Bidder ?? "", Bids.LastOrDefault()?.Value ?? Money.None()));
    }

    private bool IsBidValid(Bid bid, Vehicle vehicle)
    {
       return bid.Value.Amount > 0 && 
              (Bids.Count == 0 || bid.Value.Amount > CurrentHighestBid.Value.Amount && bid.Value.CurrencyType == CurrentHighestBid.Value.CurrencyType)
           && bid.Value.Amount >= vehicle.StartingBid.Amount && bid.Value.CurrencyType == vehicle.StartingBid.CurrencyType;
    }

    private Bid CurrentHighestBid => Bids.Count > 0 ? Bids[^1] : Bid.Empty;
}