using Voyara.Core.DTOs.Packages;

namespace Voyara.Core;

public interface IPackageService
{
    Task<IEnumerable<PackageDto>> GetAllAsync(PackageFilterDto filter);
    Task<PackageDto> GetByIdAsync(Guid id);
    Task<IEnumerable<PackageDto>> GetByCategoryAsync(string category);
    Task<PackageDto> CreateAsync(CreatePackageDto dto);
    Task<PackageDto> UpdateAsync(Guid id, UpdatePackageDto dto);
    Task DeleteAsync(Guid id);
}

