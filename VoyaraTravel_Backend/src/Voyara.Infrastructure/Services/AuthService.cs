using Microsoft.Extensions.Caching.Distributed;
using Voyara.Core.Entities;
using Voyara.Core;
using Voyara.Core.DTOs.Auth;
using Voyara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Voyara.Shared.Exceptions;


namespace Voyara.Infrastructure;

public class AuthService(
    VoyaraDbContext db,
    ITokenService tokenService,
    IEmailService emailService,
    IDistributedCache cache) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // Check email exists
        if (await db.Users.AnyAsync(u => u.Email == dto.Email.ToLower()))
            throw new BadRequestException("Email already registered");

        var user = new User
        {
            Name = dto.Name.Trim(),
            Email = dto.Email.ToLower().Trim(),
            Phone = dto.Phone.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Nationality = dto.Nationality ?? string.Empty,
            Dob = dto.Dob,
            Role = "User"
        };

        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        // Send welcome email (fire and forget)
        _ = emailService.SendWelcomeEmailAsync(user.Email, user.Name);

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower())
            ?? throw new UnauthorizedException("Invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password");

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var userId = tokenService.GetUserIdFromToken(dto.AccessToken)
            ?? throw new UnauthorizedException("Invalid token");

        var user = await db.Users.FindAsync(userId)
            ?? throw new UnauthorizedException("User not found");

        if (user.RefreshToken != dto.RefreshToken ||
            user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new UnauthorizedException("Refresh token expired or invalid");

        return BuildAuthResponse(user);
    }

    public async Task LogoutAsync(Guid userId)
    {
        var user = await db.Users.FindAsync(userId);
        if (user == null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await db.SaveChangesAsync();
    }

    public async Task SendPasswordResetOtpAsync(string email)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLower())
            ?? throw new NotFoundException("No account found with this email");

        // Generate 6-digit OTP
        var otp = new Random().Next(100000, 999999).ToString();

        // Store OTP in Redis/cache for 10 minutes
        await cache.SetStringAsync(
            $"otp:{email.ToLower()}",
            otp,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        await emailService.SendPasswordResetOtpAsync(user.Email, otp);
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        // Validate OTP
        var storedOtp = await cache.GetStringAsync($"otp:{dto.Email.ToLower()}")
            ?? throw new BadRequestException("OTP expired or invalid");

        if (storedOtp != dto.Otp)
            throw new BadRequestException("Incorrect OTP");

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower())
            ?? throw new NotFoundException("User not found");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        // Remove OTP from cache
        await cache.RemoveAsync($"otp:{dto.Email.ToLower()}");
    }

    // ── Helpers ──────────────────────────────────────────
    private AuthResponseDto BuildAuthResponse(User user)
    {
        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        db.SaveChanges();

        return new AuthResponseDto(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            User: new UserDto(
                user.Id,
                user.Name,
                user.Email,
                user.Phone,
                user.Role,
                user.Nationality,
                user.Dob,
                user.CreatedAt
            )
        );
    }
}