using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Models;
using NexaWork.Application.DTOs;

namespace NexaWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public OrganizationsController(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    [HttpGet("current")]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> GetCurrentOrganization()
    {
        var orgId = _currentUserService.OrganizationId;
        if (!orgId.HasValue)
        {
            return NotFound(ApiResponse<OrganizationDto>.FailureResult("No organization context found."));
        }

        var org = await _context.Organizations
            .Include(o => o.Users)
            .Include(o => o.Projects)
            .FirstOrDefaultAsync(o => o.Id == orgId.Value);

        if (org == null)
        {
            return NotFound(ApiResponse<OrganizationDto>.FailureResult("Organization not found"));
        }

        var dto = new OrganizationDto
        {
            Id = org.Id,
            Name = org.Name,
            Code = org.Code,
            Domain = org.Domain,
            EmployeeCount = org.Users.Count,
            ProjectCount = org.Projects.Count,
            CreatedAt = org.CreatedAt
        };

        return Ok(ApiResponse<OrganizationDto>.SuccessResult(dto));
    }

    [HttpGet("departments")]
    public async Task<ActionResult<ApiResponse<List<DepartmentDto>>>> GetDepartments()
    {
        var orgId = _currentUserService.OrganizationId;
        var depts = await _context.Departments
            .Where(d => !orgId.HasValue || d.OrganizationId == orgId.Value)
            .Include(d => d.Manager)
            .Include(d => d.Teams)
            .Include(d => d.Users)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                OrganizationId = d.OrganizationId,
                Name = d.Name,
                Code = d.Code,
                Description = d.Description,
                ManagerName = d.Manager != null ? d.Manager.FullName : null,
                TeamCount = d.Teams.Count,
                MemberCount = d.Users.Count
            })
            .ToListAsync();

        return Ok(ApiResponse<List<DepartmentDto>>.SuccessResult(depts));
    }

    [HttpGet("teams")]
    public async Task<ActionResult<ApiResponse<List<TeamDto>>>> GetTeams()
    {
        var teams = await _context.Teams
            .Include(t => t.Department)
            .Include(t => t.TeamLead)
            .Include(t => t.Members)
            .Select(t => new TeamDto
            {
                Id = t.Id,
                DepartmentId = t.DepartmentId,
                DepartmentName = t.Department != null ? t.Department.Name : "",
                Name = t.Name,
                Description = t.Description,
                TeamLeadName = t.TeamLead != null ? t.TeamLead.FullName : null,
                MemberCount = t.Members.Count
            })
            .ToListAsync();

        return Ok(ApiResponse<List<TeamDto>>.SuccessResult(teams));
    }

    [HttpGet("members")]
    public async Task<ActionResult<ApiResponse<List<UserProfileDto>>>> GetMembers()
    {
        var orgId = _currentUserService.OrganizationId;
        var members = await _context.Users
            .Where(u => !orgId.HasValue || u.OrganizationId == orgId.Value)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Select(u => new UserProfileDto
            {
                Id = u.Id,
                OrganizationId = u.OrganizationId,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                JobTitle = u.JobTitle,
                AvatarUrl = u.AvatarUrl,
                Role = u.UserRoles.Select(ur => ur.Role!.Name).FirstOrDefault() ?? u.RoleType.ToString()
            })
            .ToListAsync();

        return Ok(ApiResponse<List<UserProfileDto>>.SuccessResult(members));
    }
}
