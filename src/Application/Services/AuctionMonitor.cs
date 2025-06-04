using System.Threading.Channels;
using Domain.Entities;

namespace Application.Services;

public class AuctionMonitor : IAuctionMonitor
{
    private readonly ChannelReader<Auction> _channelReader;

    public AuctionMonitor(ChannelReader<Auction> channelReader)
    {
        _channelReader = channelReader;
    }

    public async IAsyncEnumerable<Auction> GetActiveAuctionsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && await _channelReader.WaitToReadAsync(cancellationToken))
        {
            // Wait for the next auction to be available
            
            yield return await _channelReader.ReadAsync(cancellationToken);
        }
        
    }
}