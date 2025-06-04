namespace Domain.Entities.Auction;

//check how to convert money to dto
public sealed record AuctionClosed(Guid Vehicle, string Winner, Money WinningBid);