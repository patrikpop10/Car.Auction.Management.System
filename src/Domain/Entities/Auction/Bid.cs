namespace Domain.Entities.Auction;

public sealed class Bid(string bidder, Money value) {
    public string Bidder { get; } = bidder;
    public Money Value { get; } = value;
    public DateTime PlacedAt { get; } = DateTime.UtcNow;

    public static Bid Empty => new(string.Empty, Money.None());
}