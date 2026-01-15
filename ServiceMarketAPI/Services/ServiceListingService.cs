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

        
        public async Task<bool> UpdateListingAsync(int id, UpdateServiceRequest request, string userId)
        {
            var listing = await _context.ServiceListings.FindAsync(id);

           
            if (listing == null || listing.UserId != userId)
            {
                return false;
            }

            listing.Title = request.Title;
            listing.Description = request.Description;
            listing.Price = request.Price;
            listing.City = request.City;
            listing.CategoryId = request.CategoryId;

            _context.ServiceListings.Update(listing);
            await _context.SaveChangesAsync();

            return true;
        }

        
        public async Task<bool> DeleteListingAsync(int id, string userId)
        {
            var listing = await _context.ServiceListings.FindAsync(id);

            if (listing == null || listing.UserId != userId)
            {
                return false;
            }

            _context.ServiceListings.Remove(listing);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<ServiceListing>> GetAllListingsAsync(ServiceFilterDto filter)
        {
            
            var query = _context.ServiceListings
                .Include(x => x.Category) 
                .Include(x => x.User)     
                .AsQueryable();

            
            if (filter.CategoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == filter.CategoryId.Value);
            }

            if (!string.IsNullOrEmpty(filter.City))
            {
                
                query = query.Where(x => x.City.ToLower().Contains(filter.City.ToLower()));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(x => x.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= filter.MaxPrice.Value);
            }

            
            query = filter.SortBy switch
            {
                "price_asc" => query.OrderBy(x => x.Price),            
                "price_desc" => query.OrderByDescending(x => x.Price), 
                "rating_desc" => query.OrderByDescending(x => x.AverageRating), 
                _ => query.OrderByDescending(x => x.CreatedDate)       
            };

        
            var skipAmount = (filter.Page - 1) * filter.PageSize;
            query = query.Skip(skipAmount).Take(filter.PageSize);

            
            return await query.ToListAsync();
        }
    }
}