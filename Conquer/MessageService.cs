using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Conquer;

public class MessageService
{
    private static readonly MethodInfo GetRequiredServiceMethod =
        typeof(ServiceProviderServiceExtensions).GetMethod(nameof(ServiceProviderServiceExtensions.GetRequiredService),
            BindingFlags.Public | BindingFlags.Static, new[] { typeof(IServiceProvider) })!;

    private static readonly ParameterExpression ClientExpr = Expression.Parameter(typeof(Client));
    private static readonly ParameterExpression MessageExpr = Expression.Parameter(typeof(IMessage));
    private static readonly ParameterExpression ServiceProviderExpr = Expression.Parameter(typeof(IServiceProvider));
    private readonly ILogger<MessageService> _logger;

    private readonly Dictionary<MessageType, Func<IMessage>> _messageFactories = new();
    private readonly Dictionary<MessageType, Func<Client, IMessage, IServiceProvider, Task>> _messageHandlers = new();

    private readonly IServiceProvider _serviceProvider;

    public MessageService(IServiceProvider serviceProvider, ILogger<MessageService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var assembly = Assembly.GetEntryAssembly();

        if (assembly == null)
        {
            return;
        }

        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsAssignableTo(typeof(IMessage)) || !Enum.TryParse<MessageType>(type.Name, out var messageType))
            {
                continue;
            }

            _messageFactories[messageType] = Expression.Lambda<Func<IMessage>>(Expression.New(type)).Compile();

            var method = type.GetMethod("HandleAsync");

            if (method == null)
            {
                continue;
            }

            var parameters = method.GetParameters();
            var arguments = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                arguments[i] = parameterType.IsAssignableTo(typeof(Client))
                    ? Expression.Convert(ClientExpr, parameterType)
                    : Expression.Call(GetRequiredServiceMethod.MakeGenericMethod(parameterType),
                        ServiceProviderExpr);
            }

            var body = Expression.Call(Expression.Convert(MessageExpr, type), method, arguments);
            var lambda =
                Expression.Lambda<Func<Client, IMessage, IServiceProvider, Task>>(body, ClientExpr,
                    MessageExpr, ServiceProviderExpr);
            _messageHandlers[messageType] = lambda.Compile();
        }
    }

    public IMessage? Create(MessageType messageType)
    {
        if (!_messageFactories.TryGetValue(messageType, out var messageFactory))
        {
            _logger.LogWarning("Message not found for {MessageType}.", messageType);
            return null;
        }

        return messageFactory();
    }

    public async Task HandleAsync(Client client, IMessage message)
    {
        if (!_messageHandlers.TryGetValue(message.Type, out var messageHandler))
        {
            _logger.LogWarning("Message handler not found for {Message}.", message);
            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        await messageHandler(client, message, scope.ServiceProvider);
    }
}