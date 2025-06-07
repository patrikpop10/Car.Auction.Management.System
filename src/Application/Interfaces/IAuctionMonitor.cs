using Application.Models.Responses;

namespace Application.Interfaces;

public interface IAuctionMonitor {
    IAsyncEnumerable<AuctionMonitoringResponse> GetActiveAuctionsAsync(CancellationToken cancellationToken);
}

public record AuctionDto(Guid VehicleId, DateTime StartTime, DateTime? EndTime, bool IsActive);