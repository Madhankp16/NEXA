using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace NexaWork.Infrastructure.Hubs;

public class NotificationHub : Hub
{
    public async Task JoinOrganizationGroup(string orgId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"org_{orgId}");
    }

    public async Task LeaveOrganizationGroup(string orgId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"org_{orgId}");
    }

    public async Task JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
    }
}
