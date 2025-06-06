using Application.DTOs;
using Domain.Entities;

namespace Application.Extensions;

public static class MoneyExtensions
{
    public static MoneyDto ToDto(this Money money)
    {
        return new MoneyDto{Amount = money.Amount, Currency = money.CurrencyType.ToString()};
    }
    public static Money ToDomain(this MoneyDto moneyDto)
    {
        return new Money(moneyDto.Amount, Enum.TryParse(moneyDto.Currency, out CurrencyType currencyType) ? currencyType : CurrencyType.NONE);
    }
}