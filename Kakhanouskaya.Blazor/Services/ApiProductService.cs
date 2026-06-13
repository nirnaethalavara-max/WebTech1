using System.Net.Http.Json;
using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Models;

namespace Kakhanouskaya.Blazor.Services
{
    public class ApiProductService : IProductService<Dish>
    {
        private readonly HttpClient _httpClient;
        private List<Dish> _dishes = new();
        private int _currentPage = 1;
        private int _totalPages = 1;

        public event Action? ListChanged;

        public IEnumerable<Dish> Products => _dishes;
        public int CurrentPage => _currentPage;
        public int TotalPages => _totalPages;

        public ApiProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task GetProducts(int pageNo = 1, int pageSize = 3)
        {
            try
            {
                // Фарміруем URL з параметрамі
                var url = $"?pageNo={pageNo}&pageSize={pageSize}";

                // Адпраўляем GET-запыт да API
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Дэсерыялізуем JSON у C#-аб'ект
                    var result = await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Dish>>>();

                    if (result != null && result.Success && result.Data != null)
                    {
                        _dishes = result.Data.Items ?? new List<Dish>();
                        _currentPage = result.Data.CurrentPage;
                        _totalPages = result.Data.TotalPages;
                    }
                }
                else
                {
                    // У выпадку памылкі ачышчаем спіс
                    _dishes = new List<Dish>();
                    _currentPage = 1;
                    _totalPages = 1;
                }
            }
            catch (Exception ex)
            {
                // У выпадку выключэння (напрыклад, API не даступна)
                Console.WriteLine($"Памылка пры загрузцы страў: {ex.Message}");
                _dishes = new List<Dish>();
                _currentPage = 1;
                _totalPages = 1;
            }

            // Паведамляем кампанентам, што спіс змяніўся
            ListChanged?.Invoke();
        }
    }
}