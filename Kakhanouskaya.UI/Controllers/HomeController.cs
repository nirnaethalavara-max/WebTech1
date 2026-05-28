using Kakhanouskaya.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kakhanouskaya.UI.Controllers
{
    public class HomeController : Controller
    {
        //[Authorize]
        //[Authorize(Policy = "admin")]
        public IActionResult Index()
        {
            ViewBag.Title = "Галоўная старонка"; // Тэкст для ўкладкі браўзера
            ViewData["Header"] = "Лабараторная работа №3"; // Загаловак на старонцы
            var items = new List<ListDemo>
    {
        new ListDemo { Id = 1, Name = "Элемент 1" },
        new ListDemo { Id = 2, Name = "Элемент 2" },
        new ListDemo { Id = 3, Name = "Элемент 3" }
    };
            ViewBag.ItemsList = new SelectList(items, "Id", "Name");
            return View(items);
        }


    }

}
