using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Auction;

namespace Application.Services;

public class AuctionMonitor : IAuctionMonitor
{
    private readonly ChannelReader<Auction> _channelReader;

    public AuctionMonitor(ChannelReader<Auction> channelReader)
    {
        _channelReader = channelReader;
    }

    public async IAsyncEnumerable<Auction> GetActiveAuctionsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && await _channelReader.WaitToReadAsync(cancellationToken))
        {
            yield return await _channelReader.ReadAsync(cancellationToken);
        }
        
    }
}