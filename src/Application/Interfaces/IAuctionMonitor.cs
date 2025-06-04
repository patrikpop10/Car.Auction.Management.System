using Domain.Entities;
using Domain.Entities.Auction;

namespace Application.Services;

public interface IAuctionMonitor
{
    IAsyncEnumerable<Auction> GetActiveAuctionsAsync(CancellationToken cancellationToken);
}