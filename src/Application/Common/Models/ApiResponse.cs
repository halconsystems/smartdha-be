using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Success(T data, string message = "")
    {
        return new ApiResponse<T>
        {
            Succeeded = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> Failure(string message)
    {
        return new ApiResponse<T>
        {
            Succeeded = false,
            Message = message
        };
    }
}
