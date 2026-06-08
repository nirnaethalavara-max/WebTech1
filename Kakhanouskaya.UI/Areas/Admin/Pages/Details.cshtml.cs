using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.UI.Services;
using Kakhanouskaya.DOMAIN.Services;

namespace Kakhanouskaya.UI.Areas.Admin.Pages
{
    [Authorize(Policy = "admin")]
    public class DetailsModel : PageModel
    {
        private readonly IProductService _productService;

        public DetailsModel(IProductService productService)
        {
            _productService = productService;
        }

        public Dish Dish { get; set; } = default!;

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
                return Page();
            }

            return NotFound();
        }
    }
}