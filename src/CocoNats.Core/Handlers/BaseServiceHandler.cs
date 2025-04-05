using System.Reflection;

using CocoNats.Abstractions.Attributes;
using CocoNats.Abstractions.Contracts;

using CocoNats.Abstractions.Services;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NATS.Client.Core;

namespace CocoNats.Core.Handlers;

/// <summary>
/// Base handler for NATS services that automatically handles subscription and reply logic.
/// </summary>
public abstract class BaseServiceHandler<TService>(
    ILogger<BaseServiceHandler<TService>> logger,
    INatsConnection connection,
    IServiceProvider serviceProvider
) : IHostedService, IAsyncDisposable
    where TService : class, INatsService
{
    private readonly ILogger<BaseServiceHandler<TService>> _logger = logger;
    private readonly INatsConnection _connection = connection;
    private readonly List<Task> _replyTask = [];
    private CancellationTokenSource? _cts;
    public IServiceProvider ServiceProvider { get; }= serviceProvider;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting service handler for {ServiceType}", typeof(TService).Name);
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cancellationToken = _cts.Token;

        using var scope = ServiceProvider.CreateScope();
        var serviceInstance = scope.ServiceProvider.GetRequiredService<TService>();
        var serviceType = serviceInstance.GetType();

        _logger.LogInformation("Service instance created: {ServiceType}", serviceType.Name);

        var methods = typeof(TService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName)
            .ToList();

        _logger.LogInformation("Found {MethodCount} methods in service type {ServiceType}", methods.Count, serviceType.Name);
        
        foreach (var method in methods)
        {
            var implMethod = serviceType.GetMethod(
                method.Name,
                method.GetParameters().Select(p => p.ParameterType).ToArray());
            
            if (implMethod == null)
            {
                _logger.LogWarning("Method {MethodName} not found in service type {ServiceType}", method.Name, serviceType.Name);
                continue;
            }

            var subjectAttr = implMethod.GetCustomAttribute<SubjectAttribute>();
            string subject;

            if (subjectAttr != null)
            {
                subject = subjectAttr.Subject;
                _logger.LogInformation("Using custom subject from attribute: {Subject}", subject);
            }
            else
            {
                subject = GetSubjectFromMethod(method);
                _logger.LogInformation("Using convention-based subject: {Subject}", subject);
            }
            
            // add check for duplicate subject and hint user to solve conflicts
            var task = CreateSubscriptionFromMethod(method, subject, cancellationToken);
            _replyTask.Add(task);
        }

        _logger.LogInformation("Service handler started for {ServiceType}", typeof(TService).Name);
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _cts?.Cancel();
        await Task.WhenAll(_replyTask);
        _cts?.Dispose();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping service handler for {ServiceType}", typeof(TService).Name);
        _cts?.Cancel();
        return Task.WhenAll(_replyTask);
    }

    private Task CreateSubscriptionFromMethod(MethodInfo method, string subject, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating subscription for subject: {Subject}", subject);
        var parameters = method.GetParameters();

        Type requestType;
        if (parameters.Length == 0)
        {
            requestType = typeof(Request);
        }
        else
        {
            requestType = parameters[0].ParameterType.IsGenericType && parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(NatsMsg<>)
                        ? parameters[0].ParameterType.GenericTypeArguments[0]
                        : parameters[0].ParameterType;
        }
        Type? responseType;
        if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            responseType = method.ReturnType.GetGenericArguments()[0];
        }
        else if (method.ReturnType == typeof(Task))
        {
            responseType = typeof(object);
        }
        else
        {
            responseType = method.ReturnType;
        }


        var subscribeMethod = typeof(BaseServiceHandler<TService>)
            .GetMethod(nameof(SubscribeToSubject), BindingFlags.NonPublic | BindingFlags.Instance)
            ?.MakeGenericMethod(requestType, responseType) ?? throw new InvalidOperationException($"Could not create subscription for method {method.Name}");

        return (Task)subscribeMethod.Invoke(this, [method, subject, cancellationToken])!;
    }

    private async Task SubscribeToSubject<TRequest, TResponse>(MethodInfo method, string subject, CancellationToken cancellationToken)
        where TRequest : class
    {
        var useNatsMsg = IsNatsMsgUsed(method);
        var hasParameters = method.GetParameters().Length > 0;

        await foreach (var msg in _connection.SubscribeAsync<TRequest>(subject, cancellationToken: cancellationToken))
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();
                var serviceInstance = scope.ServiceProvider.GetRequiredService<TService>();

                object?[] parameters;
                if (!hasParameters)
                {
                    parameters = [];
                }
                else if (useNatsMsg)
                {
                    parameters = [msg];
                }
                else if (msg.Data != null)
                {
                    parameters = [msg.Data];
                }
                else
                {
                    parameters = [Activator.CreateInstance<TRequest>()];
                }

                object? result = null;
                if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var task = (Task)method.Invoke(serviceInstance, parameters)!;
                    await task;

                    var resultProperty = task.GetType().GetProperty("Result");
                    result = resultProperty?.GetValue(task);
                }
                else if (method.ReturnType == typeof(Task))
                {
                    var task = (Task)method.Invoke(serviceInstance, parameters)!;
                    await task;
                }
                else
                {
                    result = method.Invoke(serviceInstance, parameters);
                }

                if (result != null)
                {
                    await msg.ReplyAsync((TResponse)result, cancellationToken: cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Method {Method} returned null for subject {Subject}", method.Name, subject);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message for subject {Subject}", subject);
            }
        }
    }

    public virtual string GetSubjectFromMethod(MethodInfo method)
    {
        var attr = method.GetCustomAttribute<SubjectAttribute>();
        var useNatsMsg = BaseServiceHandler<TService>.IsNatsMsgUsed(method);

        if (attr != null)
        {
            return attr.Subject;
        }

        var methodName = method.Name;
        if (methodName.EndsWith("Async")) methodName = methodName[..^5];

        if (methodName.StartsWith("Get"))
        {
            var resource = methodName[3..].ToLower();
            return methodName.EndsWith('s')
                ? $"{resource[..^1]}.get"
                : $"{resource}.get.*";
        }
        else if (methodName.StartsWith("Create"))
        {
            var resource = methodName[6..].ToLower();
            return $"{resource}.post";
        }
        else if (methodName.StartsWith("Update"))
        {
            var resource = methodName[6..].ToLower();
            return useNatsMsg
                ? $"{resource}.put.*"
                : $"{resource}.put";
        }
        else if (methodName.StartsWith("Delete"))
        {
            var resource = methodName[6..].ToLower();
            return useNatsMsg
                ? $"{resource}.delete.*"
                : $"{resource}.delete";
        }

        // If not a standard CRUD method, use method name directly with dots as separators
        return methodName.ToLower().Replace("_", ".");
    }

    private static bool IsNatsMsgUsed(MethodInfo method)
    {
        return method.GetParameters().Length > 0 &&
               method.GetParameters()[0].ParameterType.IsGenericType &&
               method.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(NatsMsg<>);
    }
}