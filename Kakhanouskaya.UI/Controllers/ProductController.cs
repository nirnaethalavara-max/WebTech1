using Kakhanouskaya.DOMAIN.Entities;
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

        public async Task<IActionResult> Index(string? category)
        {
            // Атрымаць спіс катэгорый
            var categoriesResponse = await _categoryService.GetCategoryListAsync();
            if (!categoriesResponse.Success)
            {
                return NotFound(categoriesResponse.ErrorMessage);
            }

            ViewBag.Categories = categoriesResponse.Data;

            // Вызначыць бягучую катэгорыю для адлюстравання
            var currentCategory = string.IsNullOrEmpty(category)
                ? "Усе"
                : categoriesResponse.Data?.FirstOrDefault(c => c.NormalizedName == category)?.Name ?? "Усе";

            ViewBag.CurrentCategory = currentCategory;

            // Атрымаць спіс страў
            var productResponse = await _productService.GetProductListAsync(category);
            if (!productResponse.Success)
            {
                ViewBag.Error = productResponse.ErrorMessage;
                return View(new List<Dish>());
            }

            return View(productResponse.Data?.Items ?? new List<Dish>());
        }
    }

}
