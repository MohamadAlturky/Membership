using IdentityProvider.Api.Data;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
var app = builder.Build();

app.MapIdentityApi<User>();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors(AllowSpecificOrigins);
app.MapGet("/",() => "Auth Service Is Running 🔥🔥");
app.Run();