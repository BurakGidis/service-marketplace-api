using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ServiceMarketAPI.Data;
using ServiceMarketAPI.DTOs.Services;
using ServiceMarketAPI.Models;
using ServiceMarketAPI.Services;
using System.Security.Claims;

namespace ServiceMarketAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceListingService _service;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CategoryCacheKey = "AllCategoriesCache";

        public ServicesController(IServiceListingService service, ApplicationDbContext context, IMemoryCache cache)
        {
            _service = service;
            _context = context;
            _cache = cache;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            if (!_cache.TryGetValue(CategoryCacheKey, out List<Category> categories))
            {
               
                categories = await _context.Categories.ToListAsync();

                
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                _cache.Set(CategoryCacheKey, categories, cacheEntryOptions);
            }

          
            return Ok(categories);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _service.CreateListingAsync(request, userId);

            return Ok(new { message = "İlan başarıyla oluşturuldu ve profiliniz Hizmet Veren (Provider) olarak güncellendi." });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _service.UpdateListingAsync(id, request, userId);

            if (!result)
            {
                return NotFound(new { message = "İlan bulunamadı veya bu işlemi yapmaya yetkiniz yok." });
            }

            return Ok(new { message = "İlan başarıyla güncellendi." });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteService(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _service.DeleteListingAsync(id, userId);

            if (!result)
            {
                return NotFound(new { message = "İlan bulunamadı veya silme yetkiniz yok." });
            }

            return Ok(new { message = "İlan başarıyla silindi." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllListings([FromQuery] ServiceFilterDto filter)
        {
            var listings = await _service.GetAllListingsAsync(filter);
            return Ok(listings);
        }
    }
}