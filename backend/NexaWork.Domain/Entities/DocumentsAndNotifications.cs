using System;
using System.Collections.Generic;
using NexaWork.Domain.Common;
using NexaWork.Domain.Enums;

namespace NexaWork.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public bool IsRead { get; set; } = false;
    public string? TargetUrl { get; set; }
}

public class EmailLog : BaseEntity
{
    public string RecipientEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = "Sent"; // Sent, Failed, Queued
    public string? ErrorMessage { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}

public class Document : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = "General"; // HR, Finance, Legal, Project, General
    public int CurrentVersion { get; set; } = 1;
    public DocumentAccessLevel AccessLevel { get; set; } = DocumentAccessLevel.Internal;
    public Guid OwnerId { get; set; }
    public User? Owner { get; set; }

    public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
}

public class DocumentVersion : BaseEntity
{
    public Guid DocumentId { get; set; }
    public Document? Document { get; set; }

    public int VersionNumber { get; set; } = 1;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string? ChangeSummary { get; set; }
    public Guid UploadedById { get; set; }
    public User? UploadedBy { get; set; }
}
