using Domain.Entities;

namespace Domain.Events;

//check how to convert money to dto
public sealed record AuctionClosed(Guid AuctionId, Guid Vehicle, string Winner, Money WinningBid);