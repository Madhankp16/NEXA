using System;
using System.Collections.Generic;
using NexaWork.Domain.Enums;

namespace NexaWork.Application.DTOs;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string OrganizationName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
}

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserProfileDto User { get; set; } = new();
}

public class RefreshTokenRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class UserProfileDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string? JobTitle { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}

public class OrganizationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public int ProjectCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DepartmentDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ManagerName { get; set; }
    public int TeamCount { get; set; }
    public int MemberCount { get; set; }
}

public class TeamDto
{
    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? TeamLeadName { get; set; }
    public int MemberCount { get; set; }
}

public class ProjectDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public int CompletedTaskCount { get; set; }
    public int MemberCount { get; set; }
    public double ProgressPercentage => TaskCount == 0 ? 0 : Math.Round((double)CompletedTaskCount / TaskCount * 100, 1);
}

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
}

public class TaskItemDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public PriorityLevel Priority { get; set; }
    public int OrderIndex { get; set; }
    public Guid? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public string? AssigneeAvatar { get; set; }
    public Guid ReporterId { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public double EstimatedHours { get; set; }
    public double LoggedHours { get; set; }
    public int CommentCount { get; set; }
    public int AttachmentCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTaskDto
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public Guid? AssigneeId { get; set; }
    public DateTime? DueDate { get; set; }
    public double EstimatedHours { get; set; }
}

public class UpdateTaskStatusDto
{
    public TaskItemStatus Status { get; set; }
    public int OrderIndex { get; set; }
}

public class WorkflowDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public WorkflowType WorkflowType { get; set; }
    public string? Description { get; set; }
    public int StepCount { get; set; }
    public bool IsActive { get; set; }
}

public class ApprovalRequestDto
{
    public Guid Id { get; set; }
    public Guid RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string RequesterAvatar { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkflowType WorkflowType { get; set; }
    public ApprovalStatus Status { get; set; }
    public int CurrentStepNumber { get; set; }
    public int TotalSteps { get; set; }
    public string PayloadJson { get; set; } = "{}";
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<ApprovalStepDto> Steps { get; set; } = new();
}

public class ApprovalStepDto
{
    public Guid Id { get; set; }
    public int StepNumber { get; set; }
    public string StepName { get; set; } = string.Empty;
    public Guid? ApproverUserId { get; set; }
    public string? ApproverUserName { get; set; }
    public string? ApproverRole { get; set; }
    public StepApprovalStatus Status { get; set; }
    public string? Comments { get; set; }
    public DateTime? ActionDate { get; set; }
}

public class ProcessApprovalStepDto
{
    public bool IsApproved { get; set; }
    public string? Comments { get; set; }
}

public class CreateApprovalRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkflowType WorkflowType { get; set; } = WorkflowType.LeaveRequest;
    public string PayloadJson { get; set; } = "{}";
}

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public string? TargetUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int CurrentVersion { get; set; }
    public DocumentAccessLevel AccessLevel { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string? LatestFileName { get; set; }
    public long LatestFileSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class DashboardKpiDto
{
    public int TotalEmployees { get; set; }
    public int TotalProjects { get; set; }
    public int ActiveTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingApprovals { get; set; }
    public int TotalDocuments { get; set; }
    public double ProjectCompletionRate { get; set; }
    public List<DepartmentDistributionDto> DepartmentDistributions { get; set; } = new();
    public List<MonthlyTaskActivityDto> MonthlyActivities { get; set; } = new();
    public List<RecentApprovalItemDto> RecentApprovals { get; set; } = new();
}

public class DepartmentDistributionDto
{
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
}

public class MonthlyTaskActivityDto
{
    public string Month { get; set; } = string.Empty;
    public int Created { get; set; }
    public int Completed { get; set; }
}

public class RecentApprovalItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
