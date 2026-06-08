using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Services;

namespace Kakhanouskaya.UI.Areas.Admin.Pages
{
    [Authorize(Policy = "admin")]
    public class EditModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public EditModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public Dish Dish { get; set; } = default!;

        [BindProperty]
        public IFormFile? Image { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _productService.GetProductByIdAsync(id.Value);

            if (response.Success && response.Data != null)
            {
                Dish = response.Data;

                var categoriesResponse = await _categoryService.GetCategoryListAsync();
                if (categoriesResponse.Success)
                {
                    ViewData["CategoryId"] = new SelectList(categoriesResponse.Data, "Id", "Name", Dish.CategoryId);
                }

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var categoriesResponse = await _categoryService.GetCategoryListAsync();
                if (categoriesResponse.Success)
                {
                    ViewData["CategoryId"] = new SelectList(categoriesResponse.Data, "Id", "Name", Dish.CategoryId);
                }
                return Page();
            }

            // Проста выклікаем WITHOUT присваивания (метод возвращает Task, а не Task<something>)
            await _productService.UpdateProductAsync(Dish.Id, Dish, Image);

            return RedirectToPage("./Index");
        }
    }
}