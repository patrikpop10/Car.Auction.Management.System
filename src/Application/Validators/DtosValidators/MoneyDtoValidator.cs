using Application.Models.Dtos;
using FluentValidation;

namespace Application.Validators.DtosValidators;

public class MoneyDtoValidator: AbstractValidator<MoneyDto>
{
    public MoneyDtoValidator()
    {
        RuleFor(m => m.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(m => m.Currency)
            .NotEmpty()
            .WithMessage("Currency must not be empty.");

        RuleFor(m => m.Currency)
            .Length(3, 3)
            .WithMessage("Currency must be exactly 3 characters long.");

        RuleFor(m => m.Currency)
            .Matches("^[A-Z]+$")
            .WithMessage("Currency must consist of uppercase letters only.");
        
        RuleFor(m => m.Currency)
            .Must(BeAValidCurrency)
            .WithMessage("Currency must be a valid currency code.");
    }

    private static bool BeAValidCurrency(string arg)
    {
        // This is a simplified example. In a real application, you might want to check against a more comprehensive list of valid currencies.
        var validCurrencies = new[] { "USD", "EUR", "GBP", "JPY"};
        return validCurrencies.Contains(arg);
    }
}