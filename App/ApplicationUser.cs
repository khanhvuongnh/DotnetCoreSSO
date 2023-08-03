using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace App
{
    public class ApplicationUser : IdentityUser
    {
        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        //{
        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    var userIdentity = await manager.CreateAsync(this, "CreateIdentityAsync");

        //    // Add custom user claims here
        //    return ClaimsIdentity;
        //}
    }
}
