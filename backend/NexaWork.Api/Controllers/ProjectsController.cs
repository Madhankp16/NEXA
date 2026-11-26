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
public class ProjectsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ProjectsController(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProjectDto>>>> GetAll()
    {
        var orgId = _currentUserService.OrganizationId;
        var query = _context.Projects.AsQueryable();

        if (orgId.HasValue)
        {
            query = query.Where(p => p.OrganizationId == orgId.Value);
        }

        var projects = await query
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .Include(p => p.Members)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                OrganizationId = p.OrganizationId,
                Name = p.Name,
                Key = p.Key,
                Description = p.Description,
                Status = p.Status,
                Priority = p.Priority,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Budget = p.Budget,
                OwnerId = p.OwnerId,
                OwnerName = p.Owner != null ? p.Owner.FullName : "System",
                TaskCount = p.Tasks.Count,
                CompletedTaskCount = p.Tasks.Count(t => t.Status == TaskItemStatus.Done),
                MemberCount = p.Members.Count
            })
            .ToListAsync();

        return Ok(ApiResponse<List<ProjectDto>>.SuccessResult(projects));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProjectDto>>> GetById(Guid id)
    {
        var project = await _context.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return NotFound(ApiResponse<ProjectDto>.FailureResult("Project not found."));
        }

        var dto = new ProjectDto
        {
            Id = project.Id,
            OrganizationId = project.OrganizationId,
            Name = project.Name,
            Key = project.Key,
            Description = project.Description,
            Status = project.Status,
            Priority = project.Priority,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            OwnerId = project.OwnerId,
            OwnerName = project.Owner?.FullName ?? "System",
            TaskCount = project.Tasks.Count,
            CompletedTaskCount = project.Tasks.Count(t => t.Status == TaskItemStatus.Done),
            MemberCount = project.Members.Count
        };

        return Ok(ApiResponse<ProjectDto>.SuccessResult(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProjectDto>>> Create([FromBody] CreateProjectDto request)
    {
        var userId = _currentUserService.UserId;
        var orgId = _currentUserService.OrganizationId;

        if (!userId.HasValue || !orgId.HasValue)
        {
            return Unauthorized(ApiResponse<ProjectDto>.FailureResult("Unauthorized request."));
        }

        var project = new Project
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId.Value,
            OwnerId = userId.Value,
            Name = request.Name,
            Key = request.Key.ToUpper().Trim(),
            Description = request.Description,
            Priority = request.Priority,
            Status = ProjectStatus.Active,
            StartDate = request.StartDate ?? DateTime.UtcNow,
            EndDate = request.EndDate,
            Budget = request.Budget,
            CreatedAt = DateTime.UtcNow
        };

        project.Members.Add(new ProjectMember
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            UserId = userId.Value,
            RoleInProject = ProjectMemberRole.Owner,
            JoinedAt = DateTime.UtcNow
        });

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var dto = new ProjectDto
        {
            Id = project.Id,
            OrganizationId = project.OrganizationId,
            Name = project.Name,
            Key = project.Key,
            Description = project.Description,
            Status = project.Status,
            Priority = project.Priority,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            OwnerId = project.OwnerId,
            OwnerName = "You",
            TaskCount = 0,
            CompletedTaskCount = 0,
            MemberCount = 1
        };

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, ApiResponse<ProjectDto>.SuccessResult(dto, "Project created successfully"));
    }
}
