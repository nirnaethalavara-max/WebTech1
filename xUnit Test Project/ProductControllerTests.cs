using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Models;
using Kakhanouskaya.DOMAIN.Services;
using Kakhanouskaya.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kakhanouskaya.Tests
{
    public class ProductControllerTests
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductControllerTests()
        {
            _productService = Substitute.For<IProductService>();
            _categoryService = Substitute.For<ICategoryService>();
        }

        // Тэст 1: Спіс катэгорый захоўваецца ў ViewData (як у ЛР)
        [Fact]
        public async Task IndexPutsCategoriesToViewData()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Супы", NormalizedName = "soups" },
                new Category { Id = 2, Name = "Гарніры", NormalizedName = "side-dishes" },
                new Category { Id = 3, Name = "Дэсерты", NormalizedName = "desserts" }
            };

            var categoriesResponse = new ResponseData<List<Category>>
            {
                Success = true,
                Data = categories
            };

            _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoriesResponse));

            // Mock для GetProductListAsync
            var productResponse = new ResponseData<ListModel<Dish>>
            {
                Success = true,
                Data = new ListModel<Dish> { Items = new List<Dish>(), CurrentPage = 1, TotalPages = 1 }
            };
            _productService.GetProductListAsync(null, 1).Returns(Task.FromResult(productResponse));

            var controller = new ProductController(_productService, _categoryService);

            // Act
            var response = await controller.Index(null);

            // Assert
            var view = Assert.IsType<ViewResult>(response);
            var categoriesList = Assert.IsType<List<Category>>(view.ViewData["categories"]);
            Assert.Equal(3, categoriesList.Count);
            Assert.Equal("Усе", view.ViewData["currentCategory"]);
        }

        // Тэст 2: Імя бягучай катэгорыі захоўваецца ў ViewData
        [Fact]
        public async Task IndexSetsCorrectCurrentCategory()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Супы", NormalizedName = "soups" },
                new Category { Id = 2, Name = "Гарніры", NormalizedName = "side-dishes" }
            };

            var categoriesResponse = new ResponseData<List<Category>>
            {
                Success = true,
                Data = categories
            };

            _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoriesResponse));

            var productResponse = new ResponseData<ListModel<Dish>>
            {
                Success = true,
                Data = new ListModel<Dish> { Items = new List<Dish>(), CurrentPage = 1, TotalPages = 1 }
            };
            _productService.GetProductListAsync("soups", 1).Returns(Task.FromResult(productResponse));

            var controller = new ProductController(_productService, _categoryService);
            var currentCategory = "soups";

            // Act
            var response = await controller.Index(currentCategory);

            // Assert
            var view = Assert.IsType<ViewResult>(response);
            Assert.Equal("Супы", view.ViewData["currentCategory"]);
        }

        // Тэст 3: У выпадку памылкі вяртаецца NotFoundObjectResult
        [Fact]
        public async Task IndexReturnsNotFound()
        {
            // Arrange
            string errorMessage = "Test error";
            var categoriesResponse = new ResponseData<List<Category>>
            {
                Success = false,
                ErrorMessage = errorMessage
            };

            _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoriesResponse));

            var controller = new ProductController(_productService, _categoryService);

            // Act
            var response = await controller.Index(null);

            // Assert
            var result = Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal(errorMessage, result.Value);
        }
    }
}