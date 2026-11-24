namespace Dalil_Academy.Core.DTOs.Enrollments;

public record PayEnrollmentResponseDto(
    int PaymentId,
    string OrderId,
    decimal Amount,
    string Currency,
    string ApprovalUrl
);