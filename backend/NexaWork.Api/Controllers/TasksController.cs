using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Models;
using NexaWork.Application.DTOs;
using NexaWork.Domain.Entities;
using NexaWork.Domain.Enums;

namespace NexaWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationHubService _notificationHubService;

    public TasksController(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        INotificationHubService notificationHubService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _notificationHubService = notificationHubService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TaskItemDto>>>> GetAll([FromQuery] Guid? projectId)
    {
        var query = _context.Tasks.AsQueryable();

        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }

        var tasks = await query
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .Include(t => t.Reporter)
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .OrderBy(t => t.OrderIndex)
            .ThenByDescending(t => t.CreatedAt)
            .Select(t => new TaskItemDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                ProjectName = t.Project != null ? t.Project.Name : "",
                ProjectKey = t.Project != null ? t.Project.Key : "",
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                OrderIndex = t.OrderIndex,
                AssigneeId = t.AssigneeId,
                AssigneeName = t.Assignee != null ? t.Assignee.FullName : "Unassigned",
                AssigneeAvatar = t.Assignee != null ? t.Assignee.AvatarUrl : null,
                ReporterId = t.ReporterId,
                ReporterName = t.Reporter != null ? t.Reporter.FullName : "System",
                DueDate = t.DueDate,
                EstimatedHours = t.EstimatedHours,
                LoggedHours = t.LoggedHours,
                CommentCount = t.Comments.Count,
                AttachmentCount = t.Attachments.Count,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<TaskItemDto>>.SuccessResult(tasks));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskItemDto>>> Create([FromBody] CreateTaskDto request)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Unauthorized(ApiResponse<TaskItemDto>.FailureResult("Unauthorized"));
        }

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
            AssigneeId = request.AssigneeId,
            ReporterId = userId.Value,
            DueDate = request.DueDate,
            EstimatedHours = request.EstimatedHours,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);

        // Add Notification for assignee
        if (request.AssigneeId.HasValue && request.AssigneeId.Value != userId.Value)
        {
            var notif = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = request.AssigneeId.Value,
                Title = "New Task Assigned",
                Message = $"You have been assigned to task: '{task.Title}'",
                Type = NotificationType.Info,
                TargetUrl = "/tasks",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notif);
            await _notificationHubService.SendToUserAsync(request.AssigneeId.Value, "ReceiveNotification", notif);
        }

        await _context.SaveChangesAsync();

        var project = await _context.Projects.FindAsync(task.ProjectId);
        var assignee = task.AssigneeId.HasValue ? await _context.Users.FindAsync(task.AssigneeId.Value) : null;
        var reporter = await _context.Users.FindAsync(userId.Value);

        var dto = new TaskItemDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            ProjectName = project?.Name ?? "",
            ProjectKey = project?.Key ?? "",
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            AssigneeId = task.AssigneeId,
            AssigneeName = assignee?.FullName ?? "Unassigned",
            ReporterId = task.ReporterId,
            ReporterName = reporter?.FullName ?? "You",
            DueDate = task.DueDate,
            EstimatedHours = task.EstimatedHours,
            CreatedAt = task.CreatedAt
        };

        return Ok(ApiResponse<TaskItemDto>.SuccessResult(dto, "Task created successfully"));
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusDto request)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound(ApiResponse<bool>.FailureResult("Task not found"));
        }

        task.Status = request.Status;
        task.OrderIndex = request.OrderIndex;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<bool>.SuccessResult(true, "Task status updated"));
    }
}
