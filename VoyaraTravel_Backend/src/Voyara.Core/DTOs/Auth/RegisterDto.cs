using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Auth
{
    public record RegisterDto
    (
    string Name,
    string Email,
    string Phone,
    string Password,
    string? Nationality,
    DateTime? Dob
);
}
