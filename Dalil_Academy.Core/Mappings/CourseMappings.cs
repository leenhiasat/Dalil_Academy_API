using Dalil_Academy.Core.DTOs.Courses;
using Dalil_Academy.Core.Entities;

namespace Dalil_Academy.Core.Mappings;

public static class CourseMappings
{
    public static CourseResponseDto ToDto(this Course course)
    {
        return new CourseResponseDto(
            course.Id,
            course.Title,
            course.Price,
            course.Instructor?.FullName,
            course.MaxStudents
        );
    }

    public static Course ToEntity(this CreateCourseDto dto)
    {
        return new Course
        {
            Title = dto.Title,
            Price = dto.Price,
            InstructorId = dto.InstructorId,
            MaxStudents = dto.MaxStudents
        };
    }

    public static void UpdateFromDto(
        this Course course,
        UpdateCourseDto dto)
    {
        course.Title = dto.Title;
        course.Price = dto.Price;
        course.MaxStudents = dto.MaxStudents;
    }
}