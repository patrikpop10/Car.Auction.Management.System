namespace Domain.Entities;

public sealed record Money(decimal Amount, CurrencyType CurrencyType)
{ 
    public static Money None() => new Money(0, CurrencyType.NONE);
}
public enum CurrencyType
{
    USD,
    EUR,
    GBP,
    JPY,
    NONE
}