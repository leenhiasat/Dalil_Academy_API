using Dalil_Academy.Core.DTOs.Courses;
using Dalil_Academy.Core.DTOs;
using Dalil_Academy.Core.Entities;

namespace Dalil_Academy.Core.Repository;

public interface ICourseRepository
{
    Task<List<Course>>GetAllAsync();
    Task<List<Course>> GetByPriceRangeAsync( decimal? minPrice, decimal? maxPrice);

    Task<PageResult<Course>> GetPageAsync(int page, int pageSize);
    Task<Course?> GetByIdAsync(int id);

    Task<Course> CreateAsync(Course course);

    Task UpdateAsync(Course course);

    Task<bool> DeleteAsync(int id);

    Task<int> GetCountAsync();
    Task<(List<Course> Items, int TotalCount)> SearchAsync(
    string? search,
    int page,
    int pageSize
);
}

