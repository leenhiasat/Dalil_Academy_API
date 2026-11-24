using Dalil_Academy.Core.DTOs.Students;
using Dalil_Academy.Core.DTOs.Students;
using FluentValidation;

namespace Dalil_Academy.Core.Validators;

public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254);
    }
}
