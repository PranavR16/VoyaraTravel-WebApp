using Microsoft.EntityFrameworkCore;
using Voyara.Core;
using Voyara.Infrastructure.Data;
using Voyara.Core.Entities;
namespace Voyara.Infrastructure;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId);
    Task<Booking?> GetByIdWithDetailsAsync(Guid bookingId);
    Task<Booking?> GetByRefAsync(string bookingRef);
}

public class BookingRepository(VoyaraDbContext context)
    : GenericRepository<Booking>(context), IBookingRepository
{
    public async Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId)
        => await _context.Bookings
            .Include(b => b.Package)
            .Include(b => b.Travelers)
            .Include(b => b.Addons)
            .Include(b => b.Payment)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

    public async Task<Booking?> GetByIdWithDetailsAsync(Guid bookingId)
        => await _context.Bookings
            .Include(b => b.Package)
                .ThenInclude(p => p.Destination)
            .Include(b => b.User)
            .Include(b => b.Travelers)
            .Include(b => b.Addons)
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

    public async Task<Booking?> GetByRefAsync(string bookingRef)
        => await _context.Bookings
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.BookingRef == bookingRef);
}