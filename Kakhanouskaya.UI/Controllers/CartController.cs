using Microsoft.AspNetCore.Mvc;
using Kakhanouskaya.DOMAIN.Models;
using Kakhanouskaya.UI.Extensions;  // для нашых метадаў Set/Get
//using Kakhanouskaya.Services.Interfaces; // сервіс для працы са стравамі
using Kakhanouskaya.DOMAIN.Models;
using Kakhanouskaya.DOMAIN.Services;
using Kakhanouskaya.UI.Extensions;

namespace Kakhanouskaya.UI.Controllers
{
    
        public class CartController : Controller
        {
            private readonly IProductService _productService; // ці IDishService

            public CartController(IProductService productService)
            {
                _productService = productService;
            }

            // Паказаць кошык
            public IActionResult Index()
            {
                var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
                return View(cart.CartItems);
            }

            // Дадаць страву ў кошык
            [Route("[controller]/add/{id:int}")]
            public async Task<IActionResult> Add(int id, string returnUrl)
            {
                var result = await _productService.GetProductByIdAsync(id); // ці GetDishByIdAsync

                if (result.Success)
                {
                    var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
                    cart.AddToCart(result.Data);
                    HttpContext.Session.Set<Cart>("cart", cart);
                }

                return Redirect(returnUrl);
            }

            // Выдаліць страву з кошыка
            [Route("[controller]/remove/{id:int}")]
            public IActionResult Remove(int id)
            {
                var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
                cart.RemoveItems(id);
                HttpContext.Session.Set<Cart>("cart", cart);

                return RedirectToAction("Index");
            }
        }
    
}
