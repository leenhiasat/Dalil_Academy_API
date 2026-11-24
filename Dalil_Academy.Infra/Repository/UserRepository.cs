using Dalil_Academy.Core.Entities;
using Dalil_Academy.Core.Exceptions;
using Dalil_Academy.Core.Repository;
using Dalil_Academy.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Dalil_Academy.Infra.Repository;

public class UserRepository(AppDbContext db)
    : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        return db.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User> CreateAsync(
        User user,
        Student student)
    {
        await using var transaction =
            await db.Database.BeginTransactionAsync();

        try
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();

            student.UserId = user.Id;

            db.Students.Add(student);
            await db.SaveChangesAsync();

            await transaction.CommitAsync();

            return user;
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pg &&
                  pg.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            await transaction.RollbackAsync();

            db.Entry(user).State = EntityState.Detached;
            db.Entry(student).State = EntityState.Detached;

            var message = pg.ConstraintName switch
            {
                "IX_Users_Email" =>
                    "This email is already registered as a user.",

                "IX_Students_Email" =>
                    "This email is already registered as a student.",

                "IX_Students_UserId" =>
                    "This user already has a student profile.",

                _ =>
                    "A record with the same unique value already exists."
            };

            throw new ConflictException(message);
        }
        catch
        {
            await transaction.RollbackAsync();

            db.Entry(user).State = EntityState.Detached;
            db.Entry(student).State = EntityState.Detached;

            throw;
        }
    }
}