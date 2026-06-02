using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Shared
{
    public record UpdateUserDto(
     string? Name,
     string? Phone,
     string? Nationality,
     string? Passport,
     DateTime? Dob
 );
}
