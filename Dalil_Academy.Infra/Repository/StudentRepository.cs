using Dalil_Academy.Core.DTOs;
using Dalil_Academy.Core.Entities;
using Dalil_Academy.Core.Exceptions;
using Dalil_Academy.Core.Repository;
using Dalil_Academy.Infra.Data;
using Dalil_Academy.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Dalil_Academy.Infra.Repository;

public class StudentRepository(AppDbContext db)
    : IStudentRepository
{
    public async Task<List<Student>> GetAllAsync()
    {
        return await db.Students.ToListAsync();
    }

    public Task<Student?> GetByUserIdAsync(int userId)
    {
        return db.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }
    public async Task<Student?> GetByIdAsync(int id)
    {
        return await db.Students.FindAsync(id);
    }

    public async Task<Student?> GetWithCoursesAsync(int id)
    {
        return await db.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c!.Instructor)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student> CreateAsync(Student student)
    {
        try
        {
            db.Students.Add(student);
            await db.SaveChangesAsync();

            return student;
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            throw new ConflictException(
                "A student with this email already exists.");
        }
    }

    public async Task UpdateAsync(Student student)
    {
        try
        {
            db.Students.Update(student);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            throw new ConflictException(
                "A student with this email already exists.");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await db.Students.FindAsync(id);

        if (student is null)
            return false;

        db.Students.Remove(student);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<PageResult<Student>> GetPageAsync(
    int page,
    int pageSize)
    {
        var totalCount = await db.Students.CountAsync();

        var students = await db.Students
            .Include(s => s.User)
            .OrderBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PageResult<Student>(
            students,
            page,
            pageSize,
            totalCount
        );
    }
}
