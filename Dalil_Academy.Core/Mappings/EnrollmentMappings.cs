using Dalil_Academy.Core.DTOs.Enrollments;
using Dalil_Academy.Core.Entities;

namespace Dalil_Academy.Core.Mappings;

public static class EnrollmentMappings
{
    public static EnrollmentResponseDto ToDto(
        this Enrollment enrollment)
    {
        return new EnrollmentResponseDto(
            enrollment.Id,
            enrollment.StudentId,
            enrollment.Student?.FullName ?? string.Empty,
            enrollment.CourseId,
            enrollment.Course?.Title ?? string.Empty,
            enrollment.EnrolledAt,
            enrollment.Status
        );
    }
}