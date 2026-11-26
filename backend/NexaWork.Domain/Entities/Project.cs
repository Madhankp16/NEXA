using System;
using System.Collections.Generic;
using NexaWork.Domain.Common;
using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class Project : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty; // e.g. "NEXA", "ENG"
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public Guid OwnerId { get; set; }
    public User? Owner { get; set; }

    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}

public class ProjectMember : BaseEntity
{
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public ProjectMemberRole RoleInProject { get; set; } = ProjectMemberRole.Contributor;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

public class TaskItem : BaseEntity
{
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public int OrderIndex { get; set; } = 0;

    public Guid? AssigneeId { get; set; }
    public User? Assignee { get; set; }

    public Guid ReporterId { get; set; }
    public User? Reporter { get; set; }

    public DateTime? DueDate { get; set; }
    public double EstimatedHours { get; set; } = 0;
    public double LoggedHours { get; set; } = 0;

    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    public ICollection<TaskAttachment> Attachments { get; set; } = new List<TaskAttachment>();
}

public class TaskComment : BaseEntity
{
    public Guid TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string CommentText { get; set; } = string.Empty;
}

public class TaskAttachment : BaseEntity
{
    public Guid TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;

    public Guid UploadedById { get; set; }
    public User? UploadedBy { get; set; }
}
