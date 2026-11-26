using System;
using System.Collections.Generic;
using NexaWork.Domain.Common;
using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class WorkflowDefinition : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string Name { get; set; } = string.Empty;
    public WorkflowType WorkflowType { get; set; }
    public string? Description { get; set; }
    public int StepCount { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public string StepsConfigJson { get; set; } = "[]"; // Dynamic steps configuration

    public ICollection<ApprovalRequest> ApprovalRequests { get; set; } = new List<ApprovalRequest>();
}

public class ApprovalRequest : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public Guid RequesterId { get; set; }
    public User? Requester { get; set; }

    public Guid? WorkflowDefinitionId { get; set; }
    public WorkflowDefinition? WorkflowDefinition { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkflowType WorkflowType { get; set; } = WorkflowType.LeaveRequest;
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public int CurrentStepNumber { get; set; } = 1;
    public int TotalSteps { get; set; } = 1;
    public string PayloadJson { get; set; } = "{}"; // Form submission details
    public DateTime? CompletedAt { get; set; }

    public ICollection<ApprovalStep> Steps { get; set; } = new List<ApprovalStep>();
    public ICollection<ApprovalHistory> History { get; set; } = new List<ApprovalHistory>();
}

public class ApprovalStep : BaseEntity
{
    public Guid ApprovalRequestId { get; set; }
    public ApprovalRequest? ApprovalRequest { get; set; }

    public int StepNumber { get; set; }
    public string StepName { get; set; } = string.Empty;
    
    public Guid? ApproverUserId { get; set; }
    public User? ApproverUser { get; set; }

    public string? ApproverRole { get; set; }
    public StepApprovalStatus Status { get; set; } = StepApprovalStatus.Pending;
    public string? Comments { get; set; }
    public DateTime? ActionDate { get; set; }
}

public class ApprovalHistory : BaseEntity
{
    public Guid ApprovalRequestId { get; set; }
    public ApprovalRequest? ApprovalRequest { get; set; }

    public Guid ActionByUserId { get; set; }
    public User? ActionByUser { get; set; }

    public string Action { get; set; } = string.Empty; // Submitted, Approved, Rejected, Commented
    public string? Comments { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
