using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Models;
using NexaWork.Application.DTOs;
using NexaWork.Domain.Enums;

namespace NexaWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ICurrentUserService _currentUserService;

    public DashboardController(
        IApplicationDbContext context,
        ICacheService cacheService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _cacheService = cacheService;
        _currentUserService = currentUserService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<DashboardKpiDto>>> GetSummary()
    {
        var orgId = _currentUserService.OrganizationId;
        var cacheKey = $"dashboard_kpi_{orgId}";

        var cached = await _cacheService.GetAsync<DashboardKpiDto>(cacheKey);
        if (cached != null)
        {
            return Ok(ApiResponse<DashboardKpiDto>.SuccessResult(cached, "Loaded from cache"));
        }

        var totalEmployees = await _context.Users.CountAsync(u => !orgId.HasValue || u.OrganizationId == orgId.Value);
        var totalProjects = await _context.Projects.CountAsync(p => !orgId.HasValue || p.OrganizationId == orgId.Value);
        var activeTasks = await _context.Tasks.CountAsync(t => t.Status != TaskItemStatus.Done);
        var completedTasks = await _context.Tasks.CountAsync(t => t.Status == TaskItemStatus.Done);
        var pendingApprovals = await _context.ApprovalRequests.CountAsync(a => a.Status == ApprovalStatus.Pending);
        var totalDocs = await _context.Documents.CountAsync(d => !orgId.HasValue || d.OrganizationId == orgId.Value);

        var totalTasks = activeTasks + completedTasks;
        var completionRate = totalTasks == 0 ? 0 : Math.Round((double)completedTasks / totalTasks * 100, 1);

        var depts = await _context.Departments
            .Include(d => d.Users)
            .Select(d => new DepartmentDistributionDto
            {
                DepartmentName = d.Name,
                EmployeeCount = d.Users.Count
            })
            .ToListAsync();

        var recentApprovals = await _context.ApprovalRequests
            .Include(ar => ar.Requester)
            .OrderByDescending(ar => ar.CreatedAt)
            .Take(5)
            .Select(ar => new RecentApprovalItemDto
            {
                Id = ar.Id,
                Title = ar.Title,
                RequesterName = ar.Requester != null ? ar.Requester.FullName : "System",
                Type = ar.WorkflowType.ToString(),
                Status = ar.Status.ToString(),
                CreatedAt = ar.CreatedAt
            })
            .ToListAsync();

        var kpi = new DashboardKpiDto
        {
            TotalEmployees = totalEmployees,
            TotalProjects = totalProjects,
            ActiveTasks = activeTasks,
            CompletedTasks = completedTasks,
            PendingApprovals = pendingApprovals,
            TotalDocuments = totalDocs,
            ProjectCompletionRate = completionRate,
            DepartmentDistributions = depts,
            MonthlyActivities = new List<MonthlyTaskActivityDto>
            {
                new() { Month = "Jul", Created = 32, Completed = 28 },
                new() { Month = "Aug", Created = 45, Completed = 40 },
                new() { Month = "Sep", Created = 52, Completed = 48 },
                new() { Month = "Oct", Created = 64, Completed = 58 },
                new() { Month = "Nov", Created = 70, Completed = 65 }
            },
            RecentApprovals = recentApprovals
        };

        await _cacheService.SetAsync(cacheKey, kpi, TimeSpan.FromMinutes(5));

        return Ok(ApiResponse<DashboardKpiDto>.SuccessResult(kpi));
    }
}
