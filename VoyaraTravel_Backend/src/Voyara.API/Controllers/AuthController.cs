using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.API.Extensions;
using Voyara.Core;
using Voyara.Core.DTOs.Auth;

namespace Voyara.API.Controllers
{

    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        // POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await authService.RegisterAsync(dto);
            return CreatedAtAction(nameof(Register), result);
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await authService.LoginAsync(dto);
            return Ok(result);
        }

        // POST api/auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            var result = await authService.RefreshTokenAsync(dto);
            return Ok(result);
        }

        // POST api/auth/logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await authService.LogoutAsync(User.GetUserId());
            return Ok(new { message = "Logged out successfully" });
        }

        // POST api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordDto dto)
        {
            await authService.SendPasswordResetOtpAsync(dto.Email);
            return Ok(new { message = "OTP sent to your email address" });
        }

        // POST api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDto dto)
        {
            await authService.ResetPasswordAsync(dto);
            return Ok(new { message = "Password reset successfully" });
        }
    }

    //[ApiController]
    //[Route("api/auth")]
    //public class AuthController(IAuthService authService) : ControllerBase
    //{
    //    [HttpPost("register")]
    //    public async Task<IActionResult> Register(RegisterDto dto)
    //        => Ok(await authService.RegisterAsync(dto));

    //    [HttpPost("login")]
    //    public async Task<IActionResult> Login(LoginDto dto)
    //        => Ok(await authService.LoginAsync(dto));

    //    [HttpPost("refresh")]
    //    public async Task<IActionResult> Refresh(RefreshTokenDto dto)
    //        => Ok(await authService.RefreshTokenAsync(dto));

    //    [HttpPost("logout")]
    //    [Authorize]
    //    //public async Task<IActionResult> Logout()
    //    //{
    //    //    await authService.LogoutAsync(User.GetUserId());
    //    //    return NoContent();
    //    //}

    //    [HttpPost("forgot-password")]
    //    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    //    {
    //        await authService.SendPasswordResetOtpAsync(dto.Email);
    //        return Ok(new { message = "OTP sent to your email" });
    //    }

    //    [HttpPost("reset-password")]
    //    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    //    {
    //        await authService.ResetPasswordAsync(dto);
    //        return Ok(new { message = "Password reset successfully" });
    //    }
}
