using System.Diagnostics;
using BookLibraryAPI.Core.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookLibraryAPI.Application.Common.Behaviors;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    private static readonly ActivitySource ActivitySource = new("BookLab.Application");
    private static readonly string ModuleName = GetModuleName(typeof(TRequest).FullName!);
    private static readonly string RequestName = typeof(TRequest).Name;
    private static readonly string ActivityName = $"{ModuleName}.{RequestName}";

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!logger.IsEnabled(LogLevel.Information))
        {
            return await next();
        }

        using var activity = ActivitySource.StartActivity(ActivityName);
        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["Module"] = ModuleName,
            ["Request"] = RequestName
        });

        var stopwatch = Stopwatch.StartNew();
        
        logger.LogInformation("Processing {RequestName}", RequestName);

        try
        {
            var response = await next();
            
            stopwatch.Stop();
            
            logger.LogInformation(
                "Completed {RequestName} in {ElapsedMs}ms with result {IsSuccess}",
                RequestName, stopwatch.ElapsedMilliseconds, response.IsSuccess);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, 
                "Failed {RequestName} in {ElapsedMs}ms", 
                RequestName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    private static string GetModuleName(string requestName) => requestName.Split('.')[2];
}
