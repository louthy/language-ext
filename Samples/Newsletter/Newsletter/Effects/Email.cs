using Newsletter.Effects.Traits;
using SendGrid.Helpers.Mail;

namespace Newsletter.Effects;

/// <summary>
/// Json parser
/// </summary>
public static class Email<M, RT>
    where RT : 
        Has<M, EmailIO>,
        Has<M, Config>
    where M :
        Monad<M>,
        Fallible<M>
{
    static K<M, EmailIO> trait =>
        Has<M, RT, EmailIO>.ask;

    /// <summary>
    /// Send an email
    /// </summary>
    public static K<M, Unit> send(
        EmailAddress from,
        EmailAddress to,
        string subject,
        string plainTextContent,
        string htmlContent) =>
        from key in Config<M, RT>.sendGridApiKey
        from eml in trait
        from res in liftIO(io => eml.Send(key, @from, to, subject, plainTextContent, htmlContent, io.Token))
        select unit;
}
