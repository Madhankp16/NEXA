using FluentAssertions;
using NexaWork.Application.DTOs;
using NexaWork.Application.Validators;
using Xunit;

namespace NexaWork.Tests;

public class ValidatorTests
{
    private readonly LoginRequestValidator _loginValidator = new();
    private readonly CreateProjectValidator _projectValidator = new();

    [Theory]
    [InlineData("valid@nexawork.com", "Password123!", true)]
    [InlineData("", "Password123!", false)]
    [InlineData("not-an-email", "Password123!", false)]
    [InlineData("valid@nexawork.com", "123", false)]
    public void LoginRequestValidator_Should_Validate_Correctly(string email, string password, bool expectedValid)
    {
        var model = new LoginRequest { Email = email, Password = password };
        var result = _loginValidator.Validate(model);
        result.IsValid.Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("Platform Core", "NEXA", true)]
    [InlineData("", "NEXA", false)]
    [InlineData("Platform Core", "a", false)] // too short
    [InlineData("Platform Core", "nexa-1", false)] // lowercase/hyphen not allowed
    public void CreateProjectValidator_Should_Validate_Project_Key_Format(string name, string key, bool expectedValid)
    {
        var model = new CreateProjectDto { Name = name, Key = key };
        var result = _projectValidator.Validate(model);
        result.IsValid.Should().Be(expectedValid);
    }
}
