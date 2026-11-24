namespace Dalil_Academy.Core.DTOs.Payments;

public record PayPalOrderResult(
    string OrderId,
    string ApprovalUrl
);