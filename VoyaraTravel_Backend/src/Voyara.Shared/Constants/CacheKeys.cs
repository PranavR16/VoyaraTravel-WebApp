using System;
using System.Collections.Generic;
using System.Text;

namespace Voyara.Shared.Constants
{
    public static class CacheKeys
    {
        public static string OtpKey(string email) => $"otp:{email.ToLower()}";
        public static string RateLimit(string ip) => $"ratelimit:{ip}";
        public static string UserSession(Guid id) => $"session:{id}";
    }
}
