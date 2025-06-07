using Application.Models.Requests;
using Application.Validators.DtosValidators;
using FluentValidation;

namespace Application.Validators.RequestValidators;

public class BidRequestValidator : AbstractValidator<BidRequest> {
    public BidRequestValidator() {
        RuleFor(x => x.Bidder)
            .NotEmpty()
            .WithMessage("Bidder is required.");

        RuleFor(x => x.Bid)
            .NotNull()
            .WithMessage("Amount is required.")
            .SetValidator(new MoneyDtoValidator());
    }
}