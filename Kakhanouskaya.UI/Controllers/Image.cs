using Kakhanouskaya.DOMAIN.Services;
using Kakhanouskaya.UI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kakhanouskaya.UI.Controllers
{
    public class ImageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProductService _productService;  // ← дадаем

        // Абнаўляем канструктар
        public ImageController(UserManager<ApplicationUser> userManager, IProductService productService)
        {
            _userManager = userManager;
            _productService = productService;
        }

        // Гэты метад для аватара карыстальніка (без id)
        public async Task<IActionResult> GetAvatar()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return GetDefaultImage();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return GetDefaultImage();
            }

            if (user.Avatar != null)
                return File(user.Avatar, user.MimeType);

            return GetDefaultImage();
        }

        // 🟢 НОВЫ МЕТАД для выявы стравы (з id)
        public async Task<IActionResult> GetDishImage(int id)
        {
            var dish = await _productService.GetProductByIdAsync(id);

            if (dish.Success && dish.Data != null && !string.IsNullOrEmpty(dish.Data.Image))
            {
                // Шлях да выявы адносна wwwroot (напрыклад, "/images/photo.png")
                var imagePath = dish.Data.Image.TrimStart('/');
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath);

                if (System.IO.File.Exists(fullPath))
                {
                    var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                    var contentType = GetContentType(fullPath);
                    return File(imageBytes, contentType);
                }
            }

            // Калі выява не знойдзена — вяртаем стандартную
            return GetDefaultImage();
        }

        private IActionResult GetDefaultImage()
        {
            // Шукаем стандартную выяву
            var defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/default-avatar.png");

            if (!System.IO.File.Exists(defaultPath))
            {
                defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/free-png.ru-74-200x200.png");
            }

            if (System.IO.File.Exists(defaultPath))
            {
                var imageBytes = System.IO.File.ReadAllBytes(defaultPath);
                return File(imageBytes, "image/png");
            }

            return NotFound();
        }

        private string GetContentType(string path)
        {
            var extension = Path.GetExtension(path).ToLower();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "image/png"
            };
        }
    }
}