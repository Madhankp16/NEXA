using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NexaWork.Application.Common.Interfaces;

namespace NexaWork.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        _logger.LogInformation("Enterprise SMTP Dispatch -> To: {To} | Subject: {Subject}", to, subject);
        return Task.CompletedTask;
    }
}
