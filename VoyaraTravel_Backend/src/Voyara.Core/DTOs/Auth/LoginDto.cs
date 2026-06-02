using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Auth
{
    public record LoginDto(
    string Email,
    string Password
);
}
