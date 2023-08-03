using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[ApiController]
[Route("[controller]")]
public class UnAuthenticateController : ControllerBase
{
    public IActionResult Get()
    {
        return Redirect("https://identity.company.local");
    }
}
