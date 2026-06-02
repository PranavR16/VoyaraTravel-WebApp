using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using Voyara.Core;

using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
namespace Voyara.Infrastructure;

public class EmailService(IConfiguration config) : IEmailService
{
    private readonly string _fromEmail = config["SendGrid:FromEmail"]!;
    private readonly string _fromName = config["SendGrid:FromName"]!;

    public async Task SendBookingConfirmationAsync(
        string toEmail, string toName,
        string bookingRef, string packageName, decimal amount)
    {
        var subject = $"✅ Booking Confirmed — {bookingRef}";
        var html = $"""
            <div style="font-family:DM Sans,sans-serif;max-width:600px;margin:0 auto;background:#F8F6F0;padding:32px;border-radius:16px;">
              <h1 style="color:#1B4332;font-family:Georgia,serif;">Booking Confirmed! 🎉</h1>
              <p>Hi <strong>{toName}</strong>, your journey is all set.</p>
              <div style="background:white;border-radius:12px;padding:20px;margin:20px 0;border-left:4px solid #B8860B;">
                <p><strong>Booking Ref:</strong> {bookingRef}</p>
                <p><strong>Package:</strong> {packageName}</p>
                <p><strong>Total:</strong> ₹{amount:N0}</p>
              </div>
              <p>We'll send you all travel documents 48 hours before departure.</p>
              <p style="color:#5A7A5A;">— Team Voyara 🌿</p>
            </div>
            """;

        await SendAsync(toEmail, toName, subject, html);
    }

    public async Task SendPaymentReceiptAsync(
        string toEmail, string toName,
        string bookingRef, decimal amount, string method)
    {
        var subject = $"💳 Payment Receipt — {bookingRef}";
        var html = $"""
            <div style="font-family:DM Sans,sans-serif;max-width:600px;margin:0 auto;background:#F8F6F0;padding:32px;border-radius:16px;">
              <h1 style="color:#1B4332;font-family:Georgia,serif;">Payment Successful</h1>
              <p>Hi <strong>{toName}</strong>, payment received.</p>
              <div style="background:white;border-radius:12px;padding:20px;margin:20px 0;">
                <p><strong>Reference:</strong> {bookingRef}</p>
                <p><strong>Amount:</strong> ₹{amount:N0}</p>
                <p><strong>Method:</strong> {method}</p>
                <p><strong>Date:</strong> {DateTime.UtcNow:dd MMM yyyy}</p>
              </div>
              <p style="color:#5A7A5A;">— Team Voyara 🌿</p>
            </div>
            """;

        await SendAsync(toEmail, toName, subject, html);
    }

    public async Task SendPasswordResetOtpAsync(string toEmail, string otp)
    {
        var subject = "🔑 Your Voyara Password Reset OTP";
        var html = $"""
            <div style="font-family:DM Sans,sans-serif;max-width:600px;margin:0 auto;">
              <h2>Password Reset</h2>
              <p>Your OTP is:</p>
              <div style="font-size:36px;font-weight:bold;color:#1B4332;letter-spacing:8px;padding:20px;background:#D8F3DC;border-radius:12px;text-align:center;">
                {otp}
              </div>
              <p>This OTP expires in 10 minutes.</p>
              <p style="color:red;">If you didn't request this, ignore this email.</p>
            </div>
            """;

        await SendAsync(toEmail, toEmail, subject, html);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string name)
    {
        var subject = "🌿 Welcome to Voyara Travel!";
        var html = $"""
            <div style="font-family:DM Sans,sans-serif;max-width:600px;margin:0 auto;background:#F8F6F0;padding:32px;border-radius:16px;">
              <h1 style="color:#1B4332;font-family:Georgia,serif;">Welcome, {name}! 🎉</h1>
              <p>Thank you for joining Voyara. Extraordinary journeys await you.</p>
              <a href="https://voyara.com/packages"
                 style="display:inline-block;background:#1B4332;color:white;padding:14px 32px;border-radius:50px;text-decoration:none;font-weight:700;margin-top:16px;">
                Explore Packages →
              </a>
              <p style="color:#5A7A5A;margin-top:24px;">— Team Voyara 🌿</p>
            </div>
            """;

        await SendAsync(toEmail, name, subject, html);
    }

    // ── Core Send ─────────────────────────────────────────
    private async Task SendAsync(
        string toEmail, string toName,
        string subject, string htmlContent)
    {
        var client = new SendGridClient(config["SendGrid:ApiKey"]);
        var from = new EmailAddress(_fromEmail, _fromName);
        var to = new EmailAddress(toEmail, toName);
        var message = MailHelper.CreateSingleEmail(
            from, to, subject, string.Empty, htmlContent);

        var response = await client.SendEmailAsync(message);

        if (!response.IsSuccessStatusCode)
            Console.WriteLine($"⚠️ Email failed: {response.StatusCode}");
    }
}

