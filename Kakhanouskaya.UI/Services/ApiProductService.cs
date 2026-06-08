using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Models;
using Kakhanouskaya.DOMAIN.Services;
using System.Net.Http;
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
        public async Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            try
            {
                // Робім GET-запыт на адрас выгляду: api/dishes/{id}
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress.ToString().TrimEnd('/')}/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ResponseData<Dish>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return result ?? new ResponseData<Dish> { Success = false, ErrorMessage = "Памылка дэсерыялізацыі дадзеных" };
                }

                return new ResponseData<Dish>
                {
                    Success = false,
                    ErrorMessage = $"Памылка API пры атрыманні стравы: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<Dish>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }


        public async Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
        {
            try
            {
                // 1. Абнаўляем асноўныя дадзеныя стравы праз PUT-запыт (JSON)
                var response = await _httpClient.PutAsJsonAsync($"{_httpClient.BaseAddress.ToString().TrimEnd('/')}/{id}", product);

                if (!response.IsSuccessStatusCode)
                {
                    var errContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Памылка API пры абнаўленні: {response.StatusCode}. {errContent}");
                }

                // 2. Калі карыстальнік выбраў новы файл выявы, загружаем яго зверху
                if (formFile != null)
                {
                    using var content = new MultipartFormDataContent();
                    var streamContent = new StreamContent(formFile.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(formFile.ContentType);

                    content.Add(streamContent, "image", formFile.FileName);

                    // Адпраўляем на той жа метад захавання выявы, што працуе пры стварэнні
                    var imageResponse = await _httpClient.PostAsync($"{_httpClient.BaseAddress.ToString().TrimEnd('/')}/{id}", content);

                    if (!imageResponse.IsSuccessStatusCode)
                    {
                        throw new Exception("Дадзеныя стравы абноўлены, але новую выяву захаваць не атрымалася.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Не атрымалася абнавіць страву: {ex.Message}");
            }
        }


        public async Task DeleteProductAsync(int id)
        {
            try
            {
                // Адпраўляем DELETE-запыт на адрас выгляду: api/dishes/{id}
                // Паколькі BaseAddress ужо змяшчае корань кантролера, проста дадаем ID у канец
                var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress.ToString().TrimEnd('/')}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Памылка API пры выдаленні: {response.StatusCode}. {errContent}");
                }
            }
            catch (Exception ex)
            {
                // Тут можна залагаваць памылку або пракінуць яе далей, каб старонка ведала пра збой
                throw new Exception($"Не атрымалася выдаліць страву: {ex.Message}");
            }
        }


        public async Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile)
        {
            try
            {
                // 1. Адпраўляем JSON-аб'ект на базавы адрас кантролера
                var response = await _httpClient.PostAsJsonAsync("", product);

                if (!response.IsSuccessStatusCode)
                {
                    var errContent = await response.Content.ReadAsStringAsync();
                    return new ResponseData<Dish>
                    {
                        Success = false,
                        ErrorMessage = $"Не атрымалася стварыць: {response.StatusCode}. {errContent}"
                    };
                }

                // 2. Дэсерыялізуем адказ. У вас API вяртае ResponseData<Dish>, таму чытаем яго цалкам
                var apiResult = await response.Content.ReadFromJsonAsync<ResponseData<Dish>>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResult == null || !apiResult.Success || apiResult.Data == null)
                {
                    return new ResponseData<Dish>
                    {
                        Success = false,
                        ErrorMessage = apiResult?.ErrorMessage ?? "Памылка апрацоўкі дадзеных ад API"
                    };
                }

                var createdDish = apiResult.Data;

                // 3. Калі ёсць выява, адпраўляем яе на правільны роўт кантролера з ID
                if (formFile != null)
                {
                    using var content = new MultipartFormDataContent();
                    var streamContent = new StreamContent(formFile.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(formFile.ContentType);

                    // "image" павінна строга супадаць з імем параметра ў DishesController [HttpPost("{id}")]
                    content.Add(streamContent, "image", formFile.FileName);

                    // Выкарыстоўваем адносны шлях да бягучага кантролера
                    var imageResponse = await _httpClient.PostAsync($"{createdDish.Id}", content);

                    if (!imageResponse.IsSuccessStatusCode)
                    {
                        var imgErr = await imageResponse.Content.ReadAsStringAsync();
                        return new ResponseData<Dish>
                        {
                            Success = false,
                            ErrorMessage = $"Страву стварылі, але выяву захаваць не атрымалася: {imageResponse.StatusCode} - {imgErr}"
                        };
                    }
                }

                return new ResponseData<Dish> { Success = true, Data = createdDish };
            }
            catch (Exception ex)
            {
                return new ResponseData<Dish> { Success = false, ErrorMessage = ex.Message };
            }
        }


    }
}