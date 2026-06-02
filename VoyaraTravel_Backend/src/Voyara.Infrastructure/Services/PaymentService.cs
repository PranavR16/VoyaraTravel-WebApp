using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;
using Voyara.Core;
using Voyara.Core.DTOs.Payments;
using Voyara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Voyara.Shared.Exceptions;

namespace Voyara.Infrastructure;

public class PaymentService(
    VoyaraDbContext db,
    IEmailService emailService,
    IConfiguration config) : IPaymentService
{
    public async Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
    {
        var booking = await db.Bookings
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == dto.BookingId)
            ?? throw new NotFoundException("Booking not found");

        if (booking.Status != "Pending")
            throw new BadRequestException("Booking is not in pending state");

        // Create Razorpay order
        //var client = new RazorpayClient(
        //    config["Razorpay:KeyId"],
        //    config["Razorpay:KeySecret"]);

        //var options = new Dictionary<string, object>
        //{
        //    { "amount",   (int)(booking.TotalAmount * 100) }, // paise
        //    { "currency", "INR" },
        //    { "receipt",  booking.BookingRef }
        //};

        //var order = client.Order.Create(options);
        //string orderId = order["id"].ToString()!;

        string orderId;

        bool useDummy = config.GetValue<bool>("Payment:UseDummy");

        if (useDummy)
        {
            // ✅ Dummy order (NO Razorpay call)
            orderId = "order_dummy_" + Guid.NewGuid().ToString("N");
        }
        else
        {
            var client = new RazorpayClient(
                config["Razorpay:KeyId"],
                config["Razorpay:KeySecret"]);

            var options = new Dictionary<string, object>
              {
                  { "amount",   (int)(booking.TotalAmount * 100) },
                  { "currency", "INR" },
                  { "receipt",  booking.BookingRef }
              };

            var order = client.Order.Create(options);
            orderId = order["id"].ToString()!;
        }

        // Save payment record
        var payment = new Voyara.Core.Entities.Payment
        {
            RazorpayOrderId = orderId,
            Method = dto.Method,
            Status = "Pending",
            Amount = booking.TotalAmount,
            Currency = "INR",
            BookingId = dto.BookingId
        };

        await db.Payments.AddAsync(payment);
        await db.SaveChangesAsync();

        //return new CreateOrderResponseDto(
        //    orderId,
        //    booking.TotalAmount,
        //    "INR",
        //    config["Razorpay:KeyId"]!);

        return new CreateOrderResponseDto(
                orderId,
                booking.TotalAmount,
                "INR",
                useDummy ? "dummy_key" : config["Razorpay:KeyId"]!
            );
    }

    public async Task<PaymentResponseDto> VerifyAsync(VerifyPaymentDto dto)
    {
        // Verify Razorpay signature
        //var payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";
        //var secret = config["Razorpay:KeySecret"]!;
        //var computed = ComputeHmac(payload, secret);

        //if (computed != dto.RazorpaySignature)
        //    throw new BadRequestException("Invalid payment signature");

        bool useDummy = config.GetValue<bool>("Payment:UseDummy");

        if (!useDummy)
        {
            var payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";
            var secret = config["Razorpay:KeySecret"]!;
            var computed = ComputeHmac(payload, secret);

            if (computed != dto.RazorpaySignature)
                throw new BadRequestException("Invalid payment signature");
        }

        // Update payment
        var payment = await db.Payments
            .Include(p => p.Booking)
                .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(p => p.RazorpayOrderId == dto.RazorpayOrderId)
            ?? throw new NotFoundException("Payment record not found");

        payment.RazorpayPaymentId = dto.RazorpayPaymentId;
        payment.RazorpaySignature = dto.RazorpaySignature;
        payment.Status = "Success";
        payment.PaidAt = DateTime.UtcNow;

        // Confirm booking
        payment.Booking.Status = "Confirmed";

        await db.SaveChangesAsync();

        // Send receipt email
        var user = payment.Booking.User;
        _ = emailService.SendPaymentReceiptAsync(
            user.Email,
            user.Name,
            payment.Booking.BookingRef,
            payment.Amount,
            payment.Method);

        return new PaymentResponseDto(
            true,
            payment.Booking.BookingRef,
            "Payment successful! Booking confirmed.");
    }

    public async Task HandleWebhookAsync(string payload, string signature)
    {
        // Verify webhook signature
        if (config.GetValue<bool>("Payment:UseDummy"))
        {
            return; // skip webhook validation in dummy mode
        }
        var secret = config["Razorpay:WebhookSecret"]!;
        var computed = ComputeHmac(payload, secret);

        if (computed != signature)
            throw new BadRequestException("Invalid webhook signature");

        // Handle payment.failed event
        if (payload.Contains("\"payment.failed\""))
        {
            // Extract order id from payload and mark payment as failed
            // Simplified — in production parse JSON properly
            Console.WriteLine("⚠️ Payment failed webhook received");
        }
    }

    // ── HMAC helper ───────────────────────────────────────
    private static string ComputeHmac(string message, string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var data = Encoding.UTF8.GetBytes(message);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(data);
        return BitConverter.ToString(hash)
                           .Replace("-", "")
                           .ToLower();
    }
}

