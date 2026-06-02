using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Voyara.Core.DTOs.Shared;
using Voyara.Core.Entities;
using Voyara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Voyara.Shared.Exceptions;

namespace Voyara.API.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController(VoyaraDbContext db) : ControllerBase
    {
        // GET api/reviews/package/{packageId}
        [HttpGet("package/{packageId}")]
        public async Task<IActionResult> GetByPackage(Guid packageId)
        {
            var reviews = await db.Reviews
                .Include(r => r.User)
                .Where(r => r.PackageId == packageId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto(
                    r.Id,
                    r.Rating,
                    r.Comment,
                    r.User.Name,
                    r.CreatedAt))
                .ToListAsync();

            return Ok(reviews);
        }

        // POST api/reviews
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Check user already reviewed this package
            if (await db.Reviews.AnyAsync(r =>
                r.UserId == userId && r.PackageId == dto.PackageId))
                throw new BadRequestException("You have already reviewed this package");

            // Verify user has a confirmed booking for this package
            var hasBooking = await db.Bookings.AnyAsync(b =>
                b.UserId == userId &&
                b.PackageId == dto.PackageId &&
                b.Status == "Confirmed");

            if (!hasBooking)
                throw new BadRequestException(
                    "You can only review packages you have booked");

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new BadRequestException("Rating must be between 1 and 5");

            var review = new Review
            {
                Rating = dto.Rating,
                Comment = dto.Comment,
                UserId = userId,
                PackageId = dto.PackageId
            };

            await db.Reviews.AddAsync(review);

            // Update package average rating
            var pkg = await db.Packages.FindAsync(dto.PackageId);
            if (pkg != null)
            {
                var allRatings = await db.Reviews
                    .Where(r => r.PackageId == dto.PackageId)
                    .Select(r => r.Rating)
                    .ToListAsync();

                allRatings.Add(dto.Rating);
                pkg.Rating = Math.Round(allRatings.Average(), 1);
                pkg.ReviewCount = allRatings.Count;
            }

            await db.SaveChangesAsync();

            var user = await db.Users.FindAsync(userId);

            return CreatedAtAction(nameof(GetByPackage),
                new { packageId = dto.PackageId },
                new ReviewDto(review.Id, review.Rating,
                    review.Comment, user?.Name ?? "", review.CreatedAt));
        }

        // DELETE api/reviews/{id} — Admin or owner
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var review = await db.Reviews.FindAsync(id)
                ?? throw new NotFoundException("Review not found");

            if (review.UserId != userId && role != "Admin")
                throw new UnauthorizedException("Access denied");

            db.Reviews.Remove(review);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
