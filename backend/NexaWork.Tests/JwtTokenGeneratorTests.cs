using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NexaWork.Domain.Entities;
using NexaWork.Domain.Enums;
using NexaWork.Infrastructure.Services;
using Xunit;

namespace NexaWork.Tests;

public class JwtTokenGeneratorTests
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly JwtTokenGenerator _jwtGenerator;

    public JwtTokenGeneratorTests()
    {
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["JwtSettings:Secret"]).Returns("EnterpriseNexaWorkSuperSecretKeyForJwt20261125VeryLongKey!");
        _configMock.Setup(c => c["JwtSettings:Issuer"]).Returns("NexaWork");
        _configMock.Setup(c => c["JwtSettings:Audience"]).Returns("NexaWorkClient");
        _configMock.Setup(c => c["JwtSettings:ExpiryMinutes"]).Returns("60");

        _jwtGenerator = new JwtTokenGenerator(_configMock.Object);
    }

    [Fact]
    public void GenerateAccessToken_Should_Return_Valid_Jwt_Token_With_Claims()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            Email = "architect@nexawork.com",
            FirstName = "Alex",
            LastName = "Vance",
            RoleType = UserRoleType.SuperAdmin
        };

        var roles = new List<string> { "SuperAdmin" };
        var permissions = new List<string> { "Projects.Create", "Workflow.Approve" };

        // Act
        var token = _jwtGenerator.GenerateAccessToken(user, roles, permissions);

        // Assert
        token.Should().NotBeNullOrWhiteSpace();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Issuer.Should().Be("NexaWork");
        jwtToken.Audiences.Should().Contain("NexaWorkClient");

        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);
        emailClaim.Should().NotBeNull();
        emailClaim!.Value.Should().Be("architect@nexawork.com");

        var permissionClaims = jwtToken.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();
        permissionClaims.Should().Contain("Projects.Create");
        permissionClaims.Should().Contain("Workflow.Approve");
    }

    [Fact]
    public void GenerateRefreshToken_Should_Return_Secure_Random_Token()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var refreshToken = _jwtGenerator.GenerateRefreshToken(userId);

        // Assert
        refreshToken.Should().NotBeNull();
        refreshToken.Token.Should().NotBeNullOrWhiteSpace();
        refreshToken.UserId.Should().Be(userId);
        refreshToken.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        refreshToken.IsActive.Should().BeTrue();
    }
}
