using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Models;
using Kakhanouskaya.DOMAIN.Services;

namespace Kakhanouskaya.UI.Services
{
    public class ApiCategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public ApiCategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var result = await _httpClient.GetAsync(_httpClient.BaseAddress);

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadFromJsonAsync<ResponseData<List<Category>>>();
            }

            var response = new ResponseData<List<Category>>
            {
                Success = false,
                ErrorMessage = "Ошибка чтения API"
            };
            return response;
        }
    }
}