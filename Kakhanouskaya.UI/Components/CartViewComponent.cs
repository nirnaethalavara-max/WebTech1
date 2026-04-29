using Microsoft.AspNetCore.Mvc;

namespace Kakhanouskaya.UI.Components
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Тут у будучыні будзе логіка падліку тавараў у кошыку.
            // Напрыклад: var count = _cartService.Count;

            return View(); // Гэта каманда шукае файл Views/Shared/Components/Cart/Default.cshtml
        }
    }
}
