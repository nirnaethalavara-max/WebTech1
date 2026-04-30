using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Models;

namespace Kakhanouskaya.DOMAIN.Services
{
    public class MemoryCategoryService : ICategoryService
    {
        public Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Desserts", NormalizedName = "desserts" },
            new Category { Id = 2, Name = "Cheese", NormalizedName = "cheese" }            
        };

            var result = new ResponseData<List<Category>>
            {
                Data = categories,
                Success = true
            };

            return Task.FromResult(result);
        }
    }
}
