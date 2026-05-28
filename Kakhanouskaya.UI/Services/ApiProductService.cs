using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Models;
using Kakhanouskaya.DOMAIN.Services;
using System.Text.Json;

namespace Kakhanouskaya.UI.Services
{
    public class ApiProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ApiProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseData<ListModel<Dish>>> GetProductListAsync(
            string? categoryNormalizedName,
            int pageNo = 1)
        {
            try
            {
                // Фарміруем URL
                var url = $"{_httpClient.BaseAddress}?pageNo={pageNo}";
                if (!string.IsNullOrEmpty(categoryNormalizedName))
                {
                    url += $"&category={categoryNormalizedName}";
                }

                Console.WriteLine($"Запыт да API: {url}");

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Адказ API: {content}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ResponseData<ListModel<Dish>>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (result != null && result.Success)
                    {
                        Console.WriteLine($"Атрымана {result.Data?.Items?.Count} страў");
                        foreach (var dish in result.Data?.Items ?? new List<Dish>())
                        {
                            Console.WriteLine($"  {dish.Name}: {dish.Image}");
                        }
                        return result;
                    }
                }

                return new ResponseData<ListModel<Dish>>
                {
                    Success = false,
                    ErrorMessage = $"Памылка API: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<ListModel<Dish>>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        // Астатнія метады (пакуль не рэалізуем для тэсту)
        public Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
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