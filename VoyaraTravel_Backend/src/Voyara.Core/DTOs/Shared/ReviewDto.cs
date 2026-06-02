using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Shared
{
    public record ReviewDto(
    Guid Id,
    int Rating,
    string Comment,
    string UserName,
    DateTime CreatedAt
);
}
