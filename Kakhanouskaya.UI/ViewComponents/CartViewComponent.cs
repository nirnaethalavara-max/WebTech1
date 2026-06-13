using Kakhanouskaya.DOMAIN.Models;
using Microsoft.AspNetCore.Mvc;
using Kakhanouskaya.DOMAIN.Models;
using Kakhanouskaya.UI.Extensions;

namespace Kakhanouskaya.UI.ViewComponents
{
    

   
        public class CartViewComponent : ViewComponent
        {
            public IViewComponentResult Invoke()
            {
                var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
                return View(cart);
            }
        }
    
}
