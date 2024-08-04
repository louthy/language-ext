using SendGrid;
using SendGrid.Helpers.Mail;
using Newsletter.Effects.Traits;

namespace Newsletter.Effects.Live;

public class Email : EmailIO
{
    public static readonly EmailIO Default = new Email();
    
    public async Task<Response> Send(
        string apiKey,
        EmailAddress from,
        EmailAddress to,
        string subject,
        string plainTextContent,
        string htmlContent,
        CancellationToken token)
    {
        var client   = new SendGridClient(apiKey);
        var msg      = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg, token);
        return response;
    }    
}
