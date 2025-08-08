using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.ViewModels;
public class SuccessResponse<T>
{
    public int Status { get; set; } = StatusCodes.Status200OK;
    public string Title { get; set; } = "Success";
    public string? Detail { get; set; } = "Request processed successfully.";
    public T? Data { get; set; }

    public SuccessResponse(T? data, string? detail = null, string? title = null)
    {
        if (typeof(T) == typeof(string))
        {
            // If no explicit detail provided, treat string as message
            if (string.IsNullOrWhiteSpace(detail))
            {
                Detail = data?.ToString();
                Data = default;
            }
            else
            {
                Data = data;
                Detail = detail;
            }
        }
        else
        {
            Data = data;
            if (!string.IsNullOrWhiteSpace(detail))
                Detail = detail;
        }

        if (!string.IsNullOrWhiteSpace(title))
            Title = title;
    }


    //public SuccessResponse(T? data, string? detail = null, string? title = null)
    //{
    //    if (typeof(T) == typeof(string))
    //    {
    //        // For string, treat it as a detail message and keep Data null
    //        Detail = data?.ToString();
    //        Data = default;
    //    }
    //    else
    //    {
    //        Data = data;
    //        if (!string.IsNullOrWhiteSpace(detail))
    //            Detail = detail;
    //    }

    //    if (!string.IsNullOrWhiteSpace(title))
    //        Title = title;
    //}

    public static SuccessResponse<string> FromMessage(string message, string? title = null)
    {
        return new SuccessResponse<string>(message, null, title ?? "Success");
    }
}
