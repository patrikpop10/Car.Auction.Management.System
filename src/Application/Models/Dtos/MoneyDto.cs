namespace Application.Models.Dtos;

public sealed record MoneyDto {
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
}