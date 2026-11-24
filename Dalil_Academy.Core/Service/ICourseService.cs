using Dalil_Academy.Core.DTOs;
using Dalil_Academy.Core.DTOs.Common;
using Dalil_Academy.Core.DTOs.Courses;

namespace Dalil_Academy.Core.Service;

public interface ICourseService
{

    Task<PageResult<CourseResponseDto>> GetPageAsync(int page, int pageSize);
    Task<List<CourseResponseDto>> GetAllAsync();
    Task<List<CourseResponseDto>> GetByPriceRangeAsync(
    decimal? minPrice,
    decimal? maxPrice);

    Task<CourseResponseDto> GetByIdAsync(int id);

    Task<CourseResponseDto> CreateAsync(CreateCourseDto dto);

    Task UpdateAsync(int id, UpdateCourseDto dto);

    Task DeleteAsync(int id);

    Task<int> GetCountAsync();
    Task<PagedResponseDto<CourseResponseDto>> SearchAsync(
    string? search,
    int page,
    int pageSize
);

}
