using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NexaWork.Domain.Entities;
using NexaWork.Domain.Enums;

namespace NexaWork.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure Database is created
        await context.Database.EnsureCreatedAsync();

        if (await context.Organizations.AnyAsync())
        {
            return; // Already seeded
        }

        // 1. Create Organization
        var org = new Organization
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Nexa Enterprise Solutions",
            Code = "NEXA-ENT",
            Domain = "nexawork.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Organizations.Add(org);

        // 2. Departments
        var engineeringDept = new Department
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            OrganizationId = org.Id,
            Name = "Engineering & Technology",
            Code = "ENG",
            Description = "Software development, architecture, QA, and cloud DevOps infrastructure",
            CreatedAt = DateTime.UtcNow
        };

        var productDept = new Department
        {
            Id = Guid.Parse("22222222-2222-2222-2222-333333333333"),
            OrganizationId = org.Id,
            Name = "Product & Design",
            Code = "PROD",
            Description = "Product vision, UX/UI design systems, and client delivery",
            CreatedAt = DateTime.UtcNow
        };

        var operationsDept = new Department
        {
            Id = Guid.Parse("22222222-2222-2222-2222-444444444444"),
            OrganizationId = org.Id,
            Name = "Operations & HR",
            Code = "OPS",
            Description = "People operations, talent acquisition, legal compliance, and facilities",
            CreatedAt = DateTime.UtcNow
        };

        context.Departments.AddRange(engineeringDept, productDept, operationsDept);

        // 3. Teams
        var backendTeam = new Team
        {
            Id = Guid.NewGuid(),
            DepartmentId = engineeringDept.Id,
            Name = "Core Platform Services",
            Description = "Distributed .NET microservices, PostgreSQL, caching & API gateways",
            CreatedAt = DateTime.UtcNow
        };

        var frontendTeam = new Team
        {
            Id = Guid.NewGuid(),
            DepartmentId = engineeringDept.Id,
            Name = "Web & Mobile Experience",
            Description = "React, TypeScript, Redux Toolkit, and UI architecture",
            CreatedAt = DateTime.UtcNow
        };

        context.Teams.AddRange(backendTeam, frontendTeam);

        // 4. Permissions
        var permissions = new List<Permission>
        {
            new() { Id = Guid.NewGuid(), Name = "Org.Manage", Category = "Organization", Description = "Manage company settings and hierarchy" },
            new() { Id = Guid.NewGuid(), Name = "Projects.Create", Category = "Projects", Description = "Create new enterprise projects" },
            new() { Id = Guid.NewGuid(), Name = "Projects.Manage", Category = "Projects", Description = "Update, archive, assign members to projects" },
            new() { Id = Guid.NewGuid(), Name = "Tasks.Manage", Category = "Tasks", Description = "Create and move tasks on Kanban board" },
            new() { Id = Guid.NewGuid(), Name = "Workflow.Approve", Category = "Workflow", Description = "Approve or reject multi-level approval requests" },
            new() { Id = Guid.NewGuid(), Name = "Reports.View", Category = "Reports", Description = "View executive KPIs and analytics dashboards" },
            new() { Id = Guid.NewGuid(), Name = "Documents.Manage", Category = "Documents", Description = "Upload, categorize and manage enterprise docs" }
        };
        context.Permissions.AddRange(permissions);

        // 5. Roles
        var adminRole = new Role
        {
            Id = Guid.Parse("33333333-3333-3333-3333-111111111111"),
            Name = "SuperAdmin",
            Description = "Enterprise organization super administrator with full system permissions",
            IsSystemRole = true,
            CreatedAt = DateTime.UtcNow
        };

        var managerRole = new Role
        {
            Id = Guid.Parse("33333333-3333-3333-3333-222222222222"),
            Name = "Manager",
            Description = "Department Manager with approval and project oversight authority",
            IsSystemRole = true,
            CreatedAt = DateTime.UtcNow
        };

        var employeeRole = new Role
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Name = "Employee",
            Description = "Standard staff contributor with task and document access",
            IsSystemRole = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Roles.AddRange(adminRole, managerRole, employeeRole);

        // Map All Permissions to SuperAdmin
        foreach (var p in permissions)
        {
            context.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = p.Id });
        }

        // 6. Users
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");

        var adminUser = new User
        {
            Id = Guid.Parse("44444444-4444-4444-4444-111111111111"),
            OrganizationId = org.Id,
            DepartmentId = engineeringDept.Id,
            TeamId = backendTeam.Id,
            Email = "admin@nexawork.com",
            PasswordHash = hashedPassword,
            FirstName = "Alex",
            LastName = "Vance",
            JobTitle = "Principal Architect & Admin",
            RoleType = UserRoleType.SuperAdmin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var managerUser = new User
        {
            Id = Guid.Parse("44444444-4444-4444-4444-222222222222"),
            OrganizationId = org.Id,
            DepartmentId = engineeringDept.Id,
            TeamId = backendTeam.Id,
            Email = "sarah.manager@nexawork.com",
            PasswordHash = hashedPassword,
            FirstName = "Sarah",
            LastName = "Connor",
            JobTitle = "VP of Engineering",
            RoleType = UserRoleType.Manager,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var devUser = new User
        {
            Id = Guid.Parse("44444444-4444-4444-4444-333333333333"),
            OrganizationId = org.Id,
            DepartmentId = engineeringDept.Id,
            TeamId = frontendTeam.Id,
            Email = "dev@nexawork.com",
            PasswordHash = hashedPassword,
            FirstName = "Marcus",
            LastName = "Chen",
            JobTitle = "Senior Full Stack Engineer",
            RoleType = UserRoleType.Employee,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(adminUser, managerUser, devUser);

        // Assign User Roles
        context.UserRoles.AddRange(
            new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id },
            new UserRole { UserId = managerUser.Id, RoleId = managerRole.Id },
            new UserRole { UserId = devUser.Id, RoleId = employeeRole.Id }
        );

        // 7. Projects
        var nexaProject = new Project
        {
            Id = Guid.Parse("55555555-5555-5555-5555-111111111111"),
            OrganizationId = org.Id,
            Name = "NexaWork Enterprise Cloud Platform",
            Key = "NEXA",
            Description = "Unified corporate portal with workflow automation, SignalR telemetry, and Kanban boards",
            Status = ProjectStatus.Active,
            Priority = PriorityLevel.Urgent,
            StartDate = DateTime.UtcNow.AddMonths(-1),
            EndDate = DateTime.UtcNow.AddMonths(5),
            Budget = 150000m,
            OwnerId = managerUser.Id,
            CreatedAt = DateTime.UtcNow
        };

        var mobileAppProject = new Project
        {
            Id = Guid.Parse("55555555-5555-5555-5555-222222222222"),
            OrganizationId = org.Id,
            Name = "Nexa Workforce Mobile Native App",
            Key = "MOBI",
            Description = "Cross-platform enterprise workforce companion for approvals, OTP, and push notifications",
            Status = ProjectStatus.Planning,
            Priority = PriorityLevel.High,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(4),
            Budget = 85000m,
            OwnerId = managerUser.Id,
            CreatedAt = DateTime.UtcNow
        };

        context.Projects.AddRange(nexaProject, mobileAppProject);

        // Project Members
        context.ProjectMembers.AddRange(
            new ProjectMember { ProjectId = nexaProject.Id, UserId = adminUser.Id, RoleInProject = ProjectMemberRole.Owner },
            new ProjectMember { ProjectId = nexaProject.Id, UserId = managerUser.Id, RoleInProject = ProjectMemberRole.Lead },
            new ProjectMember { ProjectId = nexaProject.Id, UserId = devUser.Id, RoleInProject = ProjectMemberRole.Contributor }
        );

        // 8. Tasks (Kanban items)
        var tasks = new List<TaskItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProjectId = nexaProject.Id,
                Title = "Implement Clean Architecture solution & EF Core migrations",
                Description = "Structure Domain, Application, Infrastructure and Web API layers with PostgreSQL provider.",
                Status = TaskItemStatus.Done,
                Priority = PriorityLevel.High,
                OrderIndex = 0,
                AssigneeId = devUser.Id,
                ReporterId = managerUser.Id,
                EstimatedHours = 16,
                LoggedHours = 16,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProjectId = nexaProject.Id,
                Title = "Configure JWT Authentication with Refresh Token rotation",
                Description = "Secure endpoints with Bearer tokens, ClaimsPrincipal parsing, and automated refresh handling.",
                Status = TaskItemStatus.InProgress,
                Priority = PriorityLevel.Urgent,
                OrderIndex = 1,
                AssigneeId = devUser.Id,
                ReporterId = managerUser.Id,
                EstimatedHours = 12,
                LoggedHours = 8,
                DueDate = DateTime.UtcNow.AddDays(2),
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProjectId = nexaProject.Id,
                Title = "Build Interactive Multi-Level Approval Workflow Engine",
                Description = "Enable multi-stage approvals for leave requests, equipment expenses, and budget releases.",
                Status = TaskItemStatus.InProgress,
                Priority = PriorityLevel.High,
                OrderIndex = 2,
                AssigneeId = adminUser.Id,
                ReporterId = managerUser.Id,
                EstimatedHours = 24,
                LoggedHours = 10,
                DueDate = DateTime.UtcNow.AddDays(4),
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProjectId = nexaProject.Id,
                Title = "Design Real-Time SignalR Notification Broadcasts",
                Description = "Push instant alerts to users upon approval changes, task assignments, and system broadcasts.",
                Status = TaskItemStatus.Todo,
                Priority = PriorityLevel.Medium,
                OrderIndex = 3,
                AssigneeId = devUser.Id,
                ReporterId = adminUser.Id,
                EstimatedHours = 8,
                DueDate = DateTime.UtcNow.AddDays(6),
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProjectId = nexaProject.Id,
                Title = "Redis Cache Layer & Dashboard KPI Aggregations",
                Description = "Optimize dashboard metric calculation queries with 10-minute sliding Redis cache.",
                Status = TaskItemStatus.InReview,
                Priority = PriorityLevel.Medium,
                OrderIndex = 4,
                AssigneeId = devUser.Id,
                ReporterId = adminUser.Id,
                EstimatedHours = 10,
                LoggedHours = 10,
                DueDate = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            }
        };

        context.Tasks.AddRange(tasks);

        // 9. Workflows & Approvals
        var approval = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            RequesterId = devUser.Id,
            Title = "Cloud Infrastructure Annual License Renewal",
            Description = "Requesting approval to renew AWS & PostgreSQL production cluster subscription for 12 months.",
            WorkflowType = WorkflowType.BudgetApproval,
            Status = ApprovalStatus.Pending,
            CurrentStepNumber = 1,
            TotalSteps = 2,
            PayloadJson = "{\"Amount\": 14200, \"Vendor\": \"AWS Enterprise\", \"Department\": \"Engineering\"}",
            CreatedAt = DateTime.UtcNow.AddHours(-18)
        };

        var step1 = new ApprovalStep
        {
            Id = Guid.NewGuid(),
            ApprovalRequestId = approval.Id,
            StepNumber = 1,
            StepName = "Department Manager Approval",
            ApproverUserId = managerUser.Id,
            ApproverRole = "Manager",
            Status = StepApprovalStatus.Pending
        };

        var step2 = new ApprovalStep
        {
            Id = Guid.NewGuid(),
            ApprovalRequestId = approval.Id,
            StepNumber = 2,
            StepName = "Executive VP Sign-Off",
            ApproverUserId = adminUser.Id,
            ApproverRole = "SuperAdmin",
            Status = StepApprovalStatus.Pending
        };

        context.ApprovalRequests.Add(approval);
        context.ApprovalSteps.AddRange(step1, step2);

        // 10. Documents
        var doc1 = new Document
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            Title = "NexaWork Enterprise Architecture Specification 2026.pdf",
            Category = "Project",
            AccessLevel = DocumentAccessLevel.Internal,
            OwnerId = adminUser.Id,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        var doc1Ver = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = doc1.Id,
            VersionNumber = 1,
            FileName = "NexaWork_Architecture_Spec.pdf",
            FilePath = "/storage/documents/NexaWork_Architecture_Spec.pdf",
            FileSize = 4851200,
            ContentType = "application/pdf",
            ChangeSummary = "Initial system blueprint and data schema definition.",
            UploadedById = adminUser.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        var doc2 = new Document
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            Title = "Company Employee Handbook & Security Guidelines 2026.docx",
            Category = "HR",
            AccessLevel = DocumentAccessLevel.Public,
            OwnerId = managerUser.Id,
            CurrentVersion = 2,
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        };

        context.Documents.AddRange(doc1, doc2);
        context.DocumentVersions.Add(doc1Ver);

        // 11. Notifications
        var notifications = new List<Notification>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = devUser.Id,
                Title = "New Task Assigned",
                Message = "You were assigned to 'Configure JWT Authentication with Refresh Token rotation'",
                Type = NotificationType.Info,
                IsRead = false,
                TargetUrl = "/tasks",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = managerUser.Id,
                Title = "Pending Approval Required",
                Message = "Marcus Chen submitted 'Cloud Infrastructure Annual License Renewal' for your review.",
                Type = NotificationType.Workflow,
                IsRead = false,
                TargetUrl = "/approvals",
                CreatedAt = DateTime.UtcNow.AddHours(-18)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = adminUser.Id,
                Title = "System Health Optimal",
                Message = "Database migrations and Redis cache connection initialized successfully.",
                Type = NotificationType.Success,
                IsRead = true,
                TargetUrl = "/dashboard",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        context.Notifications.AddRange(notifications);

        await context.SaveChangesAsync();
    }
}
