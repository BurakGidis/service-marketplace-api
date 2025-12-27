using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceMarketAPI.DTOs.Users;
using ServiceMarketAPI.Models;
using System.Security.Claims;

namespace ServiceMarketAPI.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            return Ok(new
            {
                user.Id,
                user.Email,
                user.UserName,
                user.PhoneNumber,
                user.IsProvider
            });
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe(UpdateMeRequest req)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            if (!string.IsNullOrWhiteSpace(req.UserName))
                user.UserName = req.UserName;

            if (!string.IsNullOrWhiteSpace(req.PhoneNumber))
                user.PhoneNumber = req.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new { message = "Profile updated." });
        }
    }
}
