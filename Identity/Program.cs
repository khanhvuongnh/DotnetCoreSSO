using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddDataProtection()
    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect("127.0.0.1:6379"))
    .SetApplicationName("unique");
builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o => o.Cookie.Domain = ".company.local");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "hello world");
app.MapGet("/protected", () => "secret").RequireAuthorization();
app.MapGet("/login", (HttpContext ctx) =>
{
    ctx.SignInAsync(new ClaimsPrincipal(new[]
    {
        new ClaimsIdentity(new List<Claim>()
        {
            new Claim(ClaimTypes.Name, Guid.NewGuid().ToString())
        },
        CookieAuthenticationDefaults.AuthenticationScheme)
    }));

    return "ok";
});

app.Run();
