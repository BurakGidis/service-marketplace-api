using Microsoft.EntityFrameworkCore;
using ServiceMarketAPI.Data;
using ServiceMarketAPI.DTOs.Services;
using ServiceMarketAPI.Models;

namespace ServiceMarketAPI.Services
{
    public class ServiceListingService : IServiceListingService
    {
        private readonly ApplicationDbContext _context;

        public ServiceListingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task CreateListingAsync(CreateServiceRequest request, string userId)
        {
            
            var newListing = new ServiceListing
            {
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                City = request.City,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            _context.ServiceListings.Add(newListing);

            
            var user = await _context.Users.FindAsync(userId);
            if (user != null && !user.IsProvider)
            {
                user.IsProvider = true; 
                _context.Users.Update(user);
            }

            
            await _context.SaveChangesAsync();
        }
    }
}