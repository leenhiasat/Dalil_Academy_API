using Dalil_Academy.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dalil_Academy.Infra.Data;

public class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Course> Courses => Set<Course>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Instructor> Instructors => Set<Instructor>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}