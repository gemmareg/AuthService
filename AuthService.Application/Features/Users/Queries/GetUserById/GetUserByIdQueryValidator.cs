using AuthService.Application.Features.Users.Queries.GetUser;
using AuthService.Shared;
using FluentValidation;

namespace AuthService.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
    {
        public GetUserByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID is required.")
                .Must(ValidationHelpers.BeAValidGuid).WithMessage("Invalid User ID format.");
        }
    }
}
