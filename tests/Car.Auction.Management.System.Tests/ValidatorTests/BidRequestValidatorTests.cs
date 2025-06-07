using Application.Models.Dtos;
using Application.Models.Requests;
using Application.Validators.RequestValidators;
using FluentValidation.TestHelper;

namespace Car.Auction.Management.System.ValidatorTests;

public class BidRequestValidatorTests {
    private BidRequestValidator _validator;

    [OneTimeSetUp]
    public void SetUp() => _validator = new BidRequestValidator();

    [Test]
    public void Validate_WhenBidderIsEmpty_ReturnsValidationError() {
        var request = new BidRequest("", new MoneyDto { Amount = 100, Currency = "USD" });
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Bidder);
    }

    [Test]
    public void Validate_WhenBidIsNull_ReturnsValidationError() {
        var request = new BidRequest("John Doe", null);
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Bid);
    }

    [Test]
    public void Validate_WhenBidRequestIsValid_ReturnsNoValidationErrors() {
        var request = new BidRequest("John Doe", new MoneyDto { Amount = 100, Currency = "USD" });
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}