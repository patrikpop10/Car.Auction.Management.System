using Application.Extensions;
using Application.Models.Dtos;
using Domain.Entities;
using FluentAssertions;

namespace Car.Auction.Management.System.ExtensionsTests;

public class MoneyExtensionsTests {
    [Test]
    public void ToDto_WhenMoneyIsProvided_ReturnsCorrectMoneyDto() {
        // Arrange
        var money = new Money(100, CurrencyType.USD);

        // Act
        var dto = money.ToDto();

        // Assert
        dto.Amount.Should().Be(100);
        dto.Currency.Should().Be("USD");
    }

    [Test]
    public void ToDomain_WhenMoneyDtoIsProvided_ReturnsCorrectMoney() {
        // Arrange
        var moneyDto = new MoneyDto { Amount = 100, Currency = "USD" };

        // Act
        var domain = moneyDto.ToDomain();

        // Assert
        domain.Amount.Should().Be(100);
        domain.CurrencyType.Should().Be(CurrencyType.USD);
    }
}