using Dalil_Academy.Core.DTOs;
using Dalil_Academy.Core.Entities;
using Dalil_Academy.Core.Exceptions;
using Dalil_Academy.Core.Repository;
using Dalil_Academy.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Dalil_Academy.Infra.Repository;

public class EfCourseRepository(AppDbContext db)
    : ICourseRepository
{
    public async Task<PageResult<Course>> GetPageAsync(
        int page,
        int pageSize)
    {
        var totalCount = await db.Courses.CountAsync();

        var courses = await db.Courses
            .AsNoTracking()
            .Include(c => c.Instructor)
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PageResult<Course>(
            courses,
            page,
            pageSize,
            totalCount
        );
    }

    public async Task<List<Course>> GetAllAsync()
    {
        return await db.Courses
            .AsNoTracking()
            .Include(c => c.Instructor)
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        return await db.Courses
            .Include(c => c.Instructor)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Course> CreateAsync(Course course)
    {
        try
        {
            db.Courses.Add(course);
            await db.SaveChangesAsync();

            await db.Entry(course)
                .Reference(c => c.Instructor)
                .LoadAsync();

            return course;
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            throw new ConflictException(
                "A course with this title already exists.");
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.ForeignKeyViolation
            })
        {
            throw new NotFoundException(
                $"Instructor {course.InstructorId} was not found.");
        }
    }

    public async Task UpdateAsync(Course course)
    {
        try
        {
            db.Courses.Update(course);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            throw new ConflictException(
                "A course with this title already exists.");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await db.Courses.FindAsync(id);

        if (course is null)
            return false;

        db.Courses.Remove(course);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<List<Course>> GetByPriceRangeAsync(
        decimal? minPrice,
        decimal? maxPrice)
    {
        var query = db.Courses
            .AsNoTracking()
            .Include(c => c.Instructor)
            .AsQueryable();

        if (minPrice.HasValue)
        {
            query = query.Where(c =>
                c.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(c =>
                c.Price <= maxPrice.Value);
        }

        return await query
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await db.Courses.CountAsync();
    }

    public async Task<(List<Course> Items, int TotalCount)> SearchAsync(
        string? search,
        int page,
        int pageSize)
    {
        var query = db.Courses
            .AsNoTracking()
            .Include(c => c.Instructor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim();

            query = query.Where(c =>
                EF.Functions.ILike(
                    c.Title,
                    $"%{searchTerm}%"));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}