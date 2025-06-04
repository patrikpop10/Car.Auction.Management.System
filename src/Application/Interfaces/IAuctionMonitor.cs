using Domain.Entities.Auction;

namespace Application.Interfaces;

public interface IAuctionMonitor
{
    IAsyncEnumerable<Auction> GetActiveAuctionsAsync(CancellationToken cancellationToken);
}