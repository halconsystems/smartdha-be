using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using static DHAFacilitationAPIs.Web.RealTime.PanicHub;

namespace DHAFacilitationAPIs.Web.RealTime;

[AllowAnonymous]
public class OrderHub : Hub<IOrderHubClient>
{
    public static class OrdercGroups
    {
        public static string Dispatchers => "dispatchers";
        public static string User(string userId) => $"user:{userId}";
        public static string Order(Guid OrderId) => $"Order:{OrderId:N}";
    }
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrWhiteSpace(userId))
            await this.Groups.AddToGroupAsync(Context.ConnectionId, OrdercGroups.User(userId));

        //if (Context.User?.IsInRole("Dispatcher") == true || Context.User?.IsInRole("SuperAdministrator") == true)
        await this.Groups.AddToGroupAsync(Context.ConnectionId, OrdercGroups.Dispatchers);
        await base.OnConnectedAsync();
    }

    public Task JoinPanic(Guid panicId) =>
        this.Groups.AddToGroupAsync(Context.ConnectionId, OrdercGroups.Order(panicId));

    public Task LeavePanic(Guid panicId) =>
        this.Groups.RemoveFromGroupAsync(Context.ConnectionId, OrdercGroups.Order(panicId));

}
