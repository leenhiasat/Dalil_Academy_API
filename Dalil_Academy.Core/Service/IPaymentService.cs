using Dalil_Academy.Core.DTOs.Enrollments;
using Dalil_Academy.Core.DTOs.Payments;

namespace Dalil_Academy.Core.Service;

public interface IPaymentService
{
    Task<PayEnrollmentResponseDto> PayAsync(int enrollmentId);

    Task<EnrollmentResponseDto> CaptureAsync(int enrollmentId);
    Task<List<PaymentHistoryResponseDto>>
    GetPaymentHistoryAsync(int enrollmentId);

}