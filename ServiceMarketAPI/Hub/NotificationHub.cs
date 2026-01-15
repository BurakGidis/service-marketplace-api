using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace ServiceMarketAPI.Hubs
{
    [Authorize] 
    public class NotificationHub : Hub
    {
       
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}