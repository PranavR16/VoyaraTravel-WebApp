using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Core.DTOs.Shared;
using Voyara.Core.Entities;
using Voyara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Voyara.Shared.Exceptions;

namespace Voyara.API.Controllers
{
    public class CouponsController(VoyaraDbContext db) : ControllerBase
    {
        // POST api/coupons/validate
        [HttpPost("validate")]
        [Authorize]
        public async Task<IActionResult> Validate([FromBody] ValidateCouponDto dto)
        {
            var coupon = await db.Coupons.FirstOrDefaultAsync(c =>
                c.Code == dto.Code.ToUpper() &&
                c.IsActive &&
                c.ExpiresAt > DateTime.UtcNow &&
                c.UsedCount < c.MaxUses);

            if (coupon == null)
                return Ok(new ValidateCouponResponseDto(
                    false, 0, "Invalid or expired coupon code"));

            return Ok(new ValidateCouponResponseDto(
                true,
                coupon.DiscountPct,
                $"{coupon.DiscountPct}% discount applied!"));
        }

        // GET api/coupons — Admin only
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var coupons = await db.Coupons
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CouponDto(
                    c.Id, c.Code, c.DiscountPct,
                    c.MaxUses, c.UsedCount, c.ExpiresAt, c.IsActive))
                .ToListAsync();

            return Ok(coupons);
        }

        // POST api/coupons — Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCouponDto dto)
        {
            if (await db.Coupons.AnyAsync(c => c.Code == dto.Code.ToUpper()))
                throw new BadRequestException("Coupon code already exists");

            var coupon = new Coupon
            {
                Code = dto.Code.ToUpper(),
                DiscountPct = dto.DiscountPct,
                MaxUses = dto.MaxUses,
                ExpiresAt = dto.ExpiresAt,
                IsActive = true
            };

            await db.Coupons.AddAsync(coupon);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new CouponDto(
                coupon.Id, coupon.Code, coupon.DiscountPct,
                coupon.MaxUses, coupon.UsedCount, coupon.ExpiresAt, coupon.IsActive));
        }

        // DELETE api/coupons/{id} — Admin only
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var coupon = await db.Coupons.FindAsync(id)
                ?? throw new NotFoundException("Coupon not found");

            coupon.IsActive = false;
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
