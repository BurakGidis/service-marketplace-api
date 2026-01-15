using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceMarketAPI.DTOs.Appointments;
using ServiceMarketAPI.Services;
using System.Security.Claims;

namespace ServiceMarketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        // Context yerine Service enjekte ediyoruz
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> PostReview([FromBody] CreateReviewRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Tüm mantık servise taşındı, sadece sonucu kontrol ediyoruz
            var result = await _reviewService.AddReviewAsync(request, userId!);

            if (!result.Success)
            {
                // Hata mesajına göre uygun durum kodu dönebiliriz, burada genel olarak BadRequest dönüyoruz
                if (result.Message == "Randevu bulunamadı.")
                    return NotFound(result.Message);
                
                if (result.Message.Contains("yetki")) // "Unauthorized" durumları için basit kontrol
                    return Unauthorized(result.Message);

                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message });
        }
    }
}