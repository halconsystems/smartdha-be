using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MobileAPI.Middlewares;

public sealed class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var traceId = context.TraceIdentifier;
        var stopwatch = Stopwatch.StartNew();

        string requestBody = "[NOT READ]";
        string responseText = "[NOT READ]";

        // ---------- REQUEST ----------
        if (context.Request.ContentLength > 0 &&
            context.Request.Body.CanRead)
        {
            context.Request.EnableBuffering();
            requestBody = await ReadStreamAsync(context.Request.Body);
            context.Request.Body.Position = 0;
            requestBody = MaskSensitive(requestBody);
        }

        // ---------- RESPONSE ----------
        var originalBody = context.Response.Body;

        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            stopwatch.Stop();

            // Read response ONLY if stream still open
            if (responseBody.CanRead)
            {
                responseText = await ReadStreamAsync(responseBody);
                responseText = MaskSensitive(responseText);
            }
        }
        catch (Exception)
        {
            stopwatch.Stop();
            responseText = "[EXCEPTION THROWN]";
            throw; //  DO NOT swallow exception
        }
        finally
        {
            // ALWAYS restore the original stream
            context.Response.Body = originalBody;

            // Copy only if possible
            if (responseBody.Length > 0)
            {
                responseBody.Position = 0;
                await responseBody.CopyToAsync(originalBody);
            }

            // ---------- LOG ----------
            _logger.LogInformation(
            $"""
            ================ HTTP REQUEST =================
            TraceId  : {traceId}
            Method   : {context.Request.Method}
            Path     : {context.Request.Path}
            Status   : {context.Response.StatusCode}
            Duration : {stopwatch.ElapsedMilliseconds} ms
            IP       : {context.Connection.RemoteIpAddress}
            
            Request:
            {(string.IsNullOrWhiteSpace(requestBody) ? "[EMPTY]" : requestBody)}
            
            Response:
            {(string.IsNullOrWhiteSpace(responseText) ? "[EMPTY]" : responseText)}
            ==============================================
            """);
        }
    }
    private static async Task<string> ReadStreamAsync(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
    private static string MaskSensitive(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return body;

        // Mask common sensitive fields in JSON (supports spaces + pretty formatting)
        body = Regex.Replace(body,
            "(\"password\"\\s*:\\s*\")([^\"]*)(\")",
            "$1***$3",
            RegexOptions.IgnoreCase);

        body = Regex.Replace(body,
            "(\"cnic\"\\s*:\\s*\")([^\"]*)(\")",
            "$1***$3",
            RegexOptions.IgnoreCase);

        body = Regex.Replace(body,
            "(\"accessToken\"\\s*:\\s*\")([^\"]*)(\")",
            "$1***$3",
            RegexOptions.IgnoreCase);

        body = Regex.Replace(body,
            "(\"refreshToken\"\\s*:\\s*\")([^\"]*)(\")",
            "$1***$3",
            RegexOptions.IgnoreCase);

        body = Regex.Replace(body,
            "(\"token\"\\s*:\\s*\")([^\"]*)(\")",
            "$1***$3",
            RegexOptions.IgnoreCase);

        body = Regex.Replace(body,
            "(\"mobileNumber\"\\s*:\\s*\")([^\"]*)(\")",
            "$1***$3",
            RegexOptions.IgnoreCase);

        return body;
    }
}
