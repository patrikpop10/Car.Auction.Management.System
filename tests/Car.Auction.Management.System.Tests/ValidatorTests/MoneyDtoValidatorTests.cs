using Application.Models.Dtos;
using Application.Validators.DtosValidators;

using FluentValidation.TestHelper;

namespace Car.Auction.Management.System.ValidatorTests;

public class MoneyDtoValidatorTests {
    private MoneyDtoValidator _validator;

    [OneTimeSetUp]
    public void Setup() => _validator = new MoneyDtoValidator();

    [Test]
    public void Validate_WhenAmountIsLessThanOrEqualToZero_ReturnsValidationError() {
        var dto = new MoneyDto { Amount = 0, Currency = "USD" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Test]
    public void Validate_WhenCurrencyIsEmpty_ReturnsValidationError() {
        var dto = new MoneyDto { Amount = 100, Currency = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Test]
    public void Validate_WhenCurrencyIsNotThreeCharactersLong_ReturnsValidationError() {
        var dto = new MoneyDto { Amount = 100, Currency = "US" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Test]
    public void Validate_WhenCurrencyIsNotUppercase_ReturnsValidationError() {
        var dto = new MoneyDto { Amount = 100, Currency = "usd" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Test]
    public void Validate_WhenCurrencyIsInvalid_ReturnsValidationError() {
        var dto = new MoneyDto { Amount = 100, Currency = "ABC" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Test]
    public void Validate_WhenMoneyDtoIsValid_ReturnsNoValidationErrors() {
        var dto = new MoneyDto { Amount = 100, Currency = "USD" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}