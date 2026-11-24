using Dalil_Academy.Core.DTOs;
using Dalil_Academy.Core.Entities;
using Dalil_Academy.Core.Entities;

namespace Dalil_Academy.Core.Repository;

public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();

    Task<Student?> GetByIdAsync(int id);
    Task<Student?> GetByUserIdAsync(int userId);

    Task<PageResult<Student>> GetPageAsync(int page, int pageSize);
    Task<Student?> GetWithCoursesAsync(int id);

    Task<Student> CreateAsync(Student student);

    Task UpdateAsync(Student student);

    Task<bool> DeleteAsync(int id);
}
