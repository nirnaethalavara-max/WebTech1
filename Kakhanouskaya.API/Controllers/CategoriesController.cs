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
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<Category>>>> GetCategories()
        {
            var response = new ResponseData<IEnumerable<Category>>
            {
                Data = await _context.Categories.ToListAsync()
            };
            return Ok(response);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseData<Category>>> GetCategory(int id)
        {
            var response = new ResponseData<Category>();
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                response.Success = false;
                response.ErrorMessage = $"Катэгорыя з id {id} не знойдзена";
                return NotFound(response);
            }

            response.Data = category;
            return Ok(response);
        }
    }
}