using Microsoft.AspNetCore.Identity;

namespace Conquer.Account.Models;

public class User : IdentityUser
{
    public bool IsBanned { get; set; }
}