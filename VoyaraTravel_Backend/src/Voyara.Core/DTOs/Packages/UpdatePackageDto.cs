using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Packages
{
    public record UpdatePackageDto(
    string? Name,
    string? Description,
    decimal? Price,
    decimal? OldPrice,
    int? Nights,
    string? Category,
    string? Badge,
    string? BadgeClass,
    int? Discount,
    string? Image,
    bool? IsActive,
    List<string>? Tags,
    List<string>? Inclusions
);
}
