using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Features.Tokens.Queries.RefreshToken
{
    public class RefreshTokenQueryValidator : AbstractValidator<RefreshTokenQuery>
    {
        public RefreshTokenQueryValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.")
                .MinimumLength(10).WithMessage("Refresh token must be at least 10 characters long.");
        }
    }
}
