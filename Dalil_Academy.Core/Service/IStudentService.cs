using Dalil_Academy.Core.DTOs;
using Dalil_Academy.Core.DTOs.Courses;
using Dalil_Academy.Core.DTOs.Courses;
using Dalil_Academy.Core.DTOs.Students;
using Dalil_Academy.Core.DTOs.Students;

namespace Dalil_Academy.Core.Service;

public interface IStudentService
{
    Task<List<StudentResponseDto>> GetAllAsync();
    Task<PageResult<StudentResponseDto>> GetPageAsync(
    int page,
    int pageSize);

    Task<StudentResponseDto> GetByIdAsync(int id);

    Task<StudentResponseDto> CreateAsync(CreateStudentDto dto);

    Task UpdateAsync(int id, UpdateStudentDto dto);

    Task DeleteAsync(int id);

    Task<List<CourseResponseDto>> GetCoursesAsync(int id);
}
