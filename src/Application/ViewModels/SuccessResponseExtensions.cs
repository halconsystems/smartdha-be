using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.ViewModels;
public static class SuccessResponseExtensions
{
    public static SuccessResponse<T> ToSuccess<T>(this T data, string? detail = null, string? title = null)
        => new SuccessResponse<T>(data, detail, title);

    public static SuccessResponse<string> ToMessage(this string message, string? title = null)
        => Success.Message(message, title);
}

