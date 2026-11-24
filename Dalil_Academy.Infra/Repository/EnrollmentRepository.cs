using Dalil_Academy.Core.Entities;
using Dalil_Academy.Core.Exceptions;
using Dalil_Academy.Core.Repository;
using Dalil_Academy.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Dalil_Academy.Infra.Repository;

public class EnrollmentRepository(AppDbContext db)
    : IEnrollmentRepository
{
    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        try
        {
            db.Enrollments.Add(enrollment);
            await db.SaveChangesAsync();

            return enrollment;
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            throw new ConflictException(
                "This student is already enrolled in this course.");
        }
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        return await db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task UpdateAsync(Enrollment enrollment)
    {
        await db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Course>> GetCoursesByUserIdAsync(
     int userId)
    {
        return await db.Enrollments
            .Where(e =>
                e.Student != null &&
                e.Student.UserId == userId &&
                e.Status == EnrollmentStatus.Paid)
            .Include(e => e.Course)
                .ThenInclude(c => c!.Instructor)
            .Select(e => e.Course!)
            .ToListAsync();
    }

    public async Task<bool> CourseExistsAsync(int courseId)
    {
        return await db.Courses.AnyAsync(c => c.Id == courseId);
    }

    public async Task<List<Student>> GetPaidStudentsByCourseIdAsync(
        int courseId)
    {
        return await db.Enrollments
            .Where(e =>
                e.CourseId == courseId &&
                e.Status == EnrollmentStatus.Paid)
            .Select(e => e.Student!)
            .ToListAsync();
    }

    public async Task DeleteAsync(Enrollment enrollment)
    {
        db.Enrollments.Remove(enrollment);
        await db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var enrollment = await db.Enrollments
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrollment is null)
            return false;

        db.Enrollments.Remove(enrollment);

        await db.SaveChangesAsync();

        return true;
    }

    public async Task<int> CountPaidEnrollmentsByCourseIdAsync(
    int courseId)
    {
        return await db.Enrollments
            .CountAsync(e =>
                e.CourseId == courseId &&
                e.Status == EnrollmentStatus.Paid);
    }
}