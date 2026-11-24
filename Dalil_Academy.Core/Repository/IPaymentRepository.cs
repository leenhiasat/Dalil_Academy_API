using Dalil_Academy.Core.Entities;

namespace Dalil_Academy.Core.Repository;

public interface IPaymentRepository
{
    Task<Payment> CreateAsync(Payment payment);

    Task<Payment?> GetLatestCreatedAsync(int enrollmentId);

    Task UpdateAsync(Payment payment);

    Task<List<Payment>> GetByEnrollmentIdAsync(int enrollmentId);
}