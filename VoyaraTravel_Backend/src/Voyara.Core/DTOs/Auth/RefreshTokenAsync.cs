using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Auth
{
    public record RefreshTokenDto(
    string AccessToken,
    string RefreshToken
);
}
