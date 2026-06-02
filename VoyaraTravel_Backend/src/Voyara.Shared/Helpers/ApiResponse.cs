using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Voyara.Shared.Helpers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success")
            => new() { Success = true, Data = data, Message = message, StatusCode = 200 };

        public static ApiResponse<T> Fail(string message, int code = 400)
            => new() { Success = false, Message = message, StatusCode = code };
    }
}
