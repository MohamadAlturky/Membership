using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityProvider.Api.Data;

public class UsersDataContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public UsersDataContext(DbContextOptions<UsersDataContext> options) : base(options)
    {
    }
}
