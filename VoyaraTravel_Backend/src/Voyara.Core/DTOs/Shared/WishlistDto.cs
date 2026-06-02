using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Shared
{
    public record WishlistDto(
    Guid Id,
    Guid PackageId,
    string PackageName,
    string PackageImage,
    decimal PackagePrice,
    DateTime CreatedAt
);
}
