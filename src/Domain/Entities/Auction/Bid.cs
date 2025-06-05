namespace Domain.Entities.Auction;

public sealed class Bid
{
    public string Bidder { get; }
    public Money Value { get; }
    public DateTime PlacedAt { get; }

    public Bid(string bidder, Money value)
    {
        Bidder = bidder;
        Value = value;
        PlacedAt = DateTime.UtcNow;
    }
    public static Bid Empty => new Bid(string.Empty, Money.None());
}