using Application.Models.Dtos;

namespace Application.Models.Requests;

public sealed record BidRequest(string Bidder, MoneyDto Bid);