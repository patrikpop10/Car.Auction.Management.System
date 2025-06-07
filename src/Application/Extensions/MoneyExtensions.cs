using Application.Models.Dtos;
using Domain.Entities;

namespace Application.Extensions;

public static class MoneyExtensions {
    public static MoneyDto ToDto(this Money money) => new() { Amount = money.Amount, Currency = money.CurrencyType.ToString() };
    public static Money ToDomain(this MoneyDto moneyDto) => new(moneyDto.Amount, Enum.TryParse(moneyDto.Currency, out CurrencyType currencyType) ? currencyType : CurrencyType.NONE);
}