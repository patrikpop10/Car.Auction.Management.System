using Domain.Entities;

namespace Application.Services;

public interface IAuctionMonitor
{
    IAsyncEnumerable<Auction> GetActiveAuctionsAsync(CancellationToken cancellationToken);
}