using Voyara.Core.DTOs.Auth;

namespace Voyara.Core;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
    Task LogoutAsync(Guid userId);
    Task SendPasswordResetOtpAsync(string email);
    Task ResetPasswordAsync(ResetPasswordDto dto);
}
