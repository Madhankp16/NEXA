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
public class WorkflowsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationHubService _notificationHubService;

    public WorkflowsController(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        INotificationHubService notificationHubService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _notificationHubService = notificationHubService;
    }

    [HttpGet("approvals")]
    public async Task<ActionResult<ApiResponse<List<ApprovalRequestDto>>>> GetApprovals([FromQuery] ApprovalStatus? status)
    {
        var orgId = _currentUserService.OrganizationId;
        var query = _context.ApprovalRequests.AsQueryable();

        if (orgId.HasValue)
        {
            query = query.Where(ar => ar.OrganizationId == orgId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(ar => ar.Status == status.Value);
        }

        var list = await query
            .Include(ar => ar.Requester)
            .Include(ar => ar.Steps)
            .ThenInclude(s => s.ApproverUser)
            .OrderByDescending(ar => ar.CreatedAt)
            .Select(ar => new ApprovalRequestDto
            {
                Id = ar.Id,
                RequesterId = ar.RequesterId,
                RequesterName = ar.Requester != null ? ar.Requester.FullName : "System",
                RequesterAvatar = ar.Requester != null ? (ar.Requester.AvatarUrl ?? "") : "",
                Title = ar.Title,
                Description = ar.Description,
                WorkflowType = ar.WorkflowType,
                Status = ar.Status,
                CurrentStepNumber = ar.CurrentStepNumber,
                TotalSteps = ar.TotalSteps,
                PayloadJson = ar.PayloadJson,
                CreatedAt = ar.CreatedAt,
                CompletedAt = ar.CompletedAt,
                Steps = ar.Steps.OrderBy(s => s.StepNumber).Select(s => new ApprovalStepDto
                {
                    Id = s.Id,
                    StepNumber = s.StepNumber,
                    StepName = s.StepName,
                    ApproverUserId = s.ApproverUserId,
                    ApproverUserName = s.ApproverUser != null ? s.ApproverUser.FullName : s.ApproverRole,
                    ApproverRole = s.ApproverRole,
                    Status = s.Status,
                    Comments = s.Comments,
                    ActionDate = s.ActionDate
                }).ToList()
            })
            .ToListAsync();

        return Ok(ApiResponse<List<ApprovalRequestDto>>.SuccessResult(list));
    }

    [HttpPost("approvals")]
    public async Task<ActionResult<ApiResponse<ApprovalRequestDto>>> CreateApproval([FromBody] CreateApprovalRequestDto request)
    {
        var userId = _currentUserService.UserId;
        var orgId = _currentUserService.OrganizationId;

        if (!userId.HasValue || !orgId.HasValue)
        {
            return Unauthorized(ApiResponse<ApprovalRequestDto>.FailureResult("Unauthorized"));
        }

        var approval = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId.Value,
            RequesterId = userId.Value,
            Title = request.Title,
            Description = request.Description,
            WorkflowType = request.WorkflowType,
            Status = ApprovalStatus.Pending,
            CurrentStepNumber = 1,
            TotalSteps = 2,
            PayloadJson = request.PayloadJson,
            CreatedAt = DateTime.UtcNow
        };

        // Create Default Multi-level approval steps
        approval.Steps.Add(new ApprovalStep
        {
            Id = Guid.NewGuid(),
            ApprovalRequestId = approval.Id,
            StepNumber = 1,
            StepName = "Team Manager Review",
            ApproverRole = "Manager",
            Status = StepApprovalStatus.Pending
        });

        approval.Steps.Add(new ApprovalStep
        {
            Id = Guid.NewGuid(),
            ApprovalRequestId = approval.Id,
            StepNumber = 2,
            StepName = "Department VP Final Sign-Off",
            ApproverRole = "SuperAdmin",
            Status = StepApprovalStatus.Pending
        });

        _context.ApprovalRequests.Add(approval);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<ApprovalRequestDto>.SuccessResult(new ApprovalRequestDto
        {
            Id = approval.Id,
            RequesterId = approval.RequesterId,
            RequesterName = "You",
            Title = approval.Title,
            Description = approval.Description,
            WorkflowType = approval.WorkflowType,
            Status = approval.Status,
            CurrentStepNumber = approval.CurrentStepNumber,
            TotalSteps = approval.TotalSteps,
            CreatedAt = approval.CreatedAt
        }, "Approval request initiated successfully"));
    }

    [HttpPost("approvals/{id}/steps/{stepNumber}/process")]
    public async Task<ActionResult<ApiResponse<bool>>> ProcessStep(Guid id, int stepNumber, [FromBody] ProcessApprovalStepDto request)
    {
        var approval = await _context.ApprovalRequests
            .Include(ar => ar.Steps)
            .FirstOrDefaultAsync(ar => ar.Id == id);

        if (approval == null)
        {
            return NotFound(ApiResponse<bool>.FailureResult("Approval request not found"));
        }

        var step = approval.Steps.FirstOrDefault(s => s.StepNumber == stepNumber);
        if (step == null)
        {
            return NotFound(ApiResponse<bool>.FailureResult("Step not found"));
        }

        step.Status = request.IsApproved ? StepApprovalStatus.Approved : StepApprovalStatus.Rejected;
        step.Comments = request.Comments;
        step.ActionDate = DateTime.UtcNow;
        step.ApproverUserId = _currentUserService.UserId;

        if (!request.IsApproved)
        {
            approval.Status = ApprovalStatus.Rejected;
            approval.CompletedAt = DateTime.UtcNow;
        }
        else
        {
            if (approval.CurrentStepNumber >= approval.TotalSteps)
            {
                approval.Status = ApprovalStatus.Approved;
                approval.CompletedAt = DateTime.UtcNow;
            }
            else
            {
                approval.CurrentStepNumber++;
            }
        }

        // Notify Requester
        var notif = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = approval.RequesterId,
            Title = request.IsApproved ? "Approval Step Cleared" : "Approval Request Rejected",
            Message = $"Step {stepNumber} for '{approval.Title}' was marked {(request.IsApproved ? "Approved" : "Rejected")}.",
            Type = request.IsApproved ? NotificationType.Success : NotificationType.Alert,
            TargetUrl = "/approvals",
            CreatedAt = DateTime.UtcNow
        };
        _context.Notifications.Add(notif);
        await _notificationHubService.SendToUserAsync(approval.RequesterId, "ReceiveNotification", notif);

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<bool>.SuccessResult(true, $"Step {stepNumber} successfully processed."));
    }
}
