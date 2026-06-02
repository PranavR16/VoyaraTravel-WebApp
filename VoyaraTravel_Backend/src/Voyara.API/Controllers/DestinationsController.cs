using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Core.DTOs.Shared;
using Voyara.Core.Entities;
using Voyara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Voyara.Shared.Exceptions;

namespace Voyara.API.Controllers
{
        [ApiController]
        [Route("api/destinations")]
        public class DestinationsController(VoyaraDbContext db) : ControllerBase
        {
            // GET api/destinations
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var destinations = await db.Destinations
                    .Where(d => d.IsActive)
                    .Include(d => d.Packages)
                    .OrderBy(d => d.Name)
                    .Select(d => new DestinationDto(
                        d.Id,
                        d.Name,
                        d.Country,
                        d.Image,
                        d.StartingPrice,
                        d.Description,
                        d.Packages.Count(p => p.IsActive)))
                    .ToListAsync();

                return Ok(destinations);
            }

            // GET api/destinations/{id}
            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(Guid id)
            {
                var d = await db.Destinations
                    .Include(d => d.Packages)
                    .FirstOrDefaultAsync(d => d.Id == id && d.IsActive)
                    ?? throw new NotFoundException("Destination not found");

                return Ok(new DestinationDto(
                    d.Id, d.Name, d.Country, d.Image,
                    d.StartingPrice, d.Description,
                    d.Packages.Count(p => p.IsActive)));
            }

            // POST api/destinations — Admin only
            [HttpPost]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Create([FromBody] CreateDestinationDto dto)
            {
                var dest = new Destination
                {
                    Name = dto.Name,
                    Country = dto.Country,
                    Image = dto.Image,
                    StartingPrice = dto.StartingPrice,
                    Description = dto.Description,
                    IsActive = true
                };

                await db.Destinations.AddAsync(dest);
                await db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = dest.Id },
                    new DestinationDto(dest.Id, dest.Name, dest.Country,
                        dest.Image, dest.StartingPrice, dest.Description, 0));
            }

            // PATCH api/destinations/{id} — Admin only
            [HttpPatch("{id}")]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Update(Guid id,
                [FromBody] CreateDestinationDto dto)
            {
                var dest = await db.Destinations.FindAsync(id)
                    ?? throw new NotFoundException("Destination not found");

                dest.Name = dto.Name;
                dest.Country = dto.Country;
                dest.Image = dto.Image;
                dest.StartingPrice = dto.StartingPrice;
                dest.Description = dto.Description;

                await db.SaveChangesAsync();
                return Ok(new DestinationDto(dest.Id, dest.Name, dest.Country,
                    dest.Image, dest.StartingPrice, dest.Description, 0));
            }

            // DELETE api/destinations/{id} — Admin only
            [HttpDelete("{id}")]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Delete(Guid id)
            {
                var dest = await db.Destinations.FindAsync(id)
                    ?? throw new NotFoundException("Destination not found");

                dest.IsActive = false;
                await db.SaveChangesAsync();
                return NoContent();
            }
        }
    }
