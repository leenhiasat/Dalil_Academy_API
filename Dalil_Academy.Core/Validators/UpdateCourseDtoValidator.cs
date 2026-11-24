using Dalil_Academy.Core.DTOs.Courses;
using FluentValidation;

namespace Dalil_Academy.Core.Validators;

public class UpdateCourseDtoValidator
    : AbstractValidator<UpdateCourseDto>
{
    public UpdateCourseDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be zero or more.");

        RuleFor(x => x.MaxStudents)
            .GreaterThan(0)
            .When(x => x.MaxStudents.HasValue)
            .WithMessage("MaxStudents must be greater than zero.");
    }
}