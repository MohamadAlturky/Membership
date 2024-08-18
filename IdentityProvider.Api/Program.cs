using System.Security.Claims;
using IdentityProvider.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

app.UseCors(AllowSpecificOrigins);
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();


app.MapGet("/",() => "Auth Service Is Running 🔥🔥🔥-🔥-🔥🔥🔥");

app.MapGet("/user/", async(ClaimsPrincipal claimsPrincipals,UsersDataContext dbContext) => {
    var id = claimsPrincipals.Claims.First(c=>c.Type == ClaimTypes.NameIdentifier).Value;
    return await dbContext.Users.FindAsync(int.Parse(id));
}).RequireAuthorization();

app.MapGet("/userId/", (ClaimsPrincipal claimsPrincipals,UsersDataContext dbContext) => {
    var id = claimsPrincipals.Claims.First(c=>c.Type == ClaimTypes.NameIdentifier).Value;
    return int.Parse(id);
}).RequireAuthorization();


app.MapPost("/users", async ([FromBody]List<int> ids, UsersDataContext dbContext) =>
{
    if (ids == null || ids.Count == 0)
    {
        return Results.Ok(new List<User>(){});
    }

    var users = await dbContext.Users
        .Where(user => ids.Contains(user.Id))
        .Select(e=>new{e.Id,e.UserName,e.Email})
        .ToListAsync();

    if (!users.Any())
    {
        return Results.NotFound("No users found for the given IDs.");
    }

    return Results.Ok(users);
});
app.MapGet("/users", async ([FromQuery]string emailSubstring,ClaimsPrincipal claimsPrincipals, UsersDataContext dbContext) =>
{
    var id = claimsPrincipals.Claims.First(c=>c.Type == ClaimTypes.NameIdentifier).Value;
    
    var users = await dbContext.Users
        .Where(u => u.Email.Contains(emailSubstring))
        .Where(e=>e.Id!= int.Parse(id))
        .Select(e=>new{
            e.Id,
            e.Email,
            e.UserName
        })
        .ToListAsync();

    return Results.Ok(users);
});

app.MapReverseProxy();
app.Run();