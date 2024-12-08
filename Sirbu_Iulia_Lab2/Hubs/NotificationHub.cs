using Microsoft.AspNetCore.SignalR;
namespace Sirbu_Iulia_Lab2.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly static PresenceTracker presenceTracker = new PresenceTracker();
        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";

            var result = await presenceTracker.ConnectionOpened(userName);
            if (result.UserJoined)
            {
                await Clients.All.SendAsync("newMessage", "system", $"{userName} joined");
            }
            var currentUsers = await presenceTracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("newMessage", "system", $"Currently online:\n{string.Join("\n", currentUsers)}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userName = Context.User?.Identity?.Name ?? $"Anonymous_{Context.ConnectionId}";

            var result = await presenceTracker.ConnectionClosed(userName);
            if (result.UserLeft)
            {
                await Clients.All.SendAsync("newMessage", "system", $"{userName} left");
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}