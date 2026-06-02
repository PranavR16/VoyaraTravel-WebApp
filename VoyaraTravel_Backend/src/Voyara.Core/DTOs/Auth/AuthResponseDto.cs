using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Auth
{
    public record AuthResponseDto(
     string AccessToken,
     string RefreshToken,
     UserDto User
 );
}
