using System.Security.Claims;
using IdentityProvider.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Authentication
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorization();
#endregion

#region DbContext
builder.Services.AddDbContext<UsersDataContext>(config =>
{
    config.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
#endregion

#region Identity
builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<UsersDataContext>()
    .AddApiEndpoints();
#endregion


#region Cors
var AllowSpecificOrigins = "_AllowFrontEnd";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(["http://localhost:3000","http://172.29.3.110:3000","http://bpmn.hiast.edu.sy"])
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                      });
});
#endregion

#region Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

#endregion
var app = builder.Build();

app.MapIdentityApi<User>();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(AllowSpecificOrigins);

app.MapGet("/",() => "Auth Service Is Running 🔥🔥");

app.MapGet("/user/", async(ClaimsPrincipal claimsPrincipals,UsersDataContext dbContext) => {
    var id = claimsPrincipals.Claims.First(c=>c.Type == ClaimTypes.NameIdentifier).Value;
    return await dbContext.Users.FindAsync(int.Parse(id));
}).RequireAuthorization();

app.MapGet("/userId/", (ClaimsPrincipal claimsPrincipals,UsersDataContext dbContext) => {
    var id = claimsPrincipals.Claims.First(c=>c.Type == ClaimTypes.NameIdentifier).Value;
    return int.Parse(id);
}).RequireAuthorization();

app.MapReverseProxy();
app.Run();