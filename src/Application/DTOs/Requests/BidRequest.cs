namespace Application.DTOs.Requests;

public sealed record BidRequest(string Bidder, MoneyDto Amount);