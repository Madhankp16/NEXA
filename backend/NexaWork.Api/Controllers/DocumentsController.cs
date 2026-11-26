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
public class DocumentsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DocumentsController(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<DocumentDto>>>> GetAll([FromQuery] string? category)
    {
        var orgId = _currentUserService.OrganizationId;
        var query = _context.Documents.AsQueryable();

        if (orgId.HasValue)
        {
            query = query.Where(d => d.OrganizationId == orgId.Value);
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(d => d.Category.ToLower() == category.ToLower());
        }

        var docs = await query
            .Include(d => d.Owner)
            .Include(d => d.Versions)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new DocumentDto
            {
                Id = d.Id,
                Title = d.Title,
                Category = d.Category,
                CurrentVersion = d.CurrentVersion,
                AccessLevel = d.AccessLevel,
                OwnerName = d.Owner != null ? d.Owner.FullName : "System",
                LatestFileName = d.Versions.OrderByDescending(v => v.VersionNumber).Select(v => v.FileName).FirstOrDefault(),
                LatestFileSize = d.Versions.OrderByDescending(v => v.VersionNumber).Select(v => v.FileSize).FirstOrDefault(),
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<DocumentDto>>.SuccessResult(docs));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DocumentDto>>> CreateDocument([FromBody] DocumentDto input)
    {
        var userId = _currentUserService.UserId;
        var orgId = _currentUserService.OrganizationId;

        if (!userId.HasValue || !orgId.HasValue)
        {
            return Unauthorized(ApiResponse<DocumentDto>.FailureResult("Unauthorized"));
        }

        var doc = new Document
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId.Value,
            Title = input.Title,
            Category = string.IsNullOrWhiteSpace(input.Category) ? "General" : input.Category,
            AccessLevel = input.AccessLevel,
            OwnerId = userId.Value,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow
        };

        var initialVersion = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = doc.Id,
            VersionNumber = 1,
            FileName = input.LatestFileName ?? $"{doc.Title}.pdf",
            FilePath = $"/storage/docs/{doc.Id}_v1.pdf",
            FileSize = input.LatestFileSize > 0 ? input.LatestFileSize : 1024 * 350,
            ContentType = "application/pdf",
            UploadedById = userId.Value,
            CreatedAt = DateTime.UtcNow
        };

        _context.Documents.Add(doc);
        _context.DocumentVersions.Add(initialVersion);
        await _context.SaveChangesAsync();

        input.Id = doc.Id;
        input.OwnerName = "You";
        input.CreatedAt = doc.CreatedAt;

        return Ok(ApiResponse<DocumentDto>.SuccessResult(input, "Document created successfully"));
    }
}
