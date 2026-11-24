using Dalil_Academy.Core.DTOs.Courses;
using Dalil_Academy.Core.DTOs.Enrollments;
using Dalil_Academy.Core.DTOs.Students;

namespace Dalil_Academy.Core.Service;

public interface IEnrollmentService
{
    Task<EnrollmentResponseDto> EnrollAsync(
        int studentId,
        int courseId);

    Task<List<CourseResponseDto>> GetMyCoursesAsync(int userId);

    Task<List<StudentResponseDto>> GetPaidStudentsByCourseIdAsync(
        int courseId);

    Task<EnrollmentResponseDto> UpdateEnrollmentAsync(
    int id,
    int courseId);

    Task DeleteEnrollmentAsync(int id);
}