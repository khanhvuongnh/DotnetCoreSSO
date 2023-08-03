using App;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;
//using System.Web.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"c:\SSOApp"))
    //.PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect("203.113.151.196:6379"))
    .SetApplicationName("SharedCookieApp");
builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
    {
        o.Cookie.Domain = ".company.local";
        o.Cookie.Name = "Identity.Application";
        o.LoginPath = "/UnAuthenticate";
    });

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
//app.UseMiddleware<UnauthorizedRedirectMiddleware>();

app.MapGet("/", (HttpContext ctx) =>
{
    //var b = CookieSecurityProvider.Decrypt("");
   return "You are in App";
});
//app.MapGet("/DecodeCookie", (HttpContext ctx) =>
//{
//    string cookieName = "Identity.Application"; // Replace this with your actual cookie name
//    string decodedValue = string.Empty;

//    if (ctx.Request.Cookies.TryGetValue(cookieName, out string encodedCookieValue))
//    {
//        // Decode the cookie value from Base64 to its original content
//        byte[] bytes = Convert.FromBase64String(encodedCookieValue);
//        decodedValue = Encoding.UTF8.GetString(bytes);

//        // Now you can use the decoded value as needed
//        // For example, you can return it or store it in a variable
//    }
//    return decodedValue;
//});
app.MapGet("/protected", (HttpContext ctx) =>
{
    var ct = ctx;
    return ct.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
}).RequireAuthorization();

app.Run();
