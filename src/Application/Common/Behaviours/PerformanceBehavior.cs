using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DHAFacilitationAPIs.Application.Common.Behaviours;
public sealed class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request,RequestHandlerDelegate<TResponse> next,CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        var traceId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "N/A";
        var requestName = typeof(TRequest).Name;
        var elapsed = stopwatch.ElapsedMilliseconds;

        _logger.LogInformation(
        $"""
        [CQRS]
        TraceId  : {traceId}
        Request  : {requestName}
        Duration : {elapsed} ms
        """);

        return response;
    }
}

