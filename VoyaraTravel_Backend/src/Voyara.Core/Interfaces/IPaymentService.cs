using Voyara.Core.DTOs.Payments;

namespace Voyara.Core;

public interface IPaymentService
{
    Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
    Task<PaymentResponseDto> VerifyAsync(VerifyPaymentDto dto);
    Task HandleWebhookAsync(string payload, string signature);
}