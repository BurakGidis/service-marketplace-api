using ServiceMarketAPI.DTOs.Appointments;
using ServiceMarketAPI.Models;

namespace ServiceMarketAPI.Services
{
    public interface IAppointmentService
    {
        Task<List<object>> GetProviderAppointmentsAsync(string userId);
        Task<(bool Success, string Message, int? AppointmentId)> CreateAppointmentAsync(CreateAppointmentRequest request, string userId);
        Task<(bool Success, string Message)> ApproveAppointmentAsync(int id, string userId);
        Task<(bool Success, string Message)> RejectAppointmentAsync(int id, string userId);
        Task<(bool Success, string Message)> CompleteAppointmentAsync(int id, string userId);
        Task<(bool Success, string Message)> CancelAppointmentAsync(int id, string userId);
    }
}