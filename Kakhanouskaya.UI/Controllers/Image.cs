using Kakhanouskaya.UI.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Kakhanouskaya.UI.Controllers
{
    public class ImageController(UserManager<ApplicationUser> userManager) : Controller
    {
        public async Task<IActionResult> GetAvatar()
        {
            var email = User.FindFirst(ClaimTypes.Email)!.Value;
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Avatar != null)
                return File(user.Avatar, user.MimeType);
            var imagePath = Path.Combine("Images", "default-profile-picture.png"); 
            return File(imagePath, "image/png");
        }
    }
}
