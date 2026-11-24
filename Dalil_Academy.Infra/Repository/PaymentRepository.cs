using Dalil_Academy.Core.Entities;
using Dalil_Academy.Core.Repository;
using Dalil_Academy.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Dalil_Academy.Infra.Repository;

public class PaymentRepository(AppDbContext db)
    : IPaymentRepository
{
    public async Task<Payment> CreateAsync(Payment payment)
    {
        db.Payments.Add(payment);
        await db.SaveChangesAsync();

        return payment;
    }

    public async Task<Payment?> GetLatestCreatedAsync(
        int enrollmentId)
    {
        return await db.Payments
            .Where(p =>
                p.EnrollmentId == enrollmentId &&
                p.Status == PaymentStatus.Created)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Payment payment)
    {
        await db.SaveChangesAsync();
    }

    public async Task<List<Payment>> GetByEnrollmentIdAsync(
        int enrollmentId)
    {
        return await db.Payments
            .Where(p => p.EnrollmentId == enrollmentId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}