using System.Net.Mail;

namespace Maets.Services.Identity;

public interface ISmtpClientFactory
{
    string DisplayName { get; }

    SmtpClient CreateClient();
}
