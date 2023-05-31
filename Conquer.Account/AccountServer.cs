using Microsoft.AspNetCore.Connections;

namespace Conquer.Account;

public class AccountServer : ConnectionHandler
{
    private readonly MessageService _messageService;

    public AccountServer(MessageService messageService) => _messageService = messageService;

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        await using var client = new AccountClient(connection);

        while (true)
        {
            var message = await client.ReadAsync();

            if (message is null)
            {
                break;
            }

            await _messageService.HandleAsync(client, message);
        }
    }
}