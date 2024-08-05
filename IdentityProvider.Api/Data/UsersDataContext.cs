using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityProvider.Api.Data;

public class UsersDataContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public UsersDataContext(DbContextOptions<UsersDataContext> options) : base(options)
    {
    }
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseNpgsql("Host=localhost;Port=5477;Database=auth;Username=auth;Password=auth_db@1234");
    // }
}
