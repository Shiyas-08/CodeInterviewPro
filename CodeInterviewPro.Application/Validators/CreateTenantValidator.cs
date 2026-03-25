using CodeInterviewPro.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Validators
{
    public class CreateTenantValidator : AbstractValidator<CreateTenantRequest>
    {
        public CreateTenantValidator()
        {

            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(x => x.Domain)
                .NotEmpty()
                .Matches(@"^[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
                .WithMessage("Invalid domain");
        }
    }
    public class UpdateTenantValidator : AbstractValidator<UpdateTenantRequest>
    {
        public UpdateTenantValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Domain).NotEmpty();
        }
    }

}