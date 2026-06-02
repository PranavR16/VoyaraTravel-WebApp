using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Packages
{
    public record PackageFilterDto(
     string? Category = null,
     string? SortBy = "popular",   // popular|low|high|rating|duration|discount
     string? Search = null,
     int Page = 1,
     int PageSize = 12
 );
}
