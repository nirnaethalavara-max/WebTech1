using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Models;      // ← ДАДАЦЬ (для ListModel)
using Kakhanouskaya.DOMAIN.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kakhanouskaya.UI.Controllers
{

    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [Route("Catalog/{category?}")]
        public async Task<IActionResult> Index(string? category, int pageNo = 1)  // ← ДАДАЦЬ pageNo
        {
        
            // Атрымаць спіс катэгорый
            var categoriesResponse = await _categoryService.GetCategoryListAsync();
            if (!categoriesResponse.Success)
            {
                return NotFound(categoriesResponse.ErrorMessage);
            }

            ViewData["categories"] = categoriesResponse.Data;

            // Вызначыць бягучую катэгорыю для адлюстравання
            var currentCategory = string.IsNullOrEmpty(category)
                ? "Усе"
                : categoriesResponse.Data?.FirstOrDefault(c => c.NormalizedName == category)?.Name ?? "Усе";

            ViewData["currentCategory"] = currentCategory;

            // Атрымаць спіс страў (з пагінацыяй)
            var productResponse = await _productService.GetProductListAsync(category, pageNo);  // ← ДАДАЦЬ pageNo

            if (!productResponse.Success)
            {
                ViewData["error"] = productResponse.ErrorMessage;
                return View(new ListModel<Dish>());  // ← Вярнуць пустую ListModel
            }

            return View(productResponse.Data);  // ← Вярнуць увесь ListModel<Dish>, а не толькі Items
        }
    }

}