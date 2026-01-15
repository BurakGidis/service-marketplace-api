using ServiceMarketAPI.DTOs.Appointments;

namespace ServiceMarketAPI.Services
{
    public interface IReviewService
    {
        Task<(bool Success, string Message)> AddReviewAsync(CreateReviewRequest request, string userId);
    }
}
