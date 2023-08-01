using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddDataProtection()
    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect("203.113.151.196:6379"))
    .SetApplicationName("unique");
builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", (HttpContext ctx) =>
{
    var ct = ctx;
    return ct.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
});
app.MapGet("/protected", (HttpContext ctx) => {
    var ct = ctx;
    return ct.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
}).RequireAuthorization();

app.Run();
