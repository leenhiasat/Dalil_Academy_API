using Dalil_Academy.Core.Entities;

namespace Dalil_Academy.Core.Repository;

public interface IEnrollmentRepository
{
    Task<Enrollment> CreateAsync(Enrollment enrollment);

    Task<Enrollment?> GetByIdAsync(int id);

    Task UpdateAsync(Enrollment enrollment);

    Task<IEnumerable<Course>> GetCoursesByUserIdAsync(int userId);

    Task<bool> CourseExistsAsync(int courseId);

    Task<List<Student>> GetPaidStudentsByCourseIdAsync(int courseId);
    Task DeleteAsync(Enrollment enrollment);
    Task<bool> DeleteAsync(int id);
    Task<int> CountPaidEnrollmentsByCourseIdAsync(int courseId);


}