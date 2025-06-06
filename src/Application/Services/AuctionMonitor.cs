using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Application.Interfaces;
using Application.Models.Responses;

namespace Application.Services;

public class AuctionMonitor : IAuctionMonitor
{
    private readonly ChannelReader<AuctionMonitoringResponse> _channelReader;

    public AuctionMonitor(ChannelReader<AuctionMonitoringResponse> channelReader)
    {
        _channelReader = channelReader;
    }

    public async IAsyncEnumerable<AuctionMonitoringResponse> GetActiveAuctionsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && await _channelReader.WaitToReadAsync(cancellationToken))
        {
            yield return await _channelReader.ReadAsync(cancellationToken);
        }

    }
}