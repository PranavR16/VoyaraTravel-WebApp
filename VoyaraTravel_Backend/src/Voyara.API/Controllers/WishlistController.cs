
using global::Voyara.Core.DTOs.Shared;
using global::Voyara.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyara.API.Extensions;
using Voyara.Core.Entities;
using Voyara.Shared.Exceptions;
namespace Voyara.API.Controllers
{

    [ApiController]
    [Route("api/wishlist")]
    [Authorize]
    public class WishlistController(VoyaraDbContext db) : ControllerBase
    {
        // GET api/wishlist
        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userId = User.GetUserId();

            var items = await db.Wishlists
                .Include(w => w.Package)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .Select(w => new WishlistDto(
                    w.Id,
                    w.PackageId,
                    w.Package.Name,
                    w.Package.Image,
                    w.Package.Price,
                    w.CreatedAt))
                .ToListAsync();

            return Ok(items);
        }

        // POST api/wishlist/{packageId}
        [HttpPost("{packageId}")]
        public async Task<IActionResult> Add(Guid packageId)
        {
            var userId = User.GetUserId();

            // Check already in wishlist
            if (await db.Wishlists.AnyAsync(w =>
                w.UserId == userId && w.PackageId == packageId))
                throw new BadRequestException("Package is already in your wishlist");

            // Check package exists
            if (!await db.Packages.AnyAsync(p => p.Id == packageId && p.IsActive))
                throw new NotFoundException("Package not found");

            var item = new Wishlist
            {
                UserId = userId,
                PackageId = packageId
            };

            await db.Wishlists.AddAsync(item);
            await db.SaveChangesAsync();

            return Ok(new { message = "Added to wishlist ❤" });
        }

        // DELETE api/wishlist/{packageId}
        [HttpDelete("{packageId}")]
        public async Task<IActionResult> Remove(Guid packageId)
        {
            var userId = User.GetUserId();

            var item = await db.Wishlists.FirstOrDefaultAsync(w =>
                w.UserId == userId && w.PackageId == packageId)
                ?? throw new NotFoundException("Item not found in wishlist");

            db.Wishlists.Remove(item);
            await db.SaveChangesAsync();

            return NoContent();
        }

        // GET api/wishlist/check/{packageId}
        [HttpGet("check/{packageId}")]
        public async Task<IActionResult> Check(Guid packageId)
        {
            var userId = User.GetUserId();

            var exists = await db.Wishlists.AnyAsync(w =>
                w.UserId == userId && w.PackageId == packageId);

            return Ok(new { isWishlisted = exists });
        }

        //[ApiController]
        //[Route("api/wishlist")]
        //[Authorize]
        //public class WishlistController(VoyaraDbContext db) : ControllerBase
        //{
        //    [HttpGet]
        //    public async Task<IActionResult> GetMyWishlist()
        //    {
        //        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        //        var items = await db.Wishlists
        //            .Include(w => w.Package)
        //            .Where(w => w.UserId == userId)
        //            .OrderByDescending(w => w.CreatedAt)
        //            .Select(w => new WishlistDto(
        //                w.Id, w.PackageId, w.Package.Name,
        //                w.Package.Image, w.Package.Price, w.CreatedAt))
        //            .ToListAsync();
        //        return Ok(items);
        //    }

        //    [HttpPost("{packageId}")]
        //    public async Task<IActionResult> Add(Guid packageId)
        //    {
        //        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        //        if (await db.Wishlists.AnyAsync(w => w.UserId == userId && w.PackageId == packageId))
        //            throw new BadRequestException("Already in wishlist");
        //        if (!await db.Packages.AnyAsync(p => p.Id == packageId && p.IsActive))
        //            throw new NotFoundException("Package not found");
        //        await db.Wishlists.AddAsync(new Wishlist { UserId = userId, PackageId = packageId });
        //        await db.SaveChangesAsync();
        //        return Ok(new { message = "Added to wishlist" });
        //    }

        //    [HttpDelete("{packageId}")]
        //    public async Task<IActionResult> Remove(Guid packageId)
        //    {
        //        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        //        var item = await db.Wishlists.FirstOrDefaultAsync(w =>
        //            w.UserId == userId && w.PackageId == packageId)
        //            ?? throw new NotFoundException("Item not in wishlist");
        //        db.Wishlists.Remove(item);
        //        await db.SaveChangesAsync();
        //        return NoContent();
        //    }
        //}
    }
}
