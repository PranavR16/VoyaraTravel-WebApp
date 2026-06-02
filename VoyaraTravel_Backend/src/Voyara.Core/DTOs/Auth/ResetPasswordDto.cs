using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Auth
{
    public record ResetPasswordDto(
    string Email,
    string Otp,
    string NewPassword
);
}
