namespace Voyara.Core;

public interface IEmailService
{
    Task SendBookingConfirmationAsync(string toEmail, string toName,
        string bookingRef, string packageName, decimal amount);

    Task SendPaymentReceiptAsync(string toEmail, string toName,
        string bookingRef, decimal amount, string method);

    Task SendPasswordResetOtpAsync(string toEmail, string otp);

    Task SendWelcomeEmailAsync(string toEmail, string name);
}