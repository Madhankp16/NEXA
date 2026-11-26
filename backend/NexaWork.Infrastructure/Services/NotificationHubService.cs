using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Infrastructure.Hubs;

namespace NexaWork.Infrastructure.Services;

public class NotificationHubService : INotificationHubService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationHubService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToUserAsync(Guid userId, string messageType, object payload)
    {
        await _hubContext.Clients.Group($"user_{userId}").SendAsync(messageType, payload);
    }

    public async Task SendToOrganizationAsync(Guid organizationId, string messageType, object payload)
    {
        await _hubContext.Clients.Group($"org_{organizationId}").SendAsync(messageType, payload);
    }
}
