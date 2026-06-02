using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Voyara.Core.DTOs.Auth;
using Voyara.Core.DTOs.Shared;
using Voyara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Voyara.Shared.Exceptions;

namespace Voyara.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController(VoyaraDbContext db) : ControllerBase
    {
        // GET api/users/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await db.Users.FindAsync(userId)
                ?? throw new NotFoundException("User not found");

            return Ok(new UserDto(
                user.Id, user.Name, user.Email, user.Phone,
                user.Role, user.Nationality, user.Dob, user.CreatedAt));
        }

        // PATCH api/users/me
        [HttpPatch("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await db.Users.FindAsync(userId)
                ?? throw new NotFoundException("User not found");

            if (dto.Name != null) user.Name = dto.Name;
            if (dto.Phone != null) user.Phone = dto.Phone;
            if (dto.Nationality != null) user.Nationality = dto.Nationality;
            if (dto.Passport != null) user.Passport = dto.Passport;
            if (dto.Dob != null) user.Dob = dto.Dob;

            user.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Ok(new UserDto(
                user.Id, user.Name, user.Email, user.Phone,
                user.Role, user.Nationality, user.Dob, user.CreatedAt));
        }

        // POST api/users/me/change-password
        [HttpPost("me/change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await db.Users.FindAsync(userId)
                ?? throw new NotFoundException("User not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new BadRequestException("Current password is incorrect");

            if (dto.NewPassword.Length < 8)
                throw new BadRequestException(
                    "New password must be at least 8 characters");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully" });
        }

        // DELETE api/users/me
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await db.Users.FindAsync(userId)
                ?? throw new NotFoundException("User not found");

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return NoContent();
        }

        // GET api/users — Admin only
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await db.Users
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserDto(
                    u.Id, u.Name, u.Email, u.Phone,
                    u.Role, u.Nationality, u.Dob, u.CreatedAt))
                .ToListAsync();

            return Ok(users);
        }
    }
}
