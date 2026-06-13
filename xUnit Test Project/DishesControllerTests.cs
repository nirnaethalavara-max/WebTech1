using Kakhanouskaya.API.Controllers;
using Kakhanouskaya.API.Data;
using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kakhanouskaya.Tests
{
    public class DishesControllerTests : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;
        private readonly IWebHostEnvironment _environment;

        public DishesControllerTests()
        {
            _environment = Substitute.For<IWebHostEnvironment>();

            // Стварэнне SQLite in-memory базы
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Стварэнне схемы і запаўненне тэставымі данымі
            using var context = new AppDbContext(_contextOptions);
            context.Database.EnsureCreated();

            // Дадаём катэгорыі
            var categories = new Category[]
            {
                new Category { Id = 1, Name = "Супы", NormalizedName = "soups" },
                new Category { Id = 2, Name = "Гарніры", NormalizedName = "side-dishes" },
                new Category { Id = 3, Name = "Дэсерты", NormalizedName = "desserts" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Дадаём стравы
            var dishes = new Dish[]
            {
                new Dish { Id = 1, Name = "Борщ", Description = "Смачны боршч", Price = 200, CategoryId = 1, Category = categories[0] },
                new Dish { Id = 2, Name = "Шчы", Description = "Кіслыя шчы", Price = 150, CategoryId = 1, Category = categories[0] },
                new Dish { Id = 3, Name = "Бульба", Description = "Смажаная бульба", Price = 300, CategoryId = 2, Category = categories[1] },
                new Dish { Id = 4, Name = "Грэчка", Description = "Грэчка з грыбамі", Price = 250, CategoryId = 2, Category = categories[1] },
                new Dish { Id = 5, Name = "Драчона", Description = "Бульбяная запяканка", Price = 320, CategoryId = 2, Category = categories[1] },
                new Dish { Id = 6, Name = "Марозіва", Description = "Ванільнае", Price = 400, CategoryId = 3, Category = categories[2] }
            };
            context.Dishes.AddRange(dishes);
            context.SaveChanges();
        }

        public void Dispose() => _connection?.Dispose();

        private AppDbContext CreateContext() => new AppDbContext(_contextOptions);

        // Тэст 1: Праверка фільтрацыі па катэгорыі
        [Fact]
        public async Task ControllerFiltersCategory()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new DishesController(context, _environment);
            var category = "soups"; // фільтр па супах

            // Act
            var response = await controller.GetDishes(category, 1, 10);
            var result = response.Result as OkObjectResult;
            var responseData = result?.Value as ResponseData<ProductListModel<Dish>>;
            var dishesList = responseData?.Data?.Items ?? new();

            // Assert
            Assert.NotNull(responseData);
            Assert.True(responseData.Success);
            Assert.All(dishesList, d => Assert.Equal(1, d.CategoryId));
        }

        // Тэст 2: Праверка падліку колькасці старонак
        [Theory]
        [InlineData(2, 3)]   // pageSize=2, чакаем 3 старонкі (6 страў / 2 = 3)
        [InlineData(3, 2)]   // pageSize=3, чакаем 2 старонкі (6 страў / 3 = 2)
        [InlineData(5, 2)]   // pageSize=5, чакаем 2 старонкі (6 страў / 5 = 2)
        [InlineData(10, 1)]  // pageSize=10, чакаем 1 старонку (6 страў / 10 = 1)
        public async Task ControllerReturnsCorrectPagesCount(int pageSize, int expectedPages)
        {
            // Arrange
            using var context = CreateContext();
            var controller = new DishesController(context, _environment);

            // Act
            var response = await controller.GetDishes(null, 1, pageSize);
            var result = response.Result as OkObjectResult;
            var responseData = result?.Value as ResponseData<ProductListModel<Dish>>;
            var totalPages = responseData?.Data?.TotalPages ?? 0;

            // Assert
            Assert.Equal(expectedPages, totalPages);
        }

        // Тэст 3: Праверка, што метод вяртае правільную старонку
        [Fact]
        public async Task ControllerReturnsCorrectPage()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new DishesController(context, _environment);
            int pageSize = 3;
            int pageNo = 2; // бярэм 2-ю старонку

            // Пры pageSize=3 і 6 стравах:
            // Старонка 1: ідэкс 0-2 (Борщ, Шчы, Бульба)
            // Старонка 2: ідэкс 3-5 (Грэчка, Драчона, Марозіва)
            var expectedFirstItemId = 4; // Грэчка

            // Act
            var response = await controller.GetDishes(null, pageNo, pageSize);
            var result = response.Result as OkObjectResult;
            var responseData = result?.Value as ResponseData<ProductListModel<Dish>>;
            var dishesList = responseData?.Data?.Items ?? new();
            var currentPage = responseData?.Data?.CurrentPage ?? 0;

            // Assert
            Assert.Equal(pageNo, currentPage);
            Assert.Equal(3, dishesList.Count); // на 2-й старонцы 3 стравы
            Assert.Equal(expectedFirstItemId, dishesList[0].Id);
        }

        // Дадатковы тэст: праверка, што пустая катэгорыя вяртае памылку
        [Fact]
        public async Task ControllerReturnsError_WhenNoDishesInCategory()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new DishesController(context, _environment);
            var category = "non-existent-category";

            // Act
            var response = await controller.GetDishes(category, 1, 3);
            var result = response.Result as OkObjectResult;
            var responseData = result?.Value as ResponseData<ProductListModel<Dish>>;

            // Assert
            Assert.NotNull(responseData);
            Assert.False(responseData.Success);
            Assert.Equal("Няма страў у выбранай катэгорыі", responseData.ErrorMessage);
        }
    }
}