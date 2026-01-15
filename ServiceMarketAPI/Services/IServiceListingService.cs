using ServiceMarketAPI.DTOs.Services;
using ServiceMarketAPI.Models;

namespace ServiceMarketAPI.Services
{
    public interface IServiceListingService
    {
        Task CreateListingAsync(CreateServiceRequest request, string userId);
        Task<List<Category>> GetAllCategoriesAsync();
        
        
        Task<bool> UpdateListingAsync(int id, UpdateServiceRequest request, string userId);
        Task<bool> DeleteListingAsync(int id, string userId);
        Task<List<ServiceListing>> GetAllListingsAsync(ServiceFilterDto filter);
    }
}