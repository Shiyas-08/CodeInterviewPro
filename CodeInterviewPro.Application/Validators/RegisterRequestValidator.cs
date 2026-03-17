using FluentValidation;
using CodeInterviewPro.Application.DTOs;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Matches(@"^(?!_)(?!.*__)[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$")
            .WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$")
            .WithMessage("Password must contain uppercase, lowercase, number and special character");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .Matches(@"^[A-Za-z][A-Za-z\s]{2,50}$")
            .WithMessage("Full name must contain only letters and spaces");
    }
}