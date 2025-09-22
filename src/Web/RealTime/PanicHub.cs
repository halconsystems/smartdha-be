using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DHAFacilitationAPIs.Web.RealTime;

[AllowAnonymous] // keep if your hub requires JWT
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
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrWhiteSpace(userId))
            await this.Groups.AddToGroupAsync(Context.ConnectionId, PanicGroups.User(userId));

        //if (Context.User?.IsInRole("Dispatcher") == true || Context.User?.IsInRole("SuperAdministrator") == true)
        await this.Groups.AddToGroupAsync(Context.ConnectionId, PanicGroups.Dispatchers);
        await base.OnConnectedAsync();
    }

    public Task JoinPanic(Guid panicId) =>
        this.Groups.AddToGroupAsync(Context.ConnectionId, PanicGroups.Panic(panicId));

    public Task LeavePanic(Guid panicId) =>
        this.Groups.RemoveFromGroupAsync(Context.ConnectionId, PanicGroups.Panic(panicId));
}
