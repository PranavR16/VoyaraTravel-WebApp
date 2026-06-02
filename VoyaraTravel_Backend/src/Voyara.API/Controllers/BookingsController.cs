using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Core;
using Voyara.Core.DTOs.Bookings;
using Voyara.Core.DTOs.Shared;
using System.Security.Claims;
using Voyara.API.Extensions;

namespace Voyara.API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingsController(IBookingService bookingService) : ControllerBase
    {
        // POST api/bookings
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
        {
            var result = await bookingService.CreateAsync(User.GetUserId(), dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // GET api/bookings
        [HttpGet]
        public async Task<IActionResult> GetMyBookings()
        {
            var result = await bookingService.GetByUserAsync(User.GetUserId());
            return Ok(result);
        }

        // GET api/bookings/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await bookingService.GetByIdAsync(id, User.GetUserId());
            return Ok(result);
        }

        // PATCH api/bookings/{id}/cancel
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await bookingService.CancelAsync(id, User.GetUserId());
            return Ok(new { message = "Booking cancelled successfully" });
        }

        // POST api/bookings/validate-coupon
        [HttpPost("validate-coupon")]
        public async Task<IActionResult> ValidateCoupon(
            [FromBody] ValidateCouponDto dto)
        {
            var isValid = await bookingService.ValidateCouponAsync(
                dto.Code, dto.Subtotal);

            return Ok(new
            {
                isValid,
                message = isValid
                    ? "Coupon is valid"
                    : "Invalid or expired coupon"
            });
        }
    }

        //[ApiController]
        //[Route("api/bookings")]
        //[Authorize]
        //public class BookingsController(IBookingService bookingService) : ControllerBase
        //{
        //    [HttpPost]
        //    public async Task<IActionResult> Create(CreateBookingDto dto)
        //        => Ok(await bookingService.CreateAsync(User.GetUserId(), dto));

        //    [HttpGet]
        //    public async Task<IActionResult> GetMyBookings()
        //        => Ok(await bookingService.GetByUserAsync(User.GetUserId()));

        //    [HttpGet("{id}")]
        //    public async Task<IActionResult> GetById(Guid id)
        //        => Ok(await bookingService.GetByIdAsync(id, User.GetUserId()));

        //    [HttpPatch("{id}/cancel")]
        //    public async Task<IActionResult> Cancel(Guid id)
        //    {
        //        await bookingService.CancelAsync(id, User.GetUserId());
        //        return Ok(new { message = "Booking cancelled successfully" });
        //    }
        //}

    }
