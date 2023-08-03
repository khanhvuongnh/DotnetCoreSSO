

namespace App;
public class UnauthorizedRedirectMiddleware
{
    private readonly RequestDelegate _next;

    public UnauthorizedRedirectMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next(context);

        // Check if the response status code is 401 Unauthorized
        if (context.Response.StatusCode == 401)
        {
            // Redirect the user to the unauthorized page
            context.Response.Redirect("https://identity.company.local");
        }
    }
}
