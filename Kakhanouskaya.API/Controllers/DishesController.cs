using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.API.Data;
using Kakhanouskaya.DOMAIN.Models;

namespace Kakhanouskaya.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DishesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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

        [HttpPost("{id}")]
        public async Task<IActionResult> SaveImage(int id, IFormFile image)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();

            var imagesPath = Path.Combine(_env.WebRootPath, "Images");
            if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);

            var randomName = Path.GetRandomFileName();
            var extension = Path.GetExtension(image.FileName);
            var fileName = Path.ChangeExtension(randomName, extension);
            var filePath = Path.Combine(imagesPath, fileName);

            using var stream = System.IO.File.OpenWrite(filePath);
            await image.CopyToAsync(stream);

            var host = $"{Request.Scheme}://{Request.Host}";
            var url = $"{host}/Images/{fileName}";
            dish.Image = url;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/dishes
        [HttpPost]
        public async Task<ActionResult<ResponseData<Dish>>> PostDish(Dish dish)
        {
            var response = new ResponseData<Dish>();

            try
            {
                _context.Dishes.Add(dish);
                await _context.SaveChangesAsync();

                response.Data = dish;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = $"Памылка БД: {ex.Message}";
                return BadRequest(response);
            }
        }

        // DELETE: api/dishes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound(new ResponseData<bool> { Success = false, ErrorMessage = "Страва не знойдзена" });
            }

            // Калі трэба, тут можна дадаць код выдалення выявы з папкі wwwroot/Images, калі яна існуе

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return Ok(new ResponseData<bool> { Success = true });
        }

        // PUT: api/dishes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDish(int id, Dish dish)
        {
            if (id != dish.Id)
            {
                return BadRequest(new ResponseData<bool> { Success = false, ErrorMessage = "ID не супадаюць" });
            }

            _context.Entry(dish).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Dishes.Any(e => e.Id == id))
                {
                    return NotFound(new ResponseData<bool> { Success = false, ErrorMessage = "Страва не знойдзена" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new ResponseData<bool> { Success = true });
        }

    }


}
