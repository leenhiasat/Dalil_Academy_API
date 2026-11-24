using Dalil_Academy.Core.Entities;

namespace Dalil_Academy.Core.DTOs.Enrollments;

public record EnrollmentResponseDto(
    int Id,
    int StudentId,
    string StudentName,
    int CourseId,
    string CourseTitle,
    DateTime EnrolledAt,
    EnrollmentStatus Status
);