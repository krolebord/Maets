using System.Net.Mail;

namespace Maets.Services;

public interface ISmtpClientFactory
{
    string DisplayName { get; }

    SmtpClient CreateClient();
}
