namespace Application.Models.Dtos;

public record BidDto {
    public required string Bidder { get; init; }
    public required MoneyDto Amount { get; init; }
}