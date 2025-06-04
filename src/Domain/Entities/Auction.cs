using Application.Common;
using Domain.Entities.Vehicles;

namespace Domain.Entities;

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

    public Result PlaceBid(Bid bid)
    {
        if (!IsActive)
        {
            return Result.Failure(Problem.AuctionNotActive(VehicleId));
        }
        if (Bids.Count > 0 && bid.Value.Amount <= Bids[^1].Value.Amount) {
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
        return Result<AuctionClosed>.Success(new AuctionClosed(VehicleId, Bids.LastOrDefault()?.Bidder ?? "", Bids.LastOrDefault()?.Value ?? Money.None()));
    }

    public Money CurrentHighestBid => Bids.Count > 0 ? Bids[^1].Value : Money.None();
}
public sealed record AuctionClosed(VehicleId Vehicle, string Winner, Money WinningBid);