using System;
using System.Collections.Generic;
using NexaWork.Domain.Common;

namespace NexaWork.Domain.Entities;

public class Organization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<WorkflowDefinition> WorkflowDefinitions { get; set; } = new List<WorkflowDefinition>();
}

public class Department : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ManagerId { get; set; }
    public User? Manager { get; set; }

    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<User> Users { get; set; } = new List<User>();
}

public class Team : BaseEntity
{
    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? TeamLeadId { get; set; }
    public User? TeamLead { get; set; }

    public ICollection<User> Members { get; set; } = new List<User>();
}
