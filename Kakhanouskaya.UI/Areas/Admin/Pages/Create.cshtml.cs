using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.UI.Services;
using Kakhanouskaya.DOMAIN.Services;

namespace Kakhanouskaya.UI.Areas.Admin.Pages
{
    [Authorize(Policy = "admin")]
    public class CreateModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public CreateModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public Dish Dish { get; set; } = default!;

        [BindProperty]
        public IFormFile? Image { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var categoriesResponse = await _categoryService.GetCategoryListAsync();
            if (categoriesResponse.Success)
            {
                ViewData["CategoryId"] = new SelectList(categoriesResponse.Data, "Id", "Name");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var categoriesResponse = await _categoryService.GetCategoryListAsync();
                if (categoriesResponse.Success)
                {
                    ViewData["CategoryId"] = new SelectList(categoriesResponse.Data, "Id", "Name");
                }
                return Page();
            }

            var response = await _productService.CreateProductAsync(Dish, Image);

            if (response.Success)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "Памылка пры стварэнні стравы");

                var categoriesResponse = await _categoryService.GetCategoryListAsync();
                if (categoriesResponse.Success)
                {
                    ViewData["CategoryId"] = new SelectList(categoriesResponse.Data, "Id", "Name");
                }

                return Page();
            }
        }
    }
}