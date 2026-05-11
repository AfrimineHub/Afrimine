using Afrimine.Services.DTOs;
using FluentValidation;

namespace Afrimine.Services.Validators
{
    internal class RegistrationRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegistrationRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required")
                .Must(name => ValidatorHelpers.ValidFullName(name));

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please enter a valid email address");

            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone number is required")
                .Must(phone => ValidatorHelpers.ValidE164(phone))
                .WithMessage("Phone number not in the correct format");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");

            RuleFor(x => x).Must(args => ValidatorHelpers.PasswordMatches(args.Password, args.ConfirmPassword))
                .WithMessage("Password and Confirm Password must match");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Please select a valid role");
        }
    }
}
