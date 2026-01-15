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

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> PostReview([FromBody] CreateReviewRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

          
            var result = await _reviewService.AddReviewAsync(request, userId!);

            if (!result.Success)
            {
              
                if (result.Message == "Randevu bulunamadı.")
                    return NotFound(result.Message);
                
                if (result.Message.Contains("yetki")) 
                    return Unauthorized(result.Message);

                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message });
        }
    }
}