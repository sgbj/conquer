using System.Net;

namespace Conquer.Account.Models;

public class Server
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public IPAddress IpAddress { get; set; } = null!;
    public int Port { get; set; }
    public string Url { get; set; } = null!;
}