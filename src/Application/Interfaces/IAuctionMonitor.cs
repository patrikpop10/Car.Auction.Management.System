using Application.Models.Responses;

namespace Application.Interfaces;

public interface IAuctionMonitor {
    IAsyncEnumerable<AuctionMonitoringResponse> GetActiveAuctionsAsync(CancellationToken cancellationToken);
}