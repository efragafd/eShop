using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors;

/// <summary>
/// Logging Behavior is a class that add logging to MediatR pipeline avoid
/// repeating it en in each Handle method.
/// Also measure performance and log if request took more than 3 seconds.
/// You need to inject this class to program container in Program.cs
/// Another important note is that this class use IRequest instead of
/// ICommand or IQuery, which means that this behavior applies for both 
/// of them, given that they inherits from IRequest
/// </summary>
public class LoggingBehavior<TRequest, TResponse> 
    (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("[START] Handle request={Request} - Response={Response} - RequestData={Data}",
            typeof(TRequest).Name, typeof(TResponse).Name, request);

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();
        var timeTaken = timer.Elapsed;
        if (timeTaken.Seconds > 3) logger.LogWarning("[PERFORMANCE] The request {Request} took {TimeTaken}s",
            typeof(TRequest).Name, timeTaken.Seconds);

        logger.LogInformation("[END] Handled {Request} with {Response}",
            typeof(TRequest).Name, typeof(TResponse).Name);
        
        return response;
    }
}
