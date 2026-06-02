using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Packages
{
    public record PackageDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    decimal OldPrice,
    int Nights,
    string Category,
    string Badge,
    string BadgeClass,
    double Rating,
    int ReviewCount,
    int Discount,
    string Image,
    string Unit,
    List<string> Tags,
    List<string> Inclusions,
    Guid DestinationId,
    string DestinationName
);
}
