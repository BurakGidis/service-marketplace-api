using ServiceMarketAPI.Models;

namespace ServiceMarketAPI.Services
{
    public interface IJwtTokenService
    {
        
        string CreateToken(ApplicationUser user);
    }
}