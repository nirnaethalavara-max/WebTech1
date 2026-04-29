using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Kakhanouskaya.UI.Data
{
    public class DbInit
    {
        public static async Task SeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            var email = "admin@gmail.com";
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "123456");

                var claim = new Claim(ClaimTypes.Role, "admin");
                await userManager.AddClaimAsync(user, claim);
            }
        }
    }
}
