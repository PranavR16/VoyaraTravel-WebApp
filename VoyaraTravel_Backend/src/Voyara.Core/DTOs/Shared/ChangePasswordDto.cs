using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Core.DTOs.Shared
{
    public record ChangePasswordDto(
     string CurrentPassword,
     string NewPassword
 );
}
