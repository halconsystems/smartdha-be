using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MobileAPI.RealTime;

[AllowAnonymous] // keep anon while you debug; switch to [Authorize] later if needed
public class PanicHub : Hub<IPanicHubClient>
{
    public static class PanicGroups
    {
        public static string Dispatchers => "dispatchers";
        public static string User(string userId) => $"user:{userId}";
        public static string Panic(Guid panicId) => $"panic:{panicId:N}";
    }

    public override async Task OnConnectedAsync()
    {
        var logger = Context.GetHttpContext()?.RequestServices.GetRequiredService<ILogger<PanicHub>>();
        logger?.LogInformation("Hub connected: {ConnId}", Context.ConnectionId);

        // Optional: auto-subscribe everyone as dispatcher while testing
        // await Groups.AddToGroupAsync(Context.ConnectionId, PanicGroups.Dispatchers);

        await base.OnConnectedAsync();
    }

    // Call this from client right after connection.start()
    public Task JoinDispatchers() =>
        Groups.AddToGroupAsync(Context.ConnectionId, PanicGroups.Dispatchers);

    public Task LeaveDispatchers() =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, PanicGroups.Dispatchers);

    public Task JoinPanic(Guid panicId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, PanicGroups.Panic(panicId));

    public Task LeavePanic(Guid panicId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, PanicGroups.Panic(panicId));
}
