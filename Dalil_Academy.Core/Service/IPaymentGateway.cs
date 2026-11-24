using Dalil_Academy.Core.DTOs.Payments;

namespace Dalil_Academy.Core.Service;

public interface IPaymentGateway
{
    Task<PayPalOrderResult> CreateOrderAsync(decimal amount);

    Task<bool> CaptureOrderAsync(string orderId);
}