using Kakhanouskaya.DOMAIN.Entities;
using Kakhanouskaya.DOMAIN.Models;

namespace Kakhanouskaya.Blazor.Services
{
    public interface IProductService<T> where T : class
    {
        // Падзея, якая паведамляе кампанентам, што спіс змяніўся
        event Action? ListChanged;

        // Спіс страў
        IEnumerable<T> Products { get; }

        // Бягучы нумар старонкі
        int CurrentPage { get; }

        // Агульная колькасць старонак
        int TotalPages { get; }

        // Атрымаць спіс страў з API
        Task GetProducts(int pageNo = 1, int pageSize = 3);
    }
}