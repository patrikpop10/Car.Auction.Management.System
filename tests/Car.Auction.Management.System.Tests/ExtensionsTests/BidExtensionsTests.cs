using Application.Extensions;
using Application.Models.Dtos;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Entities.Auction;
using FluentAssertions;

namespace Car.Auction.Management.System.ExtensionsTests;

public class BidExtensionsTests {
    [Test]
    public void RequestToDto_WhenBidRequestIsProvided_ReturnsCorrectBidDto() {
        // Arrange
        var bidRequest = new BidRequest("John", new MoneyDto { Amount = 100, Currency = "USD" });

        // Act
        var dto = bidRequest.RequestToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Bidder.Should().Be("John");
        dto.Amount.Should().NotBeNull();
        dto.Amount.Amount.Should().Be(100);
        dto.Amount.Currency.Should().Be("USD");
    }

    [Test]
    public void RequestToDomain_WhenBidRequestIsProvided_ReturnsCorrectBid() {
        // Arrange
        var bidRequest = new BidRequest("John", new MoneyDto { Amount = 100, Currency = "USD" });

        // Act
        var domain = bidRequest.RequestToDomain();

        // Assert
        domain.Should().NotBeNull();
        domain.Bidder.Should().Be("John");
        domain.Value.Should().NotBeNull();
        domain.Value.Amount.Should().Be(100);
        domain.Value.CurrencyType.Should().Be(CurrencyType.USD);

    }

    [Test]
    public void ToDto_WhenBidIsProvided_ReturnsCorrectBidDto() {
        // Arrange
        var bid = new Bid("John", new Money(100, CurrencyType.USD));

        // Act
        var dto = bid.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Bidder.Should().Be("John");
        dto.Amount.Should().NotBeNull();
        dto.Amount.Amount.Should().Be(100);
        dto.Amount.Currency.Should().Be("USD");
    }
}