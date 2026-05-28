using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.API.Data;
using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Models;

namespace Kakhanouskaya.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DishesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/dishes?category=desserts&pageNo=1&pageSize=3
        [HttpGet]
        public async Task<ActionResult<ResponseData<ProductListModel<Dish>>>> GetDishes(
            string? category,
            int pageNo = 1,
            int pageSize = 3)
        {
            var result = new ResponseData<ProductListModel<Dish>>();

            // Фільтрацыя па катэгорыі + падгрузка даных катэгорыі
            var data = _context.Dishes
                .Include(d => d.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                data = data.Where(d => d.Category != null &&
                                       d.Category.NormalizedName == category);
            }

            // Падлік агульнай колькасці старонак
            var totalItems = await data.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (pageNo > totalPages && totalPages > 0)
                pageNo = totalPages;

            // Атрыманне даных для бягучай старонкі
            var items = await data
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var listData = new ProductListModel<Dish>
            {
                Items = items,
                CurrentPage = pageNo,
                TotalPages = totalPages
            };

            result.Data = listData;

            if (totalItems == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Няма страў у выбранай катэгорыі";
            }

            return Ok(result);
        }

        // GET: api/dishes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseData<Dish>>> GetDish(int id)
        {
            var response = new ResponseData<Dish>();
            var dish = await _context.Dishes
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dish == null)
            {
                response.Success = false;
                response.ErrorMessage = $"Страва з id {id} не знойдзена";
                return NotFound(response);
            }

            response.Data = dish;
            return Ok(response);
        }
    }
}
