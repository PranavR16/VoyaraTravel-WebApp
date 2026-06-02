using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Auth
{
    public record UserDto(
     Guid Id,
     string Name,
     string Email,
     string Phone,
     string Role,
     string? Nationality,
     DateTime? Dob,
     DateTime CreatedAt
 );

}
