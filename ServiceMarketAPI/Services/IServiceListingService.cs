using ServiceMarketAPI.DTOs.Services;
using ServiceMarketAPI.Models;

namespace ServiceMarketAPI.Services
{
    public interface IServiceListingService
    {
        Task CreateListingAsync(CreateServiceRequest request, string userId);
        Task<List<Category>> GetAllCategoriesAsync();
    }
}