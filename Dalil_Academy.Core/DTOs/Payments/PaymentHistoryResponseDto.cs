using Dalil_Academy.Core.Entities;

namespace Dalil_Academy.Core.DTOs.Payments;

public record PaymentHistoryResponseDto(
    int Id,
    int EnrollmentId,
    decimal Amount,
    string Currency,
    string PayPalOrderId,
    PaymentStatus Status,
    DateTime CreatedAt,
    DateTime? PaidAt
);