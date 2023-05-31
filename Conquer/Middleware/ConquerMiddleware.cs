using System.IO.Pipelines;
using Conquer.Cryptography;
using Conquer.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Conquer.Middleware;

public static class ConquerMiddlewareExtensions
{
    public static TBuilder UseConquer<TBuilder>(this TBuilder builder) where TBuilder : IConnectionBuilder
    {
        builder.Use(next => new ConquerMiddleware(next, builder.ApplicationServices).OnConnectionAsync);
        return builder;
    }
}

internal class ConquerMiddleware
{
    private readonly ConnectionDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public ConquerMiddleware(ConnectionDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task OnConnectionAsync(ConnectionContext connection)
    {
        var oldTransport = connection.Transport;

        try
        {
            var cipher = _serviceProvider.GetRequiredService<IConnectionCipher>();
            var protocol = _serviceProvider.GetRequiredService<IProtocol>();
            var logger = (ILogger)_serviceProvider.GetRequiredService<ILogger<ConquerMiddleware>>();

            await using var newTransport = new ConquerDuplexPipe(oldTransport, cipher, logger);

            connection.Transport = newTransport;

            connection.Features.Set(cipher);
            connection.Features.Set(protocol);
            connection.Features.Set(logger);

            await _next(connection);
        }
        finally
        {
            connection.Transport = oldTransport;
        }
    }
}

internal class ConquerDuplexPipe : IDuplexPipe, IAsyncDisposable
{
    private readonly IConnectionCipher _cipher;
    private readonly Task _decryptTask, _encryptTask;
    private readonly Pipe _input = new(), _output = new();
    private readonly ILogger _logger;
    private readonly IDuplexPipe _transport;

    public ConquerDuplexPipe(IDuplexPipe transport, IConnectionCipher cipher, ILogger logger)
    {
        _transport = transport;
        _cipher = cipher;
        _logger = logger;
        _decryptTask = DecryptAsync();
        _encryptTask = EncryptAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Input.CompleteAsync();
        await Output.CompleteAsync();
        await _decryptTask;
        await _encryptTask;
    }

    public PipeReader Input => _input.Reader;
    public PipeWriter Output => _output.Writer;

    private async Task DecryptAsync()
    {
        try
        {
            while (true)
            {
                var readResult = await _transport.Input.ReadAsync();
                var buffer = readResult.Buffer;

                foreach (var readOnlyMemory in buffer)
                {
                    var memory = _input.Writer.GetMemory(readOnlyMemory.Length);
                    _cipher.Decrypt(readOnlyMemory.Span, memory.Span);
                    _input.Writer.Advance(readOnlyMemory.Length);
                }

                _transport.Input.AdvanceTo(buffer.End);

                var flushResult = await _input.Writer.FlushAsync();

                if (flushResult.IsCanceled || flushResult.IsCompleted)
                {
                    break;
                }

                if (readResult.IsCanceled || readResult.IsCompleted)
                {
                    break;
                }
            }
        }
        catch (ConnectionResetException)
        {
            // Don't let connection reset exceptions out
        }
        catch (ConnectionAbortedException)
        {
            // Don't let connection aborted exceptions out
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception in {Method}.", nameof(DecryptAsync));
        }

        await _transport.Input.CompleteAsync();
        await _input.Writer.CompleteAsync();
    }

    private async Task EncryptAsync()
    {
        try
        {
            while (true)
            {
                var readResult = await _output.Reader.ReadAsync();
                var buffer = readResult.Buffer;

                foreach (var readOnlyMemory in buffer)
                {
                    var memory = _transport.Output.GetMemory(readOnlyMemory.Length);
                    _cipher.Encrypt(readOnlyMemory.Span, memory.Span);
                    _transport.Output.Advance(readOnlyMemory.Length);
                }

                _output.Reader.AdvanceTo(buffer.End);

                var flushResult = await _transport.Output.FlushAsync();

                if (flushResult.IsCanceled || flushResult.IsCompleted)
                {
                    break;
                }

                if (readResult.IsCanceled || readResult.IsCompleted)
                {
                    break;
                }
            }
        }
        catch (ConnectionResetException)
        {
            // Don't let connection reset exceptions out
        }
        catch (ConnectionAbortedException)
        {
            // Don't let connection aborted exceptions out
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception in {Method}.", nameof(EncryptAsync));
        }

        await _output.Reader.CompleteAsync();
        await _transport.Output.CompleteAsync();
    }
}