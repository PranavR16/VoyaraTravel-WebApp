using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Core;
using Voyara.Core.DTOs.Payments;

namespace Voyara.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController(IPaymentService paymentService) : ControllerBase
    {
        // POST api/payments/create-order
        [HttpPost("create-order")]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var result = await paymentService.CreateOrderAsync(dto);
            return Ok(result);
        }

        // POST api/payments/verify
        [HttpPost("verify")]
        [Authorize]
        public async Task<IActionResult> Verify([FromBody] VerifyPaymentDto dto)
        {
            var result = await paymentService.VerifyAsync(dto);
            return Ok(result);
        }

        // POST api/payments/webhook
        // No [Authorize] — Razorpay calls this directly
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var payload = await new StreamReader(Request.Body).ReadToEndAsync();
            var signature = Request.Headers["X-Razorpay-Signature"].ToString();
            await paymentService.HandleWebhookAsync(payload, signature);
            return Ok();
        }
    }

    //[ApiController]
    //[Route("api/payments")]
    //public class PaymentsController(IPaymentService paymentService) : ControllerBase
    //{
    //    [HttpPost("create-order")]
    //    [Authorize]
    //    public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
    //        => Ok(await paymentService.CreateOrderAsync(dto));

    //    [HttpPost("verify")]
    //    [Authorize]
    //    public async Task<IActionResult> Verify(VerifyPaymentDto dto)
    //        => Ok(await paymentService.VerifyAsync(dto));

    //    [HttpPost("webhook")]
    //    public async Task<IActionResult> Webhook()
    //    {
    //        var payload = await new StreamReader(Request.Body).ReadToEndAsync();
    //        var signature = Request.Headers["X-Razorpay-Signature"].ToString();
    //        await paymentService.HandleWebhookAsync(payload, signature);
    //        return Ok();
    //    }
    //}
}
