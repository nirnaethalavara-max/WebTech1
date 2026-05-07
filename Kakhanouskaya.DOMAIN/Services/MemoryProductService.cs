using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Kakhanouskaya.DOMAIN.Services
{
    public class MemoryProductService : IProductService
    {
        private List<Dish> _dishes;
        private List<Category> _categories;
        private readonly IConfiguration _config;

        public MemoryProductService(IConfiguration config, ICategoryService categoryService)
        {
            _config = config;
            var categoryResponse = categoryService.GetCategoryListAsync().Result;
            _categories = categoryResponse.Data!;
            SetupData();
        }

        private void SetupData()
        {
            _dishes = new List<Dish>
        {
            new Dish
            {
                Id = 1,
                Name = "Абваранка",
                Description = "Да гарбаты",
                Price = 12,
                Image = "/images/free-png.ru-74-200x200.png",
                CategoryId = _categories.First(c => c.NormalizedName == "desserts").Id
            },
            new Dish
            {
                Id = 2,
                Name = "Абваранка з марцыпанамі",
                Description = "Да кампоту",
                Price = 15,
                Image = "/images/free-png.ru-78-200x200.png",
                CategoryId = _categories.First(c => c.NormalizedName == "desserts").Id
            },
            new Dish
            {
                Id = 3,
                Name = "Сыр",
                Description = "Да ўсяго",
                Price = 14,
                Image = "/images/free-png.ru-641-200x200.png",
                CategoryId = _categories.First(c => c.NormalizedName == "cheese").Id
            },
            new Dish
            {
                Id = 4,
                Name = "Торт",
                Description = "Каларыйная бомба",
                Price = 25,
                Image = "/images/free-png.ru-848-200x200.png",
                CategoryId = _categories.First(c => c.NormalizedName == "desserts").Id
            }
        };
        }

        public Task<ResponseData<ListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            // 1. Падцягваем Category для ўсіх страў
            foreach (var dish in _dishes)
            {
                dish.Category = _categories.FirstOrDefault(c => c.Id == dish.CategoryId);
            }

            // 2. Фільтруем па катэгорыі (калі пададзена)
            var filteredDishes = string.IsNullOrEmpty(categoryNormalizedName)
                ? _dishes
                : _dishes.Where(d => d.Category != null && d.Category.NormalizedName == categoryNormalizedName).ToList();

            // 3. Атрымліваем памер старонкі з канфігурацыі
            int pageSize = int.Parse(_config["ItemsPerPage"]);

            // 4. Вылічаем агульную колькасць старонак
            int totalPages = (int)Math.Ceiling(filteredDishes.Count / (double)pageSize);

            // 5. Правяраем, каб pageNo не быў большым за totalPages
            if (pageNo > totalPages && totalPages > 0)
                pageNo = totalPages;
            if (pageNo < 1)
                pageNo = 1;

            // 6. Атрымліваем элементы для бягучай старонкі
            var itemsOnPage = filteredDishes
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 7. Ствараем мадэль са спісам тавараў
            var model = new ListModel<Dish>
            {
                Items = itemsOnPage,
                CurrentPage = pageNo,
                TotalPages = totalPages
            };

            // 8. Фарміруем адказ
            var result = new ResponseData<ListModel<Dish>>
            {
                Data = model,
                Success = filteredDishes.Count > 0 || pageNo == 1  // можа быць пустая катэгорыя
            };

            if (filteredDishes.Count == 0)
            {
                result.ErrorMessage = "Няма страў у выбранай катэгорыі";
            }

            return Task.FromResult(result);
        }

        // Часовыя рэалізацыі астатніх метадаў
        public Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            var dish = _dishes.FirstOrDefault(d => d.Id == id);
            return Task.FromResult(new ResponseData<Dish> { Data = dish, Success = dish != null });
        }

        public Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }
    }
}
