using Voyara.Core;
using Voyara.Core.DTOs.Packages;
using Voyara.Shared.Exceptions;
using Voyara.Core.Entities;

namespace Voyara.Infrastructure;

public class PackageService(IPackageRepository repo) : IPackageService
{
    public async Task<IEnumerable<PackageDto>> GetAllAsync(PackageFilterDto filter)
    {
        var packages = await repo.GetAllWithDestinationAsync(filter);
        return packages.Select(ToDto);
    }

    public async Task<PackageDto> GetByIdAsync(Guid id)
    {
        var pkg = await repo.GetByIdWithDetailsAsync(id)
            ?? throw new NotFoundException($"Package {id} not found");
        return ToDto(pkg);
    }

    public async Task<IEnumerable<PackageDto>> GetByCategoryAsync(string category)
    {
        var packages = await repo.GetByCategoryAsync(category);
        return packages.Select(ToDto);
    }

    public async Task<PackageDto> CreateAsync(CreatePackageDto dto)
    {
        var pkg = new Package
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            OldPrice = dto.OldPrice,
            Nights = dto.Nights,
            Category = dto.Category.ToLower(),
            Badge = dto.Badge,
            BadgeClass = dto.BadgeClass,
            Discount = dto.Discount,
            Image = dto.Image,
            Unit = dto.Unit,
            Tags = dto.Tags,
            Inclusions = dto.Inclusions,
            DestinationId = dto.DestinationId,
            IsActive = true
        };

        await repo.AddAsync(pkg);
        return await GetByIdAsync(pkg.Id);
    }

    public async Task<PackageDto> UpdateAsync(Guid id, UpdatePackageDto dto)
    {
        var pkg = await repo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Package {id} not found");

        if (dto.Name != null) pkg.Name = dto.Name;
        if (dto.Description != null) pkg.Description = dto.Description;
        if (dto.Price != null) pkg.Price = dto.Price.Value;
        if (dto.OldPrice != null) pkg.OldPrice = dto.OldPrice.Value;
        if (dto.Nights != null) pkg.Nights = dto.Nights.Value;
        if (dto.Category != null) pkg.Category = dto.Category.ToLower();
        if (dto.Badge != null) pkg.Badge = dto.Badge;
        if (dto.BadgeClass != null) pkg.BadgeClass = dto.BadgeClass;
        if (dto.Discount != null) pkg.Discount = dto.Discount.Value;
        if (dto.Image != null) pkg.Image = dto.Image;
        if (dto.IsActive != null) pkg.IsActive = dto.IsActive.Value;
        if (dto.Tags != null) pkg.Tags = dto.Tags;
        if (dto.Inclusions != null) pkg.Inclusions = dto.Inclusions;

        await repo.UpdateAsync(pkg);
        return await GetByIdAsync(pkg.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var pkg = await repo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Package {id} not found");

        // Soft delete
        pkg.IsActive = false;
        await repo.UpdateAsync(pkg);
    }

    // ── Mapper ───────────────────────────────────────────
    private static PackageDto ToDto(Package p) => new(
        p.Id,
        p.Name,
        p.Description,
        p.Price,
        p.OldPrice,
        p.Nights,
        p.Category,
        p.Badge,
        p.BadgeClass,
        p.Rating,
        p.ReviewCount,
        p.Discount,
        p.Image,
        p.Unit,
        p.Tags,
        p.Inclusions,
        p.DestinationId,
        p.Destination?.Name ?? string.Empty
    );
}
