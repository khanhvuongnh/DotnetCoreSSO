using System.Collections;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
    //.PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect("203.113.151.196:6379"))
    .PersistKeysToFileSystem(new DirectoryInfo(@"c:\SSO"))
    .SetApplicationName("SharedCookieApp");
builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
    {
        o.Cookie.Domain = ".company.local";
        o.Cookie.Name = "Identity.Application";
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

app.MapGet("/", () => "You are on Identity server");
app.MapGet("/protected", () => "secret").RequireAuthorization();
app.MapGet("/loginI{id}", (string id, HttpContext ctx) =>
{
    ctx.SignInAsync(new ClaimsPrincipal(new[]
    {
        new ClaimsIdentity(new List<Claim>()
        {
            new Claim(ClaimTypes.Name, id)
        },
        CookieAuthenticationDefaults.AuthenticationScheme)
    }));

    var redirectUrl = "https://app.company.local/WeatherForecast";
    return Results.Redirect(redirectUrl);
});
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

//app.MapGet("/DecodeCookie", (HttpContext ctx) =>
//{
//    string cookieName = "Identity.Application"; // Replace this with your actual cookie name
//    string decodedValue = string.Empty;
//    string secretKey1 = Environment.GetEnvironmentVariable("MY_SECRET_KEY");
//    string secretKey = "n1ylFIawNusG5rEas09HuwnCC6vWpKnv9qnaFcpBmRtdZeASxyefiSAkxpJRHvuWlV9h4NkIuBceRhvdjZ4pKA==";   // Replace this with the secret key used for encryption


//    string cookieValue = ctx.Request.Cookies[cookieName];

//    if (ctx.Request.Cookies.TryGetValue(cookieName, out string encodedCookieValue))
//    {



//        // Decode the cookie value from Base64 to its original encrypted binary data
//        byte[] encryptedBytes = Convert.FromBase64String(encodedCookieValue);

//        // Decrypt the cookie using AES-256-CBC
//        byte[] decryptedBytes;
//        using (Aes aes = Aes.Create())
//        {
//            aes.Key = Encoding.UTF8.GetBytes(secretKey);
//            aes.IV = new byte[16]; // This should be the IV used during encryption
//            aes.Mode = CipherMode.CBC;
//            aes.Padding = PaddingMode.PKCS7;

//            using (ICryptoTransform decryptor = aes.CreateDecryptor())
//            {
//                decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
//            }
//        }

//        // Separate the decrypted data and HMAC (last 32 bytes)
//        byte[] data = new byte[decryptedBytes.Length - 32];
//        byte[] hmac = new byte[32];
//        Array.Copy(decryptedBytes, 0, data, 0, data.Length);
//        Array.Copy(decryptedBytes, data.Length, hmac, 0, 32);

//        // Validate the HMAC using HMACSHA256
//        using (HMACSHA256 hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
//        {
//            byte[] expectedHmac = hmacSha256.ComputeHash(data);
//            if (!StructuralComparisons.StructuralEqualityComparer.Equals(hmac, expectedHmac))
//            {
//                // HMAC validation failed, the data might have been tampered with
//                return "Cookie data has been tampered with.";
//            }
//        }

//        // Convert the decrypted data to a string (assuming it was a string before encryption)
//        decodedValue = Encoding.UTF8.GetString(data);
//    }

//    // Now you can use the decoded value as needed
//    return decodedValue;
//});

app.Run();
