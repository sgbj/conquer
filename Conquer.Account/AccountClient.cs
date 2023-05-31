using Microsoft.AspNetCore.Connections;

namespace Conquer.Account;

public class AccountClient : Client
{
    public AccountClient(ConnectionContext connection) : base(connection)
    {
    }
}