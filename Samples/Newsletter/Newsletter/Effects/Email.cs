using Newsletter.Effects.Traits;
using SendGrid.Helpers.Mail;

namespace Newsletter.Effects;

/// <summary>
/// Json parser
/// </summary>
public static class Email<M, RT>
    where RT : 
        Has<M, EmailIO>,
        Reads<M, RT, Config>
    where M :
        Monad<M>,
        Fallible<M>,
        Stateful<M, RT>
{
    static readonly K<M, EmailIO> trait =
        Stateful.getsM<M, RT, EmailIO>(rt => rt.Trait);

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
