using SendGrid;
using SendGrid.Helpers.Mail;

namespace Newsletter.Effects.Traits;

public interface EmailIO
{
    Task<Response> Send(
        string apiKey,
        EmailAddress from,
        EmailAddress to,
        string subject,
        string plainTextContent,
        string htmlContent,
        CancellationToken token);
}
