using System.Net;
using Conquer.Cryptography;
using Conquer.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Conquer;

public abstract class Client : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ConnectionContext _connection;
    private readonly ILogger _logger;
    private readonly IProtocol _protocol;
    private readonly SemaphoreSlim _semaphore = new(1);

    protected Client(ConnectionContext connection)
    {
        _connection = connection;
        _protocol = connection.Features.Get<IProtocol>() ?? throw new InvalidOperationException("Protocol not found.");
        _logger = connection.Features.Get<ILogger>() ?? throw new InvalidOperationException("Logger not found.");
        ConnectionCipher = connection.Features.Get<IConnectionCipher>() ??
                           throw new InvalidOperationException("Connection cipher not found.");
    }

    public EndPoint RemoteEndPoint => _connection.RemoteEndPoint!;
    public IConnectionCipher ConnectionCipher { get; }

    public ValueTask DisposeAsync()
    {
        _semaphore.Dispose();
        _cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
        return default;
    }

    public async ValueTask<IMessage?> ReadAsync()
    {
        try
        {
            _cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(10));

            while (true)
            {
                var result = await _connection.Transport.Input.ReadAsync(_cancellationTokenSource.Token);
                var buffer = result.Buffer;

                var consumed = buffer.Start;
                var examined = buffer.End;

                try
                {
                    if (_protocol.TryParseMessage(result.Buffer, ref consumed, ref examined, out var message))
                    {
                        _logger.LogDebug("Received {Message} from {Client}.", message, this);
                        return message;
                    }
                }
                finally
                {
                    _connection.Transport.Input.AdvanceTo(consumed, examined);
                }

                if (result.IsCompleted)
                {
                    break;
                }
            }
        }
        catch (OperationCanceledException ex)
        {
            _connection.Abort(new("Client timeout exceeded.", ex));
        }

        return null;
    }

    public async ValueTask WriteAsync(IMessage message)
    {
        await _semaphore.WaitAsync();

        try
        {
            var size = message.Size;
            var buffer = _connection.Transport.Output.GetMemory(size);
            buffer.Span.Clear();
            message.Write(buffer.Span);
            _connection.Transport.Output.Advance(size);
            await _connection.Transport.Output.FlushAsync();
        }
        finally
        {
            _semaphore.Release();
        }

        _logger.LogDebug("Sent {Message} to {Client}.", message, this);
    }

    public void Abort() => _connection.Abort();

    public override string? ToString() => RemoteEndPoint.ToString();
}