using Microsoft.EntityFrameworkCore;
using Voyara.Core;
using Voyara.Core.DTOs.Packages;
using Voyara.Infrastructure.Data;
using Voyara.Core.Entities;
namespace Voyara.Infrastructure;

public interface IPackageRepository : IGenericRepository<Package>
{
    Task<IEnumerable<Package>> GetAllWithDestinationAsync(PackageFilterDto filter);
    Task<Package?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Package>> GetByCategoryAsync(string category);
}

public class PackageRepository(VoyaraDbContext context)
    : GenericRepository<Package>(context), IPackageRepository
{
    public async Task<IEnumerable<Package>> GetAllWithDestinationAsync(
        PackageFilterDto filter)
    {
        var query = _context.Packages
            .Include(p => p.Destination)
            .Where(p => p.IsActive);

        // Filter by category
        if (!string.IsNullOrWhiteSpace(filter.Category) &&
            filter.Category.ToLower() != "all")
            query = query.Where(p =>
                p.Category.ToLower() == filter.Category.ToLower());

        // Search by name
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(p =>
                p.Name.Contains(filter.Search) ||
                p.Description.Contains(filter.Search));

        // Sort
        query = filter.SortBy?.ToLower() switch
        {
            "low" => query.OrderBy(p => p.Price),
            "high" => query.OrderByDescending(p => p.Price),
            "rating" => query.OrderByDescending(p => p.Rating),
            "duration" => query.OrderByDescending(p => p.Nights),
            "discount" => query.OrderByDescending(p => p.Discount),
            _ => query.OrderByDescending(p => p.ReviewCount) // popular
        };

        // Paginate
        return await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
    }

    public async Task<Package?> GetByIdWithDetailsAsync(Guid id)
        => await _context.Packages
            .Include(p => p.Destination)
            .Include(p => p.ReviewList)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

    public async Task<IEnumerable<Package>> GetByCategoryAsync(string category)
        => await _context.Packages
            .Include(p => p.Destination)
            .Where(p => p.IsActive &&
                        p.Category.ToLower() == category.ToLower())
            .ToListAsync();
}