using System.Net.Mail;
using Maets.Attributes;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Maets.Services.Identity.Implementations;

[Dependency(Lifetime = ServiceLifetime.Transient, Exposes = typeof(IEmailSender))]
internal class EmailSender : IEmailSender
{
    private readonly string _displayName;
    private readonly Lazy<SmtpClient> _client;

    public EmailSender(ISmtpClientFactory clientFactory)
    {
        _displayName = clientFactory.DisplayName;
        _client = new Lazy<SmtpClient>(clientFactory.CreateClient);
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MailMessage(_displayName, email, subject, htmlMessage)
        {
            IsBodyHtml = true
        };
        return _client.Value.SendMailAsync(message);
    }
}
