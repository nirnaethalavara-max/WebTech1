using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Models;


namespace Kakhanouskaya.DOMAIN.Services
{
    public interface ICategoryService
    {
        /// <summary>
        /// Получение списка всех категорий
        /// </summary>
        /// <returns></returns>
        public Task<ResponseData<List<Category>>> GetCategoryListAsync();
    }
}

