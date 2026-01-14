using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Models;
public record ApiResult<T>(bool Success, string? Message, T? Data)
{
    public static ApiResult<T> Ok(T data, string? message = null) => new(true, message, data);
    public static ApiResult<T> Fail(string message) => new(false, message, default);
}
