namespace NexaWork.Domain.Enums;

public enum UserRoleType
{
    SuperAdmin = 1,
    Admin = 2,
    Manager = 3,
    TeamLead = 4,
    Employee = 5,
    Guest = 6
}

public enum ProjectStatus
{
    Planning = 1,
    Active = 2,
    OnHold = 3,
    Completed = 4,
    Archived = 5
}

public enum PriorityLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

public enum TaskItemStatus
{
    Todo = 1,
    InProgress = 2,
    InReview = 3,
    Done = 4
}

public enum ProjectMemberRole
{
    Owner = 1,
    Lead = 2,
    Contributor = 3,
    Viewer = 4
}

public enum WorkflowType
{
    LeaveRequest = 1,
    BudgetApproval = 2,
    ExpenseClaim = 3,
    DocumentSignOff = 4,
    ProjectInitiation = 5
}

public enum ApprovalStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4
}

public enum StepApprovalStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Skipped = 4
}

public enum NotificationType
{
    Info = 1,
    Success = 2,
    Warning = 3,
    Alert = 4,
    Workflow = 5
}

public enum DocumentAccessLevel
{
    Public = 1,
    Internal = 2,
    Confidential = 3,
    Restricted = 4
}
