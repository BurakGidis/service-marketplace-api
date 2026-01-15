using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceMarketAPI.DTOs.Auth;
using ServiceMarketAPI.Models;
using ServiceMarketAPI.Services;

namespace ServiceMarketAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
       
        private readonly IJwtTokenService _jwt;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwt) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var exists = await _userManager.FindByEmailAsync(req.Email);
            if (exists != null)
                return BadRequest("Email already in use.");

            var user = new ApplicationUser
            {
                Email = req.Email,
                UserName = req.Email,
                FullName = req.FullName,
                IsProvider = req.IsProvider
            };

            var result = await _userManager.CreateAsync(user, req.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new { message = "User created" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var ok = await _signInManager.CheckPasswordSignInAsync(
                user, req.Password, lockoutOnFailure: false);

            if (!ok.Succeeded)
                return Unauthorized("Invalid credentials");

           
            var token = _jwt.CreateToken(user);

            return Ok(new
            {
                accessToken = token,
                userId = user.Id,
                isProvider = user.IsProvider
            });
        }
    }
}