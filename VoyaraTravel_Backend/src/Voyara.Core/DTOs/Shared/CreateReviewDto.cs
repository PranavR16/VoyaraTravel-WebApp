using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Shared
{
    public record CreateReviewDto(
    Guid PackageId,
    int Rating,
    string Comment
);
}
