using Dalil_Academy.Core.DTOs.Courses;
using Dalil_Academy.Core.DTOs.Enrollments;
using Dalil_Academy.Core.DTOs.Students;
using Dalil_Academy.Core.Entities;
using Dalil_Academy.Core.Exceptions;
using Dalil_Academy.Core.Mappings;
using Dalil_Academy.Core.Repository;
using Dalil_Academy.Core.Service;

namespace Dalil_Academy.Infra.Service;

public class EnrollmentService(
    IStudentRepository students,
    ICourseRepository courses,
    IEnrollmentRepository enrollments)
    : IEnrollmentService
{
    public async Task<EnrollmentResponseDto> EnrollAsync(
        int studentId,
        int courseId)
    {
        var student = await students.GetByIdAsync(studentId)
            ?? throw new NotFoundException(
                $"Student {studentId} was not found.");

        var course = await courses.GetByIdAsync(courseId)
            ?? throw new NotFoundException(
                $"Course {courseId} was not found.");

        // Check course capacity
        if (course.MaxStudents.HasValue)
        {
            var paidEnrollments =
                await enrollments.CountPaidEnrollmentsByCourseIdAsync(
                    courseId);

            if (paidEnrollments >= course.MaxStudents.Value)
            {
                throw new ConflictException(
                    "This course is full. No available seats.");
            }
        }

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            Student = student,
            CourseId = courseId,
            Course = course,
            EnrolledAt = DateTime.UtcNow,

            Status = course.Price == 0
                ? EnrollmentStatus.Paid
                : EnrollmentStatus.Pending
        };

        await enrollments.CreateAsync(enrollment);

        return enrollment.ToDto();
    }

    public async Task<List<CourseResponseDto>> GetMyCoursesAsync(
    int userId)
    {
        var courses =
            await enrollments.GetCoursesByUserIdAsync(userId);

        return courses
            .Select(c => c.ToDto())
            .ToList();
    }

    public async Task<List<StudentResponseDto>>
        GetPaidStudentsByCourseIdAsync(int courseId)
    {
        if (!await enrollments.CourseExistsAsync(courseId))
        {
            throw new NotFoundException(
                $"Course {courseId} was not found.");
        }

        var studentsList =
            await enrollments.GetPaidStudentsByCourseIdAsync(courseId);

        return studentsList
            .Select(s => s.ToDto())
            .ToList();
    }


    public async Task<EnrollmentResponseDto> UpdateEnrollmentAsync(
        int id,
        int courseId)
    {
        // Get enrollment
        var enrollment = await enrollments.GetByIdAsync(id)
            ?? throw new NotFoundException(
                $"Enrollment {id} was not found.");

        // Prevent modifying paid enrollment
        if (enrollment.Status == EnrollmentStatus.Paid)
        {
            throw new ConflictException(
                "A paid enrollment cannot be modified or deleted.");
        }

        // Check if the new course exists
        var course = await courses.GetByIdAsync(courseId)
            ?? throw new NotFoundException(
                $"Course {courseId} was not found.");

        // Update course
        enrollment.CourseId = courseId;
        enrollment.Course = course;

        await enrollments.UpdateAsync(enrollment);

        return enrollment.ToDto();
    }

    public async Task DeleteEnrollmentAsync(int id)
    {
        // Get enrollment
        var enrollment = await enrollments.GetByIdAsync(id)
            ?? throw new NotFoundException(
                $"Enrollment {id} was not found.");

        // Prevent deleting paid enrollment
        if (enrollment.Status == EnrollmentStatus.Paid)
        {
            throw new ConflictException(
                "A paid enrollment cannot be modified or deleted.");
        }

        await enrollments.DeleteAsync(enrollment);
    }

}