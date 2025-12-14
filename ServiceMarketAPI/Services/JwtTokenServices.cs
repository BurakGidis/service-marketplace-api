using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceMarketAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ServiceMarketAPI.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(ApplicationUser user)
        {
            var jwt = _config.GetSection("Jwt");
            var key = jwt["Key"]!;
            var issuer = jwt["Issuer"]!;
            var audience = jwt["Audience"]!;
            var expiresMinutes = int.Parse(jwt["ExpiresMinutes"]!);

            
            var claims = new List<Claim>
            {
                
                new Claim(ClaimTypes.NameIdentifier, user.Id),

              
                new Claim(ClaimTypes.Email, user.Email ?? ""),

                
                new Claim("isProvider", user.IsProvider.ToString().ToLower())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
