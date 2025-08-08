using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.ViewModels;
public static class Success
{
    // Data responses
    public static SuccessResponse<T> Ok<T>(T? data, string? detail = null, string? title = null)
        => new SuccessResponse<T>(data, detail, title);

    public static SuccessResponse<T> Created<T>(T? data, string? detail = "Record created successfully.", string? title = "Created")
        => new SuccessResponse<T>(data, detail, title);

    public static SuccessResponse<T> Update<T>(T? data, string? detail = "Record updated successfully.", string? title = "Update")
       => new SuccessResponse<T>(data, detail, title);

    public static SuccessResponse<T> Delete<T>(T? data, string? detail = "Record deleted successfully.", string? title = "Delete")
       => new SuccessResponse<T>(data, detail, title);


    public static SuccessResponse<T> Empty<T>(string? detail = null, string? title = null)
        => new SuccessResponse<T>(default, detail, title);

    // Message-only (string) responses — uses your FromMessage()
    public static SuccessResponse<string> Message(string message, string? title = null)
        => SuccessResponse<string>.FromMessage(message, title);

    // Paged responses
    public static SuccessResponse<PagedResult<T>> Paged<T>(
        IReadOnlyList<T> items, int page, int pageSize, int totalCount,
        string? detail = null, string? title = null)
        => new SuccessResponse<PagedResult<T>>(new PagedResult<T>(items, page, pageSize, totalCount), detail, title);

}

