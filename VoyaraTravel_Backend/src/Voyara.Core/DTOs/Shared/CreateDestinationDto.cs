using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Shared
{
    public record CreateDestinationDto(
     string Name,
     string Country,
     string Image,
     decimal StartingPrice,
     string? Description
 );
}
